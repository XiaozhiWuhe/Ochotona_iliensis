using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEnemy : MonoBehaviour
{
    [Header("移动速度（由生成器设置）")]
    public float chaseSpeed = 3f;      // 追踪速度标量（会覆盖默认值）

    [Header("伤害与销毁")]
    public int damage = 1;
    public float destroyX = -15f;      // 超出左边销毁
    public float destroyY = 20f;       // 超出上下边界销毁

    private Transform player;
    private bool isAlive = true;

    void Start()
    {
        InvokeRepeating("FindPlayer", 0f, 0.5f);
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!isAlive) return;
        if (player == null) return;

        // 边界销毁
        if (transform.position.x < destroyX || Mathf.Abs(transform.position.y) > destroyY)
        {
            Destroy(gameObject);
            return;
        }

        // 直线追踪：向玩家位置移动
        float step = chaseSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, player.position, step);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) return;
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
            // 碰撞后不消失，继续追踪（共鸣才消灭）
        }
    }

    public void InstantKill()   // 共鸣调用
    {
        if (!isAlive) return;
        isAlive = false;
        Destroy(gameObject);
    }
}