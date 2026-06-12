using UnityEngine;
using System.Collections;

public class PlayerFlightController : MonoBehaviour
{
    [Header("自动前行")]
    public float forwardSpeed = 8f;

    [Header("上下飞行参数")]
    public float flySpeed = 6f;

    [Header("A键减速参数")]
    public float slowSpeed = 3f; // 按住A时的前行速度

    [Header("闪避参数")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool canDash = true;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        // 核心：进入飞行关卡时，关闭重力，角色才能自由上下浮动
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }
    }

    void OnDisable()
    {
        // 恢复重力（以防切换回普通关卡）
        if (rb != null) rb.gravityScale = 1f;
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // 处理水平移动（正常前行 或 A键减速）
        float currentForwardSpeed = forwardSpeed;
        if (Input.GetKey(KeyCode.A))
        {
            currentForwardSpeed = slowSpeed; // 按住A减速
        }

        // 处理垂直移动（W上移，S下移）
        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.W)) verticalInput = 1f;
        if (Input.GetKey(KeyCode.S)) verticalInput = -1f;

        // 应用速度
        rb.velocity = new Vector2(currentForwardSpeed, verticalInput * flySpeed);
    }

    void HandleInput()
    {
        // 闪避D逻辑（与滑行完全相同）
        if (Input.GetKeyDown(KeyCode.D) && canDash && !isDashing)
        {
            StartCoroutine(FlightDash());
        }
    }

    IEnumerator FlightDash()
    {
        isDashing = true;
        canDash = false;

        // 闪避无敌帧开始
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null) health.SetInvincible(true);

        // 飞行闪避：沿当前水平方向猛冲
        rb.velocity = new Vector2(dashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        // 闪避无敌帧结束
        if (health != null) health.SetInvincible(false);

        isDashing = false;

        // 冷却计时
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}