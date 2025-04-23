using GameAnalyticsSDK;
using UnityEngine;
using Scripts.Gameplay;
using ArcadeVP;
using UnityEditor;

public class GameAnalyticsEnter : MonoBehaviour
{
    private string Scene => GameManager.GameMode.Map.Scene.Replace("_", string.Empty);

    private void Start()
    {
        if (GameManager.GameMode != null && GameManager.GameMode.Map != null)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, Scene);
#if YandexGamesPlatform_yg
            YG.YG2.MetricaSend(Scene);
#endif
        }
    }

    public void ProcessLeave()
    {
        if (ArcadeVehicleNetwork.LocalPlayerNetwork != null)
        {
            var progressionStatus = ArcadeVehicleNetwork.LocalPlayerNetwork.IsWin ? GAProgressionStatus.Complete : GAProgressionStatus.Fail;
            GameAnalytics.NewProgressionEvent(progressionStatus, Scene);
        }
    }
}
