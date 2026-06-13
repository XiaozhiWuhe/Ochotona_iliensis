using UnityEngine;
using System.Collections;

public class HazardZone : MonoBehaviour
{
    [Header("时间参数")]
    public float warnDuration = 2f;         // 预警持续时间（秒）

    [Header("游戏数值")]
    public int damage = 1;                  // 激活瞬间造成的伤害

    [Header("组件引用")]
    private SpriteRenderer sr;
    private BoxCollider2D zoneCollider;
    private bool isActivated = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        zoneCollider = GetComponent<BoxCollider2D>();

        // 初始安全保障：关闭碰撞箱，防止预警期间就把人挡住或提前扣血
        if (zoneCollider != null)
        {
            zoneCollider.enabled = false;
            zoneCollider.isTrigger = true; // 设为 Trigger，方便后面做玩家进出（沉默）检测
        }
    }

    void Start()
    {
        // 一生成，立刻开始生命周期协程
        StartCoroutine(ZoneLifecycle());
    }

    IEnumerator ZoneLifecycle()
    {
        // 预警阶段
        float timer = 0f;

        while (timer < warnDuration)
        {
            timer += Time.deltaTime;
            // 红色的透明度随时间快速波动
            float alpha = Mathf.PingPong(Time.time * 8f, 0.5f) + 0.1f;
            sr.color = new Color(1f, 0f, 0f, alpha); // 闪烁红光
            yield return null;
        }

        // 激活瞬间
        isActivated = true;
        sr.color = new Color(1f, 0f, 0f, 0.6f); // 变为常亮且较深的红色

        if (zoneCollider != null)
        {
            zoneCollider.enabled = true; // 瞬间激活物理碰撞体
        }

        // Collider尺寸乘以物体的世界缩放
        Vector2 realSize = new Vector2(
            zoneCollider.size.x * transform.lossyScale.x,
            zoneCollider.size.y * transform.lossyScale.y
        );

        // 使用物理盒子重叠检测
        Collider2D hitPlayer = Physics2D.OverlapBox(transform.position, realSize, transform.eulerAngles.z, LayerMask.GetMask("Player"));

        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            // 扣血
            PlayerHealth health = hitPlayer.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeTrueDamage(damage);
            }

            // 既然激活瞬间玩家就在里面，直接在此顺手施加沉默，防止物理帧延迟导致漏判定
            PlayerAbility ability = hitPlayer.GetComponent<PlayerAbility>();
            if (ability != null)
            {
                ability.SetSilence(true);
            }
        }
    }

    // 持续阶段（常规进入：沉默技能）
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated) return; // 如果还没激活（还在预警），不触发沉默

        if (collision.CompareTag("Player"))
        {
            PlayerAbility ability = collision.GetComponent<PlayerAbility>();
            if (ability != null)
            {
                ability.SetSilence(true); // 沉默玩家
            }
        }
    }

    // 离开区域：恢复技能
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAbility ability = collision.GetComponent<PlayerAbility>();
            if (ability != null)
            {
                ability.SetSilence(false); // 解除沉默
            }
        }
    }

    // 确保绝对安全的兜底：如果危机区域被强行销毁（比如玩家跑远了被关卡管理器自动回收）
    private void OnDestroy()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerAbility ability = player.GetComponent<PlayerAbility>();
            if (ability != null) ability.SetSilence(false);
        }
    }
}