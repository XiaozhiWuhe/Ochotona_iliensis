using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed = 3f;          // 向左飞的速度
    public int damage = 1;            // 碰撞造成的伤害

    void Update()
    {
        // 向左移动
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 超出屏幕左边（x < -15）就销毁，节省性能
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
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
}