using UnityEngine;
using EasyRoads3Dv3;
using NaughtyAttributes;
using Scripts.Extensions;
using System;
using Mirror;
using ArcadeVP;
using System.Collections.Generic;
using Scripts.Gameplay;
using Cysharp.Threading.Tasks;

public class RoadManager : NetworkBehaviour
{
    [SerializeField] private ERModularRoad road;
    [MinValue(0)]
    [SerializeField] private int firstMarker;
    [SerializeField] private float widthOffset;
    [Space]
    [SerializeField] private bool simpleMarkers;

    private readonly SyncDictionary<uint, PlayerScore> players = new();
    private readonly List<ArcadeVehicleNetwork> places = new();

    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedMarker;
    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedRound;



    public override void OnStartClient()
    {
        // Only client code, not for host
        if (NetworkServer.active)
            return;
        
        players.OnAdd += SyncPlaceAdd;
        players.OnRemove += SyncPlaceRemove;

        // Dictionary is populated before handlers are wired up so we
        // need to manually invoke OnAdd for each element.
        foreach (var key in players.Keys)
            players.OnAdd?.Invoke(key);
    }

    private async void SyncPlaceAdd(uint id)
    {
        ArcadeVehicleNetwork network;

        while (!TryToNetwork(id, out network))
        {
            await UniTask.NextFrame();
        }

        places.Add(network);
    }
    private void SyncPlaceRemove(uint network, PlayerScore score) => places.Remove(ToNetwork(network));

    [Server]
    public void AddPlayer(GameObject player)
    {
        var network = player.GetComponent<ArcadeVehicleNetwork>();
        var id = network.netId;
        var score = new PlayerScore
        {
            marker = firstMarker
        };

        players.Add(id, score);
        places.Add(network);
    }

    [Server]
    public void RemovePlayer(GameObject player)
    {
        var network = player.GetComponent<ArcadeVehicleNetwork>();
        var id = network.netId;

        players.Remove(id);
        places.Remove(network);
    }



    public void UpdatePlayers()
    {
        // Only server code
        if (NetworkServer.active)
        {
            foreach (var player in players)
            {
                if (!TryToNetwork(player.Key, out ArcadeVehicleNetwork network))
                    continue;

                int nextPoint = player.Value.marker.IncreaseInBoundsReturn(GetMarkers().Count);

                if (Vector3.Distance(network.transform.position, GetPoint(nextPoint)) < GetRadius())
                {
                    player.Value.SetMarker(nextPoint);
                    OnPlayerReachedMarker?.Invoke(network, player.Value.marker);

                    if (nextPoint == firstMarker)
                    {
                        player.Value.AddRound(GameManager.Instance.MatchTime);
                        OnPlayerReachedRound?.Invoke(network, player.Value.round);
                    }

                    //print($"Player: {network.name} Marker: {player.Value.marker} Round: {player.Value.round}");
                }
            }
        }

        SortPlaces();
    }

    public void SortPlaces()
    {
        places.Sort((f, s) => ComparePlayersPlaces(s, f));
    }


    public int GetPlace(ArcadeVehicleNetwork player) => places.IndexOf(player);

    public Vector3 GetPoint(int index) => GetMarkers()[index];
    public Vector3 GetNextPoint(int index) => GetPoint(index.IncreaseInBoundsReturn(GetMarkers().Count));
    public int GetNextPointIndex(int index) => index.IncreaseInBoundsReturn(GetMarkers().Count);

    public float GetRadius() => (road.GetRoadWidth() / 2f) + widthOffset;

    public IList<ArcadeVehicleNetwork> GetPlaces() => places;
    public IReadOnlyList<Vector3> GetMarkers() => road.splinePoints;
    public IReadOnlyDictionary<uint, PlayerScore> GetPlayers() => players;

    public static ArcadeVehicleNetwork ToNetwork(uint uid)
    {
        return NetworkServer.spawned.GetValueOrDefault(uid).GetComponent<ArcadeVehicleNetwork>();
    }

    public static bool TryToNetwork(uint uid, out ArcadeVehicleNetwork network)
    {
        network = null;
        return NetworkServer.spawned.TryGetValue(uid, out var identity) && identity.TryGetComponent<ArcadeVehicleNetwork>(out network);
    }

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEngine.Color defaultColor = new(1, 0, 0, 0.5f); // Red
        UnityEngine.Color firstColor = new(0, 1, 0, 0.5f); // Green

        if (road == null)
            return;

        for (int i = 0; i < GetMarkers().Count; i++)
        {
            if (i == firstMarker)
                Gizmos.color = firstColor;
            else
                Gizmos.color = defaultColor;

            Gizmos.DrawSphere(GetPoint(i), GetRadius());
        }
    }
#endif
    #endregion

    #region ComparePlayersPlaces

    public int ComparePlayersPlaces(ArcadeVehicleNetwork first, ArcadeVehicleNetwork second)
    {
        if (!players.TryGetValue(first.netId, out PlayerScore firstScore))
            return -1;

        if (!players.TryGetValue(second.netId, out PlayerScore secondScore))
            return 1;

        if (firstScore.round == secondScore.round)
        {
            if (firstScore.marker == secondScore.marker)
            {
                return 0;
            }
            else if (firstScore.marker < firstMarker && secondScore.marker > firstMarker)
            {
                return 1;
            }
            else if (firstScore.marker > firstMarker && secondScore.marker < firstMarker)
            {
                return -1;
            }
            else
            {
                return firstScore.marker - secondScore.marker;
            }
        }

        return firstScore.round - secondScore.round;
    }

    #endregion

    #region PlayerScore

    public class PlayerScore
    {
        public int marker;
        public int round;

        public readonly List<double> roundsTime;

        public double LastTime => roundsTime[round];
        public double LastBetweenTime => roundsTime.Count > 1 ? roundsTime[round] - roundsTime[round - 1] : roundsTime[round];

        public PlayerScore()
        {
            roundsTime = new();
            roundsTime.Add(0d);
        }


        public void SetMarker(int marker) => this.marker = marker;
        public void SetRound(int round) => this.round = round;

        public void AddRound(double time)
        {
            round++;
            roundsTime.Add(time);
        }
    }

    #endregion
}
