using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("需要动态修改的数字文本")]
    public TextMeshProUGUI healthNumberText;

    private void Start()
    {
        //游戏刚开始，去找场景里的玩家，拿到他的初始血量并显示
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            UpdateHealthUI(playerHealth.currentHealth);
        }
    }

    //提供给外部（PlayerHealth）调用的公共方法
    public void UpdateHealthUI(int currentHealth)
    {
        if (healthNumberText != null)
        {
            //将整型的血量数字转化为字符串，强行塞给 UI 文本组件
            healthNumberText.text = currentHealth.ToString();
        }
    }
}