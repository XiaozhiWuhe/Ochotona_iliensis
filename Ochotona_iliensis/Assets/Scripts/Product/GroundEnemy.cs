using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public float speed = 2f;          // 向左移动速度
    public int damage = 1;            // 碰撞伤害

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 确保刚体是 Dynamic，重力正常
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;          // 受重力影响，会自然落在地面上
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 防止倒下
    }

    void FixedUpdate()
    {
        // 每帧给向左的速度，y 方向保持当前速度（重力会处理垂直方向）
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    private void OnBecameInvisible()
    {
        // 离开屏幕时销毁（避免无限累积）
        Destroy(gameObject);
    }
}