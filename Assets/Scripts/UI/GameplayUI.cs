using System;
using ArcadeVP;
using Scripts.Gameplay;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    protected enum RoundsFormat
    {
        Rounds,
        MaxRounds,
        [InspectorName(@"Rounds \ MaxRounds")]
        RoundsAndMaxRounds
    }

    [SerializeField] protected TMP_Text roundsText;
    [SerializeField] protected RoundsFormat format = RoundsFormat.RoundsAndMaxRounds;
    [Space]
    [SerializeField] protected TMP_Text timeText;
    [SerializeField] protected string timeFormat = @"mm\:ss";


    protected void Start()
    {
        ApplyRounds(0);
    }

    protected void OnEnable() => GameManager.Instance.RoadManager.OnPlayerReachedRound += ReachRound;
    protected void OnDisable() => GameManager.Instance.RoadManager.OnPlayerReachedRound -= ReachRound;


    private void Update()
    {
        TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.MatchTime);
        timeText.text = time.ToString(timeFormat);
    }

    public void ReachRound(ArcadeVehicleNetwork player, int round)
    {
        if (player.isOwned)
        {
            ApplyRounds(round);
        }
    }

    public void ApplyRounds(int rounds)
    {
        roundsText.text = format switch
        {
            RoundsFormat.Rounds => rounds.ToString(),
            RoundsFormat.MaxRounds => GameManager.GameMode.rounds.ToString(),
            RoundsFormat.RoundsAndMaxRounds => $"{rounds} / {GameManager.GameMode.rounds}",
            _ => string.Empty
        };
    }
}
