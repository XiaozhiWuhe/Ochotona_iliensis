using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed = 3f;          // 向左飞的速度
    public int damage = 1;            // 碰撞造成的伤害

    // 距离激活相关
    public float activationDistance = 8f;   // 距离玩家多远时激活（X轴距离）
    private bool isActivated = false;
    private Transform player;

    void Start()
    {
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // 检查是否应该激活
        if (!isActivated)
        {
            if (player == null) return;
            // 当敌人位于玩家前方，且距离小于激活距离时，激活
            if (transform.position.x - player.position.x < activationDistance)
            {
                isActivated = true;
            }
            else
            {
                return; // 未激活，不执行移动逻辑
            }
        }

        // 激活后执行原有逻辑（向左移动 + 销毁）
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x < -15f)
            Destroy(gameObject);
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

    // s响应玩家大招的瞬间死亡接口
    public void InstantKill()
    {
        // 特效音效添加位置
        Destroy(gameObject);
    }
}