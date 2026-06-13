using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    public int maxHealth = 3;      // 最大生命值
    public int currentHealth;      // 当前生命值

    [Header("无敌时间（可选）")]
    public float invincibleDuration = 1f;   // 受伤后无敌秒数
    private bool isInvincible = false;

    //生成物相关
    private bool hasShield = false;   // 是否拥有护盾

    private PlayerController playerController;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    //生成物相关
    public void AddShield()
    {
        hasShield = true;
        Debug.Log("获得护盾！");
    }

    //普通伤害接口（会被冲刺、闪避、受伤无敌帧免疫）
    public void TakeDamage(int damage)
    {
        // 1. 护盾优先：优先消耗护盾
        if (hasShield)
        {
            hasShield = false;
            Debug.Log("护盾抵消伤害，护盾消失");
            return;
        }

        // 2. 检查普通无敌状态
        if (isInvincible || currentHealth <= 0) return;

        // 3. 执行扣血
        ApplyCoreDamage(damage);
    }

    //真实伤害/环境强制伤害接口
    //无视任何无敌帧（包括闪避），直接扣血！但保留护盾的抵消机制。
    public void TakeTrueDamage(int damage)
    {
        if (currentHealth <= 0) return;

        // 这样的话如果有护盾，也可以让护盾先顶一刀
        if (hasShield)
        {
            hasShield = false;
            Debug.Log("环境危机伤害被护盾抵消！");
            return;
        }

        // 绕过isInvincible判定，直接执行核心扣血
        Debug.Log("遭受环境真实伤害（无视无敌状态）！");
        ApplyCoreDamage(damage);
    }

    //内部核心扣血与状态触发逻辑（私有函数，避免代码冗余）
    private void ApplyCoreDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"玩家实际扣血！当前血量: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 无论是受什么伤，受伤后都给予一段常规无敌时间，防止连续暴毙
            StartCoroutine(BecomeInvincible());
        }
    }

    //受伤后的无敌帧
    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    //闪避无敌帧
    public void SetInvincible(bool status)
    {
        isInvincible = status;

        if (status)
        {
            Debug.Log("闪避/冲刺中：开启无敌状态");
        }
        else
        {
            Debug.Log("结束闪避/冲刺：解除无敌状态");
        }
    }

    // 死亡处理
    void Die()
    {
        Debug.Log("玩家死亡！");

        //瘫痪控制器
        if (playerController != null) playerController.enabled = false;

        //死亡静止
        if (rb != null) rb.velocity = Vector2.zero;

        //后面在这里补充死亡UI，或LevelManager重新加载关卡
    }
}