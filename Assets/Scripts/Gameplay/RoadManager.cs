using UnityEngine;
using EasyRoads3Dv3;
using NaughtyAttributes;
using Scripts.Extensions;
using System;
using Mirror;
using ArcadeVP;
using System.Collections.Generic;
using Scripts.Gameplay;
using System.Linq;

[ExecuteInEditMode]
public class RoadManager : NetworkBehaviour
{
    [SerializeField] private ERModularRoad road;
    [MinValue(0)]
    [SerializeField] private int firstMarker;
    [SerializeField] private float widthOffset;
    [Space]
    [SerializeField] private bool simpleMarkers;

    private readonly SyncDictionary<ArcadeVehicleNetwork, PlayerScore> players = new();
    private readonly List<ArcadeVehicleNetwork> places = new();

    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedMarker;
    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedRound;

    private Vector3[] simpleMarkersCache;



    private void Start()
    {
        if (road != null)
            simpleMarkersCache = road.markersExt.Select(m => m.position).ToArray();
        
        players.OnAdd += SyncPlaceAdd;
        players.OnRemove += SyncPlaceRemove;
    }

    private void SyncPlaceAdd(ArcadeVehicleNetwork network) => places.Add(network);
    private void SyncPlaceRemove(ArcadeVehicleNetwork network, PlayerScore score) => places.Remove(network);

    [Server]
    public void AddPlayer(GameObject player)
    {
        var network = player.GetComponent<ArcadeVehicleNetwork>();
        var score = new PlayerScore
        {
            marker = firstMarker
        };

        players.TryAdd(network, score);
        places.Add(network);
    }

    [Server]
    public void RemovePlayer(GameObject player)
    {
        var network = player.GetComponent<ArcadeVehicleNetwork>();

        players.Remove(network);
        places.Remove(network);
    }



    [ServerCallback]
    public void UpdatePlayers()
    {
        foreach (var player in players)
        {
            int nextPoint = player.Value.marker.IncreaseInBoundsReturn(GetMarkers().Count);

            if (Vector3.Distance(player.Key.transform.position, GetPoint(nextPoint)) < GetRadius())
            {
                player.Value.SetMarker(nextPoint);
                OnPlayerReachedMarker?.Invoke(player.Key, player.Value.marker);

                if (nextPoint == firstMarker)
                {
                    player.Value.AddRound(GameManager.Instance.MatchTime);
                    OnPlayerReachedRound?.Invoke(player.Key, player.Value.round);
                }

                //print($"Player: {player.Key.name} Marker: {player.Value.marker} Round: {player.Value.round}");
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
    public IReadOnlyList<Vector3> GetMarkers() => simpleMarkers ? simpleMarkersCache : road.splinePoints;
    public IReadOnlyDictionary<ArcadeVehicleNetwork, PlayerScore> GetPlayers() => players;

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEngine.Color defaultColor = new(1, 0, 0, 0.5f); // Red
        UnityEngine.Color firstColor = new(0, 1, 0, 0.5f); // Green
        

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
        var firstScore = players[first];
        var secondScore = players[second];

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
