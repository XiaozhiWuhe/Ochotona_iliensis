using UnityEngine;

public class DashShadow : MonoBehaviour
{
    private SpriteRenderer sr;
    private float activeTime;
    private float fadeTime;
    private float alpha;
    private Color shadowColor;

    public void Init(Sprite playerSprite, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float duration, int sortingLayerId, int sortingOrder)
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = gameObject.AddComponent<SpriteRenderer>();

        // 基础属性复制
        sr.sprite = playerSprite;
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;

        // 强制与主角对齐，但排序永远比主角低 1 层
        sr.sortingLayerID = sortingLayerId;
        sr.sortingOrder = sortingOrder - 1; // 保证在主角屁股后面，不会挡住主角，也不会被背景吞掉

        // 颜色与生命周期
        shadowColor = color;
        sr.color = shadowColor;

        activeTime = duration;
        fadeTime = duration;
        alpha = color.a;
    }

    void Update()
    {
        activeTime -= Time.deltaTime;

        if (activeTime <= 0)
        {
            alpha -= (shadowColor.a / fadeTime) * Time.deltaTime;
            sr.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, alpha);

            if (alpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}