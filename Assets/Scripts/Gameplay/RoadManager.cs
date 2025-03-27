using UnityEngine;
using EasyRoads3Dv3;
using NaughtyAttributes;
using Scripts.Extensions;
using System;
using Mirror;
using ArcadeVP;

public class RoadManager : NetworkBehaviour
{
    [SerializeField] private ERModularRoad road;
    [MinValue(0)]
    [SerializeField] private int firstMarker;
    [SerializeField] private float widthOffset;

    private readonly SyncDictionary<ArcadeVehicleNetwork, PlayerScore> players = new();

    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedMarker;
    public event Action<ArcadeVehicleNetwork, int> OnPlayerReachedRound;


    [Server]
    public void AddPlayer(GameObject player)
    {
        players.Add(player.GetComponent<ArcadeVehicleNetwork>(), new PlayerScore
        {
            marker = firstMarker
        });
    }

    [Server]
    public void RemovePlayer(GameObject player)
    {
        players.Remove(player.GetComponent<ArcadeVehicleNetwork>());
    }




    public void UpdatePlayers()
    {
        foreach (var player in players)
        {
            int nextPoint = player.Value.marker.IncreaseInBoundsReturn(road.markersExt.Count);

            if (Vector3.Distance(player.Key.transform.position, GetPoint(nextPoint)) < GetRadius())
            {
                player.Value.SetMarker(nextPoint);
                OnPlayerReachedMarker?.Invoke(player.Key, player.Value.marker);

                if (nextPoint == firstMarker)
                {
                    player.Value.SetRound(player.Value.round + 1);
                    OnPlayerReachedRound?.Invoke(player.Key, player.Value.round);
                }

                print($"Marker: {player.Value.marker}    Round: {player.Value.round}");
            }

        }
    }



    public Vector3 GetPoint(int index) => road.markersExt[index].position;
    public float GetRadius() => (road.GetRoadWidth() / 2f) + widthOffset;

    #region Gizmos
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color defaultColor = new(1, 0, 0, 0.5f); // Red
        Color firstColor = new(0, 1, 0, 0.5f); // Green
        

        for (int i = 0; i < road.markersExt.Count; i++)
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

    public class PlayerScore
    {
        public int marker;
        public int round;

        public void SetMarker(int marker) => this.marker = marker;
        public void SetRound(int round) => this.round = round;
    }
}
