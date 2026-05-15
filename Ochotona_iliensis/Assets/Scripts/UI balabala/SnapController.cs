using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class SnapController : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;

    public RectTransform content;

    public RectTransform viewport;

    private List<RectTransform> cards;

    public LevelCardLayout layout;

    private Tween snapTween;

    void Start()
    {
        cards = layout.GetCards();
        CenterOnFirstCard();
    }
    public void OnBeginDrag(
        PointerEventData eventData)
    {
        //Í£Ö¹µ±Ç°Îü¸½¶¯»­
        snapTween?.Kill();
    }

    public void OnEndDrag(
        PointerEventData eventData)
    {
        StartSnap();
    }

    void StartSnap()
    {
        RectTransform closest = GetClosestCard();

        float centerX = viewport.position.x;

        float offset = centerX - closest.position.x;

        Vector2 targetPosition = content.anchoredPosition + new Vector2(offset, 0);

        snapTween = content.DOAnchorPos(targetPosition, 0.35f).SetEase(Ease.OutCubic);
    }

    RectTransform GetClosestCard()
    {
        RectTransform closest = null;

        float minDistance = float.MaxValue;

        float centerX = viewport.position.x;

        foreach (RectTransform card in cards)
        {
            float distance = Mathf.Abs(card.position.x - centerX);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = card;
            }
        }
        return closest;
    }

    public void CenterOnFirstCard()
    {
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
    }
}