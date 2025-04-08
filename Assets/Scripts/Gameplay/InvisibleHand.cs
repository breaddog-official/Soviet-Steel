using Mirror;
using NaughtyAttributes;
using Scripts.Gameplay;
using UnityEngine;

public class InvisibleHand : MonoBehaviour
{
    [SerializeField, MinMaxSlider(0f, 2f)] protected Vector2 speedRange = new(1f, 1.5f);
    [SerializeField, CurveRange(0, 0, 1, 1, EColor.Orange)] protected AnimationCurve speedCurve;

    private void Update()
    {
        if (!NetworkServer.active)
            return;

        var places = GameManager.Instance.RoadManager.GetPlaces();

        for (int i = 0; i < places.Count; i++)
        {
            float speedMultiplier = Mathf.Lerp(speedRange.x, speedRange.y, speedCurve.Evaluate(i / places.Count));
            places[i].SetSpeedMultiplier(speedMultiplier);
        }
    }
}
