using UnityEngine;

public class DiscordManager : MonoBehaviour
{
#if PLATFORM_STANDALONE
    public static DiscordManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            DiscordController.Initialize();
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        DiscordController.RunCallbacks();   
    }

    private void LateUpdate()
    {
        DiscordController.UpdateStatus();
    }

    private void OnApplicationQuit()
    {
        DiscordController.Dispose();
    }
#endif
}
