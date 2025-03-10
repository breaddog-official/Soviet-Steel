#if PLATFORM_STANDALONE
using Discord;
using Scripts.TranslateManagement;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DiscordController
{
    public const long APPLICATION_ID = 1260536542363914423;

    private static Discord.Discord discord;

    private static bool isError;

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
            string map = "Menu";
            string operationName = "Menu";
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                map = MapsSheet.MapsSheetInstance[LevelsFabric.GetCurrentLevel().MapIndex].Name;
                operationName = SingleTranslater.GetFieldInformation(LevelsFabric.GetCurrentLevel().NameField);
            }

            string command = "";
            if (Player.Instance != null)
            {
                command = Player.Instance.Team switch
                {
                    TeamManager.Team.Terrorists => "Terrorists",
                    TeamManager.Team.CounterTerrorists => "Counter Terrorists",
                    _ => "",
                };
            }

            string score = "";
#if !UNITY_EDITOR
            if (GameManager.Instance != null)
            {
                score = $"Score: {GameManager.Instance.CT_Rounds} : {GameManager.Instance.T_Rounds}";
            }
#endif

            var activityManager = discord.GetActivityManager();
            var activity = new Activity
            {
                Details = score,
#if UNITY_EDITOR
                State = "Разрабатывает",
#else
                State = operationName,
#endif
                Assets =
                {
                    LargeImage = map switch
                    {
#if UNITY_EDITOR
                        _ => "developing_miniature"
#else
                        "Menu" => "ct_avatar",
                        "Dust2" => "dust2_miniature",
                        "Italy" => "italy_miniature",
                        "Office" => "office_miniature",
                        "Assault" => "assault_miniature",
                        _ => "",
#endif
                    },
                    SmallImage = command switch
                    {
#if !UNITY_EDITOR
                        "Terrorists" => "t_avatar",
                        "Counter Terrorists" => "ct_avatar",
#endif
                        _ => "",
                    },

                    LargeText = map,
                    SmallText = command,
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