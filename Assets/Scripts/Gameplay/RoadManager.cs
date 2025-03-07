using UnityEngine;
using EasyRoads3Dv3;
using NaughtyAttributes;
using System.Collections.Generic;
using Scripts.Extensions;
using System;

public class RoadManager : MonoBehaviour
{
    [SerializeField] private ERModularRoad road;
    [MinValue(0)]
    [SerializeField] private int firstMarker;

    private readonly Dictionary<Transform, PlayerScore> players = new();

    public event Action<Transform, int> OnPlayerReachedRound;



    public void AddPlayer(GameObject player)
    {
        players.Add(player.transform, new PlayerScore
        {
            marker = firstMarker
        });
    }

    public void RemovePlayer(GameObject player)
    {
        players.Remove(player.transform);
    }




    public void UpdatePlayers()
    {
        foreach (var player in players)
        {
            int nextPoint = player.Value.marker.IncreaseInBoundsReturn(road.markersExt.Count);

            if (Vector3.Distance(player.Key.position, GetPoint(nextPoint)) < GetRadius())
            {
                players[player.Key].marker = nextPoint;

                if (nextPoint == firstMarker)
                {
                    ref int round = ref players[player.Key].round;

                    round++;
                    OnPlayerReachedRound?.Invoke(player.Key, round);
                }

                print($"Marker: {players[player.Key].marker}    Round: {players[player.Key].round}");
            }

        }
    }



    public Vector3 GetPoint(int index) => road.markersExt[index].position;
    public float GetRadius() => road.GetRoadWidth() / 2;

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
    }
}
