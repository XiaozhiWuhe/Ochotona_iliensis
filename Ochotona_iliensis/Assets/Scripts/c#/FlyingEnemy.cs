using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("俯冲速度")]
    public float diveSpeed = 7f;          // 俯冲的总速度
    public float minVerticalSpeed = 3f;   // 最小垂直速度，防止角度太缓

    [Header("伤害设置")]
    public int damage = 1;

    private Vector2 diveDirection;        // 计算出的俯冲方向
    private float destroyX = -15f;
    private float destroyY = -8f;

    void Start()
    {
        // 找到玩家当前位置
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 计算从自己位置指向玩家位置的方向
            Vector2 toPlayer = player.transform.position - transform.position;

            // 如果玩家在自己右边（不应该发生，但预防一下），就用默认方向
            if (toPlayer.x > 0)
            {
                toPlayer = new Vector2(-1, -1);
            }

            // 确保方向是向左下的（玩家应该在左下方，因为角色在地面）
            // 如果算出的Y方向是向上，说明玩家比自己高，那就让飞禽水平甚至向下飞
            toPlayer.y = Mathf.Min(toPlayer.y, -0.5f); // 至少保证是向下的

            diveDirection = toPlayer.normalized;
        }
        else
        {
            // 找不到玩家，用默认斜左下方向
            diveDirection = new Vector2(-0.7f, -0.7f).normalized;
        }
    }

    void Update()
    {
        // 沿计算好的方向移动
        Vector3 movement = diveDirection * diveSpeed * Time.deltaTime;
        transform.Translate(movement);

        // 超出屏幕就销毁
        if (transform.position.x < destroyX || transform.position.y < destroyY)
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
                Debug.Log("飞禽击中了鼠兔！");
            }
            Destroy(gameObject);
        }
    }
}