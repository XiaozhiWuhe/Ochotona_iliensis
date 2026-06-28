using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public float speed = 2f;
    public int damage = 1;
    public float screenOffset = 2f;

    private bool isActivated = false;
    private bool isDead = false;
    private Rigidbody2D rb;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 屏幕边缘激活判定
        if (!isActivated)
        {
            if (mainCamera == null) return;
            float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            if (transform.position.x < rightEdge + screenOffset)
            {
                isActivated = true;
            }
            else
            {
                return;
            }
        }

        // 超出边界销毁（激活后才判断）
        if (transform.position.x < -15f)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (isActivated)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
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
        Destroy(gameObject);
    }

    // 响应玩家大招的瞬间死亡接口
    public void InstantKill()
    {
        Destroy(gameObject);
    }
}