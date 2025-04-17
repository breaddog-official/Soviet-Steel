using ArcadeVP;
using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.Gameplay;
using UnityEngine;

public class InvisibleHand : MonoBehaviour
{
    [SerializeField, MinMaxSlider(0f, 2f)] protected Vector2 playersSpeedRange = new(1f, 1.5f);
    [SerializeField, CurveRange(0, 0, 1, 1, EColor.Orange)] protected AnimationCurve playersSpeedCurve;
    [Space]
    [SerializeField, MinMaxSlider(0f, 2f)] protected Vector2 aiSpeedRange = new(1f, 1.5f);
    [SerializeField, CurveRange(0, 0, 1, 1, EColor.Orange)] protected AnimationCurve aiSpeedCurve;

    private void Update()
    {
        if (!NetworkServer.active)
            return;

        var places = GameManager.Instance.RoadManager.GetPlaces();

        for (int i = 0; i < places.Count; i++)
        {
            if (places[i].TryFindNetworkByID(out ArcadeVehicleNetwork network))
            {
                var range = network.AI ? aiSpeedRange : playersSpeedRange;
                var curve = network.AI ? aiSpeedCurve : playersSpeedCurve;

                float speedMultiplier = Mathf.Lerp(range.x, range.y, curve.Evaluate(i / (float)(places.Count - 1)));
                network.SetSpeedMultiplier(speedMultiplier);
            }
        }
    }
}
