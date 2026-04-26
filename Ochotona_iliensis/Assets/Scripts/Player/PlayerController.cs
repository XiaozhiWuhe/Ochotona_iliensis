using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float jumpForce = 10f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float rotationSpeed = 10f;//旋转平滑速度
    private float originalHeight;//初始碰撞箱高度

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

    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.D))
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
            targetUp = Vector2.up;
        }

    }

    void AlignToGround()
    {
        // 使用 Quaternion.FromToRotation 计算从世界坐标向上到目标法线的旋转
        Quaternion targetRotation = Quaternion.FromToRotation(Vector2.up, targetUp);

        // 增加旋转平滑度。冲刺时可以适当调高 rotationSpeed 
        float currentRotationSpeed = isDashing ? rotationSpeed * 2f : rotationSpeed;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
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

    System.Collections.IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            //向对应坡面的切线方向冲刺
            rb.velocity = transform.right * dashSpeed;
            yield return null;
        }
    }
}