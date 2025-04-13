using ArcadeVP;
using Scripts.Gameplay;
using TMPro;
using UnityEngine;

public class DistanceToCar : MonoBehaviour
{
    [SerializeField] private ArcadeVehicleNetwork vehicleNetwork;
    [SerializeField, Range(0, 1)] private float smoothAmount = 0.5f;

    private TMP_Text distanceText;
    private float lastDistance;


    private void Awake()
    {
        distanceText = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        if (ArcadeVehicleNetwork.LocalPlayerNetwork == null || vehicleNetwork == null)
            return;

        if (GameManager.Instance.RoadManager.GetPlace(ArcadeVehicleNetwork.LocalPlayerNetwork.netId) - 1 == GameManager.Instance.RoadManager.GetPlace(vehicleNetwork.netId))
        {
            var playerMarker = GameManager.Instance.RoadManager.GetPlayer(ArcadeVehicleNetwork.LocalPlayerNetwork.netId).marker;
            var currentMarker = GameManager.Instance.RoadManager.GetPlayer(vehicleNetwork.netId).marker;
            var distance = GameManager.Instance.RoadManager.GetDistanceBeetweenMarkers(playerMarker, currentMarker);

            distanceText.text = ((int)Mathf.Lerp(lastDistance, distance, (1f - smoothAmount) * Time.deltaTime)).ToString();
            distanceText.enabled = true;

            lastDistance = distance;
        }
        else
        {
            distanceText.enabled = false;
        }
    }
}
