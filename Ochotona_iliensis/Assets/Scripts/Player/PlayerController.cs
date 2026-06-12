using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("外观与动感表现")]
    public Transform visualModel;           //拖入专门放图片/动画的子物体，避免物理碰撞体跟着倾斜
    public float maxLeanAngle = 15f;        //最大倾斜角度（度数，比如15度）
    public float leanSmoothing = 10f;       //倾斜变化的平滑速度
    private float currentLeanAngle;         //当前的倾斜角缓存

    [Header("调试信息")]
    public float currentSpeed;

    [Header("自动移动")]
    public float baseSpeed = 8f;
    public float maxSpeed = 8f;
    public float acceleration = 2f;

    [Header("移动参数")]
    public float jumpForce = 10f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float rotationSpeed = 10f;//旋转平滑速度
    private float originalHeight;//初始碰撞箱高度
    private float dashCooldown = 1f; //冲刺冷却时间
    private bool canDash = true; //是否可以冲刺

    [Header("地面检测")]
    public float groundCheckDistance = 0.1f; //向下检测距离
    public LayerMask groundLayer;
    private Vector2 targetUp = Vector2.up;
    private Vector2 currentGroundNormal = Vector2.up; //精准记录当前踩着的地面法线方向

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private bool isGrounded;
    private bool isDashing;

    [Header("土狼时间")]
    public float coyoteTimeDuration = 0.15f; //土狼时间持续长度/秒
    private float coyoteTimeCounter;        //土狼时间计时器

    [Header("跳跃缓冲")]
    public float jumpBufferDuration = 0.15f; // 跳跃指令缓存持续长度（秒）
    private float jumpBufferCounter;        // 跳跃指令缓存计时器

    [Header("可变跳跃高度")]
    [Range(0f, 1f)]
    public float jumpCutMultiplier = 0.4f;  //松开W时，向上速度保留的比例（数值越小，小跳越矮，推荐0.3~0.5）
    private bool isJumpingUp;               //标记玩家当前是否处于自主跳跃的上升阶段

    [Header("贴地吸附力")]
    public float groundingForce = 5f;       //贴地吸附力大小（数值越大，高坡向下跑时贴地越紧）

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        originalHeight = col.size.y;

        if (visualModel == null && transform.childCount > 0)
        {
            visualModel = transform.GetChild(0);
        }
    }

    private void FixedUpdate()
    {
        ApplyForwardForce();
        ApplyGroundingForce(); //在物理帧应用沿法线向下的贴地力

        //当速度＞maxSpeed->尝试将其拉回
        //当!isDashing->减速
        if (!isDashing && rb.velocity.magnitude > maxSpeed)
        {
            //使用Lerp来实现平滑减速
            //10f是减速强度，数值越大，回落越快
            rb.velocity = Vector2.Lerp(rb.velocity, rb.velocity.normalized * maxSpeed, Time.fixedDeltaTime * 10f);
        }

        //可变跳跃高度物理控制
        float verticalVelocity = Vector2.Dot(rb.velocity, transform.up);

        if (isJumpingUp)
        {
            if (!Input.GetKey(KeyCode.W))
            {
                if (verticalVelocity > 0f)
                {
                    Vector2 horizontalComponent = rb.velocity - ((Vector2)transform.up * verticalVelocity);
                    Vector2 newVerticalComponent = (Vector2)transform.up * (verticalVelocity * jumpCutMultiplier);

                    rb.velocity = horizontalComponent + newVerticalComponent;
                }
                isJumpingUp = false;
            }
            else if (verticalVelocity <= 0f)
            {
                isJumpingUp = false;
            }
        }
    }

    void Update()
    {
        currentSpeed = rb.velocity.magnitude;

        CheckGround();
        HandleInput();
        ExecuteJumpLogic();
        AlignToGround();
        ApplyProceduralLeaning(); //每帧更新外观模型的身体倾斜
    }

    //基于角色运动状态的程序化倾斜表现
    void ApplyProceduralLeaning()
    {
        if (visualModel == null) return;

        float targetLean = 0f;

        if (isDashing)
        {
            //冲刺状态：极度前倾，表现出爆发感
            targetLean = maxLeanAngle * 1.5f;
        }
        else if (isGrounded)
        {
            //地面跑步状态：根据当前速度占最大速度的比例，动态计算前倾角度,越接近最大速度，往前趴得越用力
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            targetLean = speedRatio * maxLeanAngle;
        }
        else
        {
            //空中状态：根据上升/下落趋势微调倾斜
            float verticalVelocity = Vector2.Dot(rb.velocity, transform.up);
            if (verticalVelocity > 0.1f)
            {
                targetLean = maxLeanAngle * 0.4f; //跃起上升时稍微前倾
            }
            else if (verticalVelocity < -0.1f)
            {
                targetLean = -maxLeanAngle * 0.3f; //下落挺胸或身体后仰
            }
        }

        //使用Lerp让倾斜过渡极其丝滑
        currentLeanAngle = Mathf.Lerp(currentLeanAngle, targetLean, Time.deltaTime * leanSmoothing);

        //仅仅改变子物体的局部旋转（Z轴），不污染父物体的物理刚体旋转
        visualModel.localRotation = Quaternion.Euler(0, 0, -currentLeanAngle);
    }

    //按键管理与指令收集
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            jumpBufferCounter = jumpBufferDuration;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Crouch(true);
        }
        else
        {
            Crouch(false);
        }

        if (Input.GetKeyDown(KeyCode.D) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    void ExecuteJumpLogic()
    {
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
        }
    }

    //地面检测/射线检测
    void CheckGround()
    {
        Vector2 upVector = (Vector2)transform.up;
        Vector2 feetPosition = (Vector2)col.bounds.center + (-upVector * col.bounds.extents.y);
        RaycastHit2D hit = Physics2D.Raycast(feetPosition, -upVector, groundCheckDistance, groundLayer);

        isGrounded = hit.collider != null;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTimeDuration;
            targetUp = hit.normal;
            currentGroundNormal = hit.normal;
            Debug.DrawRay(hit.point, hit.normal * 1f, Color.blue);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            targetUp = Vector2.Lerp(targetUp, Vector2.up, Time.deltaTime * 1.5f);
            currentGroundNormal = Vector2.Lerp(currentGroundNormal, Vector2.up, Time.deltaTime * 1.5f);
        }

        Debug.DrawRay(feetPosition, -upVector * groundCheckDistance, Color.green);
    }

    //应用沿当前地面法线反方向的下压吸附力
    void ApplyGroundingForce()
    {
        if (coyoteTimeCounter > 0f && !isJumpingUp && !isDashing)
        {
            Vector2 forceDirection = -currentGroundNormal;
            rb.AddForce(forceDirection * groundingForce, ForceMode2D.Force);
            Debug.DrawRay(transform.position, forceDirection * 1.5f, Color.magenta);
        }
    }

    void AlignToGround()
    {
        Quaternion targetRotation = Quaternion.FromToRotation(Vector2.up, targetUp);
        float speed = isGrounded ? rotationSpeed : rotationSpeed * 0.5f;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);
    }

    void Jump()
    {
        isJumpingUp = true;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }

    void Crouch(bool isCrouching)
    {
        if (isCrouching)
        {
            col.size = new Vector2(col.size.x, originalHeight * 0.5f);
            if (coyoteTimeCounter <= 0f)
            {
                rb.AddForce(Vector2.down * 5f);
            }
        }
        else
        {
            col.size = new Vector2(col.size.x, originalHeight);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.SetInvincible(true);
        }

        rb.velocity = transform.right * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        if (health != null)
        {
            health.SetInvincible(false);
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        Debug.Log("闪避可释放状态！");
    }

    void ApplyForwardForce()
    {
        if (isDashing) return;

        float currentForwardSpeed = Vector2.Dot(rb.velocity, transform.right);

        if (currentForwardSpeed < baseSpeed)
        {
            rb.AddForce(transform.right * acceleration, ForceMode2D.Force);
        }
    }
}