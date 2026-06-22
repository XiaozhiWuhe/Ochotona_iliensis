using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    public float blinkSpeed = 2f; //闪烁速度

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
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