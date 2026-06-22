using UnityEngine;
using UnityEngine.UI; 

public class SkillEnergyUI : MonoBehaviour
{
    [Header("挂载了满能量图标的子物体")]
    public Image energyBarImage; 

    private PlayerAbility playerAbility;

    void Start()
    {
        //去找挂在玩家身上的技能脚本
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (energyBarImage == null)
        {
            Debug.LogError("未关联 EnergyBarImage！");
        }
    }

    void Update()
    {
        if (playerAbility == null || energyBarImage == null) return;

        //填充比例 = 当前能量 / 最大能量
        //得到一个0.0到1.0之间的浮点数
        float fillPercentage = playerAbility.currentEnergy / playerAbility.maxEnergy;

        //将百分比直接赋值给Filled模式Image的fillAmount
        energyBarImage.fillAmount = fillPercentage;

        //当能量满 100% 时，触发高亮或者完全显现；没满时可以做一丁点半透明处理
        if (fillPercentage >= 1f)
        {
            energyBarImage.color = new Color(1f, 1f, 1f, 1f); //恢复100不透明度，完美展现满能亮色
        }
        else
        {
            //未满时，给高亮层增加一点点半透明（0.9f），能更好地透出底层的空状态灰色暗底
            energyBarImage.color = new Color(1f, 1f, 1f, 0.9f);
        }
    }
}