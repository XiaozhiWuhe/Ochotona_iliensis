using UnityEngine;
using System.Collections.Generic;

public class LevelCardLayout : MonoBehaviour
{
    public RectTransform content;

    public float spacing = 500f;

    private List<RectTransform> cards = new List<RectTransform>();

    void Start()
    {
        InitCards();

        ArrangeCards();
    }

    void InitCards()
    {
        cards.Clear();

        foreach (RectTransform child in content)
        {
            cards.Add(child);
        }
    }

    void ArrangeCards()
    {
        int centerIndex =
            cards.Count / 2;

        for (int i = 0; i < cards.Count; i++)
        {
            float x =
                (i - centerIndex) * spacing;

            cards[i].anchoredPosition =
                new Vector2(x, 0);
        }
    }

    public List<RectTransform> GetCards()
    {
        return cards;
    }
}