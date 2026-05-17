using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3f; // 向左移动速度

    [Header("伤害设置")]
    public int damage = 1;

    private void Update()
    {
        // 向左移动
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // 当移出屏幕左侧很远时销毁，防止无限堆积
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log(gameObject.name + " 攻击了鼠兔！");
            }
            
            // 销毁敌人
            Destroy(gameObject);
        }
    }
}