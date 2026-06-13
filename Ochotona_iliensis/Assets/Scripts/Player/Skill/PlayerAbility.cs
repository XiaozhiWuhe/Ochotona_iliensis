using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [Header("能量设置")]
    public float maxEnergy = 100f;          // 最大能量值
    public float currentEnergy = 0f;        // 当前能量值
    public float energyGainPerSecond = 10f; // 每秒匀速自然积攒的能量
    public float energyCost = 100f;         // 释放技能需要的能量消耗
    public float energyRefundPerKill = 15f; // 没消灭一个敌人返还的能量

    [Header("技能效果范围")]
    public float killRadius = 4f;           // 消灭敌人的半径范围
    public LayerMask enemyLayer;            // 敌人的物理图层

    private bool isSilenced = false; // 是否被沉默

    void Update()
    {
        if (isSilenced) return;

        // 匀速自然积攒能量
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyGainPerSecond * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy); // 限制在0~最大值之间
        }

        // 监听技能释放按键 E
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryExecuteBlast();
        }
    }

    // 尝试释放大招
    void TryExecuteBlast()
    {
        // 检查能量是否足够
        if (currentEnergy < energyCost)
        {
            Debug.Log($"能量不足！当前能量: {(int)currentEnergy}/{energyCost}");
            return;
        }

        // 扣除能量
        currentEnergy -= energyCost;

        // 执行范围消灭逻辑
        ExecuteCircleKill();
    }

    // 范围物理检测与消灭
    void ExecuteCircleKill()
    {
        Debug.Log("释放范围消灭技能！");

        // 抓取圆圈范围内的所有敌人碰撞体
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, killRadius, enemyLayer);
        int killCount = 0;

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            bool enemyKilled = false;

            // 尝试检测是不是飞行敌人
            FlyingEnemy flyEnemy = enemyCollider.GetComponent<FlyingEnemy>();
            if (flyEnemy != null)
            {
                flyEnemy.InstantKill();
                enemyKilled = true;
            }

            // 尝试检测是不是地面敌人
            GroundEnemy groundEnemy = enemyCollider.GetComponent<GroundEnemy>();
            if (groundEnemy != null)
            {
                groundEnemy.InstantKill();
                enemyKilled = true;
            }

            // 尝试检测是不是追踪敌人
            TrackingEnemy trackEnemy = enemyCollider.GetComponent<TrackingEnemy>();
            if (trackEnemy != null)
            {
                trackEnemy.InstantKill();
                enemyKilled = true;
            }

            // 如果未来加了新怪忘记写脚本，直接物理蒸发它
            if (!enemyKilled)
            {
                Destroy(enemyCollider.gameObject);
                enemyKilled = true;
            }

            if (enemyKilled) killCount++;
        }

        //返还能量逻辑
        if (killCount > 0)
        {
            float totalRefund = killCount * energyRefundPerKill;
            currentEnergy += totalRefund;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
            Debug.Log($"成功消灭 {killCount} 个敌人，返还能量: {totalRefund}，当前能量: {currentEnergy}");
        }
    }

    // 在Scene窗口画一个红色的圈，方便策划直观调整和预览技能范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);
    }

    //提供给危机区域调用的公共方法
    public void SetSilence(bool silenceState)
    {
        isSilenced = silenceState;
        if (silenceState)
        {
            Debug.Log("玩家技能已被禁用！");
        }
        else
        {
            Debug.Log("玩家技能恢复正常！");
        }
    }
}