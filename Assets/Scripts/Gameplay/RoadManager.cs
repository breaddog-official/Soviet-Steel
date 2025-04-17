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
using System.Linq;

public class RoadManager : NetworkBehaviour
{
    [SerializeField] private ERModularRoad road;
    [MinValue(0)]
    [SerializeField] private int firstMarker;
    [SerializeField] private float widthOffset;

    private readonly SyncDictionary<uint, PlayerScore> players = new();
    private readonly List<uint> places = new();

    [SerializeField, HideInInspector]
    private float[] distances;

    [ShowNativeProperty]
    private int DistancesCount => distances.Length;
    [ShowNativeProperty]
    private float Distance => distances.Sum();

    /// <summary>
    /// Server only event
    /// </summary>
    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedMarker;
    /// <summary>
    /// Server only event
    /// </summary>
    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedRound;


    private void Awake()
    {
        if (distances == null || DistancesCount < GetMarkers().Count)
            CacheDistances();
    }

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

    private void SyncPlaceAdd(uint id)
    {
        places.Add(id);
    }

    private void SyncPlaceRemove(uint id, PlayerScore score)
    {
        places.Remove(id);
    }

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
        places.Add(id);
    }

    [Server]
    public void RemovePlayer(GameObject player)
    {
        var network = player.GetComponent<ArcadeVehicleNetwork>();
        var id = network.netId;

        players.Remove(id);
        places.Remove(id);
    }



    public void UpdatePlayers()
    {
        // Only server code
        if (NetworkServer.active)
        {
            foreach (var player in players)
            {
                if (!player.Key.TryFindNetworkByID(out ArcadeVehicleNetwork network))
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
        print($"sort, places count: {places.Count}");
        places.Sort((f, s) => ComparePlayersPlaces(s, f));
    }

    [Button]
    public void CacheDistances()
    {
        var markers = GetMarkers();

        if (distances == null || DistancesCount < markers.Count)
            distances = new float[markers.Count];

        Vector3 lastPoint = markers[markers.Count -1];
        for (int i = 0; i < markers.Count; i++)
        {
            distances[i] = Vector3.Distance(lastPoint, markers[i]);
            lastPoint = markers[i];
        }
    }

    // Places
    public int GetPlace(uint player) => places.IndexOf(player);
    public IList<uint> GetPlaces() => places;

    // Markers
    public Vector3 GetPoint(int index) => GetMarkers()[index];
    public Vector3 GetNextPoint(int index) => GetPoint(index.IncreaseInBoundsReturn(GetMarkers().Count));
    public int GetNextPointIndex(int index) => index.IncreaseInBoundsReturn(GetMarkers().Count);
    public IReadOnlyList<Vector3> GetMarkers() => road.splinePoints;
    public float GetRadius() => (road.GetRoadWidth() / 2f) + widthOffset;

    // Players
    public IReadOnlyDictionary<uint, PlayerScore> GetPlayers() => players;
    public PlayerScore GetPlayer(uint id) => players[id];

    // Distances
    public float GetDistanceBeetweenMarkers(int firstIndex, int secondIndex)
    {
        float distance = 0;

        for (int i = firstIndex; i < secondIndex; i.IncreaseInBounds(distances))
        {
            distance += distances[i];
        }

        return distance;
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

    public int ComparePlayersPlaces(uint first, uint second)
    {
        if (!players.TryGetValue(first, out PlayerScore firstScore))
        {
            print($"{first} not founded");
            return -1;
        }

        if (!players.TryGetValue(second, out PlayerScore secondScore))
        {
            print($"{second} not founded");
            return 1;
        }

        if (firstScore.round == secondScore.round)
        {
            if (firstScore.marker == secondScore.marker && first.TryFindNetworkByID(out var firstNetwork) && second.TryFindNetworkByID(out var secondNetwork))
            {
                var firstDistance = Vector3.Distance(firstNetwork.transform.position, GetNextPoint(firstScore.marker));
                var secondDistance = Vector3.Distance(secondNetwork.transform.position, GetNextPoint(secondScore.marker));
                if (firstDistance == secondDistance)
                    return 0;
                else if (firstDistance < secondDistance)
                    return 1;
                else if (firstDistance > secondDistance)
                    return -1;
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
