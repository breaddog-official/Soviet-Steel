#if PLATFORM_STANDALONE
using Discord;
using Scripts.Extensions;
using Scripts.Gameplay;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
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
        discord ??= new Discord.Discord(APPLICATION_ID, (long)CreateFlags.NoRequireDiscord);
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
            var scene = SceneManager.GetActiveScene().name;

            var activityManager = discord.GetActivityManager();
            var activity = new Activity
            {
                //Details = score,
#if UNITY_EDITOR
                State = "Разрабатывает",
#else
                //State = operationName,
#endif
                Assets =
                {
                    LargeImage = scene switch
                    {
                        "Ural" => "ural_icon",
                        "Saratov" => "saratov_icon",
                        _ => "icon"
                    },

                    LargeText = scene,
                },
            };
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Result.Ok)
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
}
#endif