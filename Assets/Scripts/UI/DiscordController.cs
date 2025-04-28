using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay;
using Scripts.TranslateManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
#if (UNITY_STANDALONE_WIN && UNITY_64) || UNITY_STANDALONE_OSX || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
    public const long APPLICATION_ID = 1349843959899361310;

    private static Discord.Discord discord;

    private static bool isError;



    private void Awake()
    {
        gameObject.DontDestroyOnLoad();

        Initialize();
    }

    private void Update()
    {
        RunCallbacks();
    }

    private void LateUpdate()
    {
        UpdateStatus();
    }

    private void OnApplicationQuit()
    {
        Dispose();
    }






    public static void Initialize()
    {
        discord ??= new Discord.Discord(APPLICATION_ID, (long)Discord.CreateFlags.NoRequireDiscord);
        isError = false;

        var activityManager = discord.GetActivityManager();
        activityManager.ClearActivity((res) => { });

        UpdateStatus();
    }

    public static void RunCallbacks()
    {
        if (isError) return;

        try
        {
            discord.RunCallbacks();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(exception);

            isError = true;
        }
    }

    public static void Dispose()
    {
        if (discord == null)
            return;

        if (!isError)
        {
            var activityManager = discord.GetActivityManager();
            activityManager.ClearActivity((res) => { });
        }

        discord.Dispose();
        discord = null;
    }

    public static void UpdateStatus()
    {
        if (isError) return;

        try
        {
            var map = GameManager.GameMode.Map;
            string mapImage = string.Empty;
            string mapName = string.Empty;
            string mapDescription = string.Empty;

            var car = GameManager.Car;
            string carImage = string.Empty;
            string carName = string.Empty;
            string carDescription = string.Empty;

            bool isMenu = SceneManager.GetActiveScene().name == "Menu";

            if (map != null && !isMenu)
            {
                mapImage = map.Icon.name.ToLower();
                mapName = TranslateManager.GetTranslationString(map.TranslateName);
                //mapDescription = TranslateManager.GetTranslationString(map.TranslateDescription);
            }
            else if (isMenu)
            {
                mapImage = "icon";
                mapName = TranslateManager.GetTranslationString("menu_levels_menu");
            }

            if (car != null && !isMenu)
            {
                carImage = car.icon.name.ToLower();
                carName = TranslateManager.GetTranslationString(car.translateName);
                //carDescription = TranslateManager.GetTranslationString(car.translateDescription);
            }

            string state = string.Empty;
            int players = 0;
            int maxPlayers = 0;

            if (Application.isEditor)
            {
                state = TranslateManager.GetTranslationString("discord_developing");
            }
            else if (NetworkServer.active)
            {
                state = TranslateManager.GetTranslationString("discord_host");
                players = NetworkServer.connections.Count;
                maxPlayers = NetworkServer.maxConnections;
            } 
            else if (NetworkClient.active)
            {
                state = $"{TranslateManager.GetTranslationString("discord_client")} {GameManager.response.name}";
                players = GameManager.response.playersCount;
                players = GameManager.response.maxPlayers;
            }

            /*long matchTime = 0L;
            if (GameManager.Instance != null)
            {
                matchTime = TimeSpan.FromSeconds(Time.timeAsDouble - GameManager.Instance.MatchTime).Seconds;
                print(matchTime);
            }*/

            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                State = state,
                Details = mapName,
                /*Timestamps =
                {
                    Start = matchTime,
                },*/
                Party =
                {
                    Privacy = Discord.ActivityPartyPrivacy.Private,
                    Size =
                    {
                        CurrentSize = players,
                        MaxSize = maxPlayers,
                    }
                },
                Assets =
                {
                    LargeImage = mapImage,
                    LargeText = mapDescription,

                    SmallImage = carImage,
                    SmallText = carName,
                },
            };

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Discord.Result.Ok)
                {
                    Debug.LogWarning($"Discord connection failed: {res}");
                }
            });
        }
        catch (Exception exception)
        {
            Debug.LogWarning(exception);

            isError = true;
        }
    }
#endif
}
