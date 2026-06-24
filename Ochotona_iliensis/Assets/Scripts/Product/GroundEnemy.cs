using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour
{
    public float speed = 2f;          // 向左移动速度
    public int damage = 1;            // 碰撞伤害
    public float activationDistance = 8f;

    private bool isActivated = false;
    private Transform player;
    private bool isDead = false;
    private Rigidbody2D rb;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 距离激活判定
        if (!isActivated)
        {
            if (player == null) return;
            if (transform.position.x - player.position.x < activationDistance)
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
        // 只在激活后施加物理速度
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