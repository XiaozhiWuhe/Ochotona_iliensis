using UnityEngine;
using UnityEngine.UI; //如果用 TextMeshPro，改成 using TMPro;

public class BlinkingText : MonoBehaviour
{
    private Text textComponent; //如果用 TMP，改成 TextMeshProUGUI
    public float blinkSpeed = 2f; // 闪烁速度

    void Start()
    {
        textComponent = GetComponent<Text>();
    }

    void Update()
    {
        if (textComponent == null) return;

        // 利用 Mathf.PingPong 动态计算透明度 Alpha 值 (从 0 到 1 循环)
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);

        Color newColor = textComponent.color;
        newColor.a = alpha;
        textComponent.color = newColor;
    }
}