using UnityEngine;
using System.Collections.Generic;

public class ScaleController : MonoBehaviour
{
    public RectTransform viewport;

    private List<RectTransform> cards;

    public LevelCardLayout layout;

    public float maxDistance = 1000f;

    public AnimationCurve scaleCurve;

    public float minScale = 0.8f;

    public float maxScale = 1.2f;

    void Start()
    {
        cards = layout.GetCards();
    }

    void Update()
    {
        UpdateScale();
    }

    void UpdateScale()
    {
        float centerX =
            viewport.position.x;

        foreach (RectTransform card in cards)
        {
            float distance =
                Mathf.Abs(
                    card.position.x -
                    centerX
                );

            float t =
                Mathf.Clamp01(
                    distance / maxDistance
                );

            float curve =
                scaleCurve.Evaluate(1 - t);

            float scale =
                Mathf.Lerp(
                    minScale,
                    maxScale,
                    curve
                );

            card.localScale =
                Vector3.one * scale;
        }
    }
}