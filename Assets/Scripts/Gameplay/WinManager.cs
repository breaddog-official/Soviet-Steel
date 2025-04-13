using System.Collections.Generic;
using ArcadeVP;
using Mirror;
using Scripts.Gameplay;
using UnityEngine;

public class WinManager : NetworkBehaviour
{
    [SerializeField] private RoadManager roadManager;

    private readonly SyncList<uint> winnersPlaces = new();


    private void OnEnable()
    {
        roadManager.OnPlayerReachedRound += OnReachRound;
    }

    private void OnDisable()
    {
        roadManager.OnPlayerReachedRound += OnReachRound;
    }


    [Server]
    private void OnReachRound(ArcadeVehicleNetwork network, int round)
    {
        if (round == GameManager.GameMode.rounds && !winnersPlaces.Contains(network.netId))
        {
            winnersPlaces.Add(network.netId);
            network.SetWin(true/*winnersPlaces.Count*/);
        }
    }

    public IReadOnlyList<uint> GetPlaces() => winnersPlaces;
    public int GetPlace(uint player) => winnersPlaces.IndexOf(player);
}
