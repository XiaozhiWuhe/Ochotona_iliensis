using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
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

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private bool isGrounded;
    private bool isDashing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        originalHeight = col.size.y;
    }
    private void FixedUpdate()
    {
        ApplyForwardForce();

        //当速度＞maxSpeed->尝试将其拉回
        //当!isDashing->减速
        if (!isDashing && rb.velocity.magnitude > maxSpeed)
        {
            //使用Lerp来实现平滑减速
            //10f是减速强度，数值越大，回落越快
            rb.velocity = Vector2.Lerp(rb.velocity, rb.velocity.normalized * maxSpeed, Time.fixedDeltaTime * 10f);
        }
    }

    void Update()
    {
        currentSpeed = rb.velocity.magnitude;

        CheckGround();
        HandleInput();
        AlignToGround();
    }

    //案件管理
    void HandleInput()
    {
        //跳跃w
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Jump();
        }

        //下蹲sssss
        if (Input.GetKey(KeyCode.S))
        {
            Crouch(true);
        }
        else
        {
            Crouch(false);
        }

        //冲刺d
        if (Input.GetKeyDown(KeyCode.D) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    //地面检测/射线检测
    void CheckGround()
    {
        //从碰撞体底部发射射线
        Vector2 origin = (Vector2)transform.position;
        float extraSearchDistance = 2.0f;
        RaycastHit2D hit = Physics2D.Raycast(origin, -transform.up, extraSearchDistance, groundLayer);

        isGrounded = hit.collider != null;

        //法线记录
        if (isGrounded)
        {
            //只有在地面的情况下，才更新目标向上矢量为地面法线
            isGrounded = true;
            targetUp = hit.normal;

            //地面法线
            Debug.DrawRay(hit.point, hit.normal * 2f, Color.blue);
            //玩家当前的向上方向
            Debug.DrawRay(transform.position, transform.up * 2f, Color.green);
        }
        else
        {
            //在空中时，慢慢恢复垂直向上
            isGrounded = false;
            targetUp = Vector2.Lerp(targetUp, Vector2.up, Time.deltaTime * 1.5f);
            //targetUp = Vector2.up;
        }

    }

    void AlignToGround()
    {
        // 使用 Quaternion.FromToRotation 计算从世界坐标向上到目标法线的旋转
        Quaternion targetRotation = Quaternion.FromToRotation(Vector2.up, targetUp);

        // 增加旋转平滑度。冲刺时可以适当调高 rotationSpeed 
        float speed = isGrounded ? rotationSpeed : rotationSpeed * 0.5f;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Crouch(bool isCrouching)
    {
        if (isCrouching)
        {
            col.size = new Vector2(col.size.x, originalHeight * 0.5f);
            //下蹲时额外施加一个向下的力
            if (!isGrounded) rb.AddForce(Vector2.down * 5f);
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

        //闪避无敌帧
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.SetInvincible(true);
        }

        //不清除速度，而是直接设定一个很高的目标冲刺速度
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