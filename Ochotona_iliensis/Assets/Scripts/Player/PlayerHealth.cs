using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 临时生命值脚本!!!
/// </summary>
/// 
public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 3;      // 最大生命值
    public int currentHealth;      // 当前生命值

    [Header("无敌时间（可选）")]
    public float invincibleDuration = 1f;   // 受伤后无敌秒数
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"玩家生命值初始化：{currentHealth}/{maxHealth}");
    }

    // 受到伤害
    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("无敌状态，免疫伤害");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"玩家受到 {damage} 点伤害，剩余生命 {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            // 进入无敌状态
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // 治疗
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log($"玩家恢复 {amount} 点生命，当前生命 {currentHealth}/{maxHealth}");
    }

    // 死亡
    private void Die()
    {
        Debug.Log("玩家死亡！游戏结束");
        // 这里可以加游戏结束逻辑
        // Time.timeScale = 0f;
    }

    // 无敌
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
        Debug.Log("无敌状态结束");
    }
}