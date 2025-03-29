using System;
using ArcadeVP;
using Scripts.Gameplay;
using TMPro;
using UnityEngine;
using Scripts.Extensions;
using Cysharp.Threading.Tasks;

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
    [Space]
    [SerializeField] protected float disableRecordAfter = 3f;
    [SerializeField] protected TMP_Text recordText;
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
        timeText.text = FormatCurrentTime(timeFormat);
    }

    public void ReachRound(ArcadeVehicleNetwork player, int round)
    {
        if (player.isOwned)
        {
            ApplyRounds(round);
            CheckRecord();
        }
    }

    public void CheckRecord()
    {
        string key = $"{GameManager.GameMode.map.Name}_record";

        // 120_000 seconds, ~33 hours
        if (PlayerPrefs.GetFloat(key, 120_000f) > GameManager.Instance.MatchTime)
        {
            recordText.gameObject.SetActive(true);
            recordText.gameObject.DisableAfter(disableRecordAfter).Forget();

            recordText.text = FormatCurrentTime(recordFormat);

            PlayerPrefs.SetFloat(key, (float)GameManager.Instance.MatchTime);
        }
    }

    private string FormatCurrentTime(string format)
    {
        TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.MatchTime);
        return time.ToString(format);
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
