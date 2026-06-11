using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEnemy : MonoBehaviour
{
    public float speed = 2.5f;
    public int damage = 1;
    public float destroyX = -15f;   // 超出此X坐标销毁

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

        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }

        // 超出左边销毁
        if (transform.position.x < destroyX)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
            // 不销毁自身，只扣血
        }
    }

    // 供共鸣技能调用，消灭敌人
    public void DieByResonance()
    {
        if (!isAlive) return;
        isAlive = false;
        // 可在此添加音效
        Destroy(gameObject);
    }
}   