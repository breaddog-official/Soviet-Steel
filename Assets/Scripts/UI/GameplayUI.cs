using System;
using ArcadeVP;
using Scripts.Gameplay;
using TMPro;
using UnityEngine;
using Scripts.Extensions;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Scripts.TranslateManagement;
using Scripts.UI;

public class GameplayUI : MonoBehaviour
{
    protected enum RoundsFormat
    {
        Rounds,
        MaxRounds,
        [InspectorName(@"Rounds \ MaxRounds")]
        RoundsAndMaxRounds
    }

    [SerializeField] protected ArcadeVehicleNetwork overrideNetwork;
    [Header("Rounds")]
    [SerializeField] protected TMP_Text roundsText;
    [SerializeField] protected RoundsFormat roundsFormat = RoundsFormat.RoundsAndMaxRounds;
    [Header("Place")]
    [SerializeField] protected bool winPlace;
    [SerializeField] protected TMP_Text placeText;
    [SerializeField] protected RoundsFormat placeFormat = RoundsFormat.RoundsAndMaxRounds;
    [Header("Time")]
    [SerializeField] protected TMP_Text timeText;
    [SerializeField] protected string timeFormat = @"mm\:ss";
    [Header("Last Time")]
    [SerializeField] protected bool winTime;
    [SerializeField] protected TMP_Text lastTimeText;
    [SerializeField] protected string lastTimeFormat = @"mm\:ss";
    [Header("Speed")]
    [SerializeField] protected TMP_Text speedText;
    [SerializeField] protected Gradient speedGradient;
    [SerializeField] protected float speedMultiplier = 1f;
    [Header("Record")]
    [SerializeField] protected float disableRecordAfter = 3f;
    [SerializeField] protected TMP_Text recordText;
    [SerializeField] protected string recordFormat = @"mm\:ss\.fff";
    [Header("Points")]
    [SerializeField] protected TextTranslater pointsText;
    [SerializeField] protected NumberTextUI pointsCounter;
    [SerializeField] protected PointsText[] pointsTexts;

    [Serializable]
    protected struct PointsText
    {
        public string translateString;
        public uint points;
        [ColorUsage(true, false)]
        public Color color;
    }

    protected ArcadeVehicleNetwork Network => overrideNetwork ?? ArcadeVehicleNetwork.LocalPlayerNetwork;


    protected void Awake()
    {
        ApplyRounds(0);

        if (recordText != null)
            recordText.gameObject.SetActive(false);
    }

    protected void OnEnable()
    {
        GameManager.Instance.RoadManager.OnPlayerReachedRound += ReachRound;
        SetRound(GameManager.Instance.RoadManager.GetPlayers()[Network.netId].round);
    }

    protected void OnDisable() => GameManager.Instance.RoadManager.OnPlayerReachedRound -= ReachRound;


    private void Update()
    {
        if (timeText != null)
            timeText.text = FormatTime(GameManager.Instance.MatchTime, timeFormat);

        if (speedText != null)
        {
            speedText.text = ((int)(Network.vehicleController.Speed * speedMultiplier)).ToString();
            speedText.color = speedGradient.Evaluate(Network.vehicleController.Speed / Network.vehicleController.maxSpeed);
        }

        ApplyPlace(winPlace ? GameManager.Instance.WinManager.GetPlace(Network.netId) + 1 : GameManager.Instance.RoadManager.GetPlace(Network.netId) + 1);
        ApplyPoints(Network.vehicleController.pointsDelta);
    }

    public void ReachRound(ArcadeVehicleNetwork player, int round)
    {
        if (player == Network)
        {
            CheckRecord();
            SetRound(round);
        }
    }

    public void SetRound(int round)
    {
        ApplyRounds(round);

        if (lastTimeText != null && GameManager.Instance.RoadManager.GetPlayers().TryGetValue(Network.netId, out var score))
        {
            if (winTime && round < GameManager.GameMode.rounds)
                return;

            var time = score.roundsTime[GameManager.GameMode.rounds];
            lastTimeText.text = FormatTime(time, lastTimeFormat);
        }
    }

    public void CheckRecord()
    {
        if (recordText == null)
            return;

        string key = $"{GameManager.GameMode.Map.TranslateName}_record";
        double time = GameManager.Instance.RoadManager.GetPlayers().GetValueOrDefault(Network.netId).LastBetweenTime;

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
            RoundsFormat.RoundsAndMaxRounds => $"{rounds}/{maxRounds}",
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
            RoundsFormat.RoundsAndMaxRounds => $"{place}/{lastPlace}",
            _ => string.Empty
        };
    }

    public void ApplyPoints(uint pointsDelta)
    {
        if (pointsText != null)
        {
            bool enabled = pointsTexts[0].points <= pointsDelta;
            pointsText.gameObject.SetActive(enabled);

            if (enabled)
            {
                PointsText? pointsTextStruct = null;
                for (int i = 0; i < pointsTexts.Length; i++)
                {
                    if (pointsTexts[i].points > pointsDelta)
                    {
                        pointsTextStruct = pointsTexts[i - 1];
                        break;
                    }
                    else if (i == pointsTexts.Length - 1)
                    {
                        pointsTextStruct = pointsTexts[i];
                        break;
                    }
                }

                if (pointsTextStruct.HasValue)
                {
                    pointsText.SetName(pointsTextStruct.Value.translateString);
                    pointsText.Text.color = pointsTextStruct.Value.color;

                    if (pointsCounter != null)
                    {
                        pointsCounter.UpdateValue($"+{pointsDelta}");
                        pointsCounter.Text.color = pointsTextStruct.Value.color;
                    }
                }
            }
        }
    }
}
