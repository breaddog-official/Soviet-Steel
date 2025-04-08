using System;
using ArcadeVP;
using Scripts.Gameplay;
using TMPro;
using UnityEngine;
using Scripts.Extensions;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField] protected RoundsFormat roundsFormat = RoundsFormat.RoundsAndMaxRounds;
    [Space]
    [SerializeField] protected TMP_Text placeText;
    [SerializeField] protected RoundsFormat placeFormat = RoundsFormat.RoundsAndMaxRounds;
    [Space]
    [SerializeField] protected TMP_Text timeText;
    [SerializeField] protected string timeFormat = @"mm\:ss";
    [Space]
    [SerializeField] protected TMP_Text speedText;
    [SerializeField] protected Gradient speedGradient;
    [SerializeField] protected float speedMultiplier = 1f;
    [Space]
    [SerializeField] protected float disableRecordAfter = 3f;
    [SerializeField] protected TMP_Text recordText;
    [SerializeField] protected ArcadeVehicleNetwork recordNetwork;
    [SerializeField] protected string recordFormat = @"mm\:ss\.fff";


    protected void Start()
    {
        ApplyRounds(0);
        recordText.gameObject.SetActive(false);
    }

    protected void OnEnable() => GameManager.Instance.RoadManager.OnPlayerReachedRound += ReachRound;
    protected void OnDisable() => GameManager.Instance.RoadManager.OnPlayerReachedRound -= ReachRound;


    private void Update()
    {
        timeText.text = FormatTime(GameManager.Instance.MatchTime, timeFormat);

        speedText.text = ((int)(recordNetwork.vehicleController.Speed * speedMultiplier)).ToString();
        speedText.color = speedGradient.Evaluate(recordNetwork.vehicleController.Speed / recordNetwork.vehicleController.maxSpeed);

        ApplyPlace(GameManager.Instance.RoadManager.GetPlace(recordNetwork) + 1);
    }

    public void ReachRound(ArcadeVehicleNetwork player, int round)
    {
        if (player == recordNetwork)
        {
            ApplyRounds(round);
            CheckRecord();
        }
    }

    public void CheckRecord()
    {
        string key = $"{GameManager.GameMode.map.Name}_record";
        double time = GameManager.Instance.RoadManager.GetPlayers().GetValueOrDefault(recordNetwork).LastBetweenTime;

        // 120_000 seconds, ~33 hours
        if (PlayerPrefs.GetFloat(key, 120_000f) > time)
        {
            recordText.gameObject.SetActive(true);
            recordText.gameObject.DisableAfter(disableRecordAfter).Forget();

            recordText.text = FormatTime(time, recordFormat);

            PlayerPrefs.SetFloat(key, (float)time);
        }
    }

    private string FormatTime(double time, string format)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return timeSpan.ToString(format);
    }

    public void ApplyRounds(int rounds)
    {
        if (roundsText == null)
            return;

        int maxRounds = GameManager.GameMode.rounds;
        roundsText.text = roundsFormat switch
        {
            RoundsFormat.Rounds => rounds.ToString(),
            RoundsFormat.MaxRounds => maxRounds.ToString(),
            RoundsFormat.RoundsAndMaxRounds => $"{rounds} / {maxRounds}",
            _ => string.Empty
        };
    }

    public void ApplyPlace(int place)
    {
        if (placeText == null)
            return;

        int lastPlace = GameManager.Instance.RoadManager.GetPlayers().Count;
        placeText.text = placeFormat switch
        {
            RoundsFormat.Rounds => place.ToString(),
            RoundsFormat.MaxRounds => lastPlace.ToString(),
            RoundsFormat.RoundsAndMaxRounds => $"{place} / {lastPlace}",
            _ => string.Empty
        };
    }
}
