using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsInitializator : MonoBehaviour, IGameAnalyticsATTListener
{
    private void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            Initialize();
        }
    }

    public void GameAnalyticsATTListenerNotDetermined() => Initialize();
    public void GameAnalyticsATTListenerRestricted() => Initialize();
    public void GameAnalyticsATTListenerDenied() => Initialize();
    public void GameAnalyticsATTListenerAuthorized() => Initialize();

    private void Initialize()
    {
        print("Game Analytics Initialized");
        GameAnalytics.Initialize();
        GameAnalytics.EnableAdvertisingIdTracking(false);
    }
}
