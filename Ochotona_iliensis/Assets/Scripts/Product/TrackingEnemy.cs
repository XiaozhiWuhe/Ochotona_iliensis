using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEnemy : MonoBehaviour
{
    public float speed = 2.5f;
    public int damage = 1;
    public float destroyX = -15f;

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
        }
    }

    public void DieByResonance()
    {
        InstantKill(); // 让原本的共鸣接口也走统一的销毁逻辑
    }

    // 响应玩家大招的瞬间死亡接口
    public void InstantKill()
    {
        if (!isAlive) return;
        isAlive = false;
        Destroy(gameObject);
    }
}