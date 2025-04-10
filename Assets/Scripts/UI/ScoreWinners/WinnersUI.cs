using System.Collections.Generic;
using ArcadeVP;
using Scripts.Gameplay;
using UnityEngine;

public class WinnersUI : MonoBehaviour
{
    [SerializeField] private WinnersInstanceUI winnerPrefab;
    [SerializeField] private Transform spawnParent;

    private readonly Dictionary<ArcadeVehicleNetwork, WinnersInstanceUI> spawnedWinners = new();


    private void Start()
    {
        var places = GameManager.Instance.RaceManager.GetPlaces();
        for (int i = 0; i < places.Count; i++)
        {
            if (RoadManager.TryToNetwork(places[i], out var network))
            {
                var instance = Instantiate(winnerPrefab, spawnParent);
                instance.Initialize(network, i);
            }
        }
    }
}
