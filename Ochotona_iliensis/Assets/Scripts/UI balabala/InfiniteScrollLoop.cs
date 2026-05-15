using UnityEngine;
using System.Collections.Generic;

public class InfiniteScrollLoop : MonoBehaviour
{
    public LevelCardLayout layout;
    public RectTransform viewport;
    private List<RectTransform> cards;
    private float itemWidthWithSpacing;

    void Start()
    {
        cards = layout.GetCards();
        // 计算单个项占用的总宽度（卡片宽 + 间隔）
        itemWidthWithSpacing = layout.spacing;
    }

    void Update()
    {
        foreach (var card in cards)
        {
            // 获取卡片相对于 Viewport 中心点的偏移
            float localX = viewport.InverseTransformPoint(card.position).x;

            // 如果卡片向左移出了太远，就把它拨到右边
            if (localX < -itemWidthWithSpacing * cards.Count / 2f)
            {
                Vector2 pos = card.anchoredPosition;
                pos.x += cards.Count * itemWidthWithSpacing;
                card.anchoredPosition = pos;
            }
            // 如果卡片向右移出了太远，就把它拨到左边
            else if (localX > itemWidthWithSpacing * cards.Count / 2f)
            {
                Vector2 pos = card.anchoredPosition;
                pos.x -= cards.Count * itemWidthWithSpacing;
                card.anchoredPosition = pos;
            }
        }
    }
}