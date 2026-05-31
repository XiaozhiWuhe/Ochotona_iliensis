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

    private PlayerController playerController;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"玩家受伤！当前血量: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
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
            Debug.Log("闪避");
        }
        else
        {
            Debug.Log("结束闪避");
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