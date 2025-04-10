using System;
using ArcadeVP;
using Scripts.Gameplay;
using TMPro;
using UnityEngine;

public class WinnersInstanceUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text placeText;
    [Space]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private string timeFormat;


    public void Initialize(ArcadeVehicleNetwork network, int place)
    {
        nameText.text = network.name;

        placeText.text = place.ToString();

        var time = GameManager.Instance.RoadManager.GetPlayers()[network.netId].LastTime;
        timeText.text = TimeSpan.FromSeconds(time).ToString(timeFormat);
    }
}
