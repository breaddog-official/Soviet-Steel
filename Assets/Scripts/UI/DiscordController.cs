using Mirror;
using Scripts.Extensions;
using Scripts.Gameplay;
using System;
using UnityEngine;

public class DiscordController : MonoBehaviour
{
    #if (UNITY_STANDALONE_WIN && UNITY_64) || UNITY_STANDALONE_OSX
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

            if (map != null)
            {
                mapImage = map.Icon.name.ToLower();
                mapName = map.Name;
                //mapDescription = map.Description;
            }

            string state = string.Empty;
            int players = 0;
            int maxPlayers = 0;

            if (NetworkServer.active)
            {
                state = "Играет на своём сервере";
                players = NetworkServer.connections.Count;
                maxPlayers = NetworkServer.maxConnections;
            } 
            else if (NetworkClient.active)
            {
                state = $"Играет на сервере {GameManager.response.name}";
                players = GameManager.response.playersCount;
                players = GameManager.response.maxPlayers;
            }

            long matchTime = 0L;
            if (GameManager.Instance != null)
            {
                matchTime = (long)(DateTime.Now - new DateTime(0, 0, 0, 0, 0, (int)GameManager.Instance.MatchTime)).TotalSeconds;
                print(matchTime);
            }

            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                State = state,
                Details = mapName,
                Timestamps =
                {
                    Start = matchTime,
                },
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
