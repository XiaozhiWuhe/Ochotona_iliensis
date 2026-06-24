using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public float fallSpeed = 4f;      // 下落速度
    public int damage = 1;            // 碰撞伤害
    public float destroyY = -10f;     // 低于这个Y坐标就销毁（防止无限下落）
    public float activationDistance = 8f;   // 距离玩家多远时开始下落

    private bool isActivated = false;
    private Transform player;
    private bool isDead = false;

    void Start()
    {
        Debug.Log("===== 碎石 Start 执行了！ =====");
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // 距离激活判定
        if (!isActivated)
        {
            if (player == null) return;
            if (transform.position.x - player.position.x < activationDistance)
            {
                isActivated = true;
            }
            else
            {
                return; // 未激活，静止不动
            }
        }

        //  // 激活后开始下落
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // 如果掉出屏幕底部，销毁自身
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    // 碰撞检测（碰到玩家）
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            isDead = true;
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
            // 石头砸中人后碎裂消失
            Destroy(gameObject);
        }
    }
}