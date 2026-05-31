using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffCrack : MonoBehaviour
{
    [Header("伤害设置")]
    public int damage = 1; // 造成1点伤害

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测碰撞的是不是玩家
        if (other.CompareTag("Player"))
        {
            // 获取玩家身上的生命值脚本
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("伊犁鼠兔撞到了崖缝！受到1点伤害。");
            }
            else
            {
                Debug.LogWarning("玩家身上未找到 PlayerHealth 脚本！");
            }
           
        }
    }
}