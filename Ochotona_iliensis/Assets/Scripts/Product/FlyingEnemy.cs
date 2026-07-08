using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("音效")]
    public AudioClip appearSound; // 出场音效

    public float speed = 3f;
    public int damage = 1;
    public float screenOffset = 2f;   // 距离屏幕右边缘多少单位时激活（建议1.5~3）

    private bool isActivated = false;
    private bool isDead = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 屏幕边缘激活判定（未激活时）
        if (!isActivated)
        {
            if (mainCamera == null) return;
            // 获取屏幕右边缘的世界坐标
            float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            // 如果敌人的x坐标小于（右边缘 + 偏移量），说明即将进入画面，激活
            if (transform.position.x < rightEdge + screenOffset)
            {
                isActivated = true;
                SoundManager.Instance.PlaySFX(appearSound); // 播放音效
            }
            else
            {
                return; // 未激活，静止不动
            }
        }

        // 激活后：向左移动 + 销毁
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

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // s响应玩家大招的瞬间死亡接口
    public void InstantKill()
    {
        // 特效音效添加位置
        Destroy(gameObject);
    }
}