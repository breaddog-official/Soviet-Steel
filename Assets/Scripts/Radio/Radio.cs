using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using Scripts.Extensions;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Radio : MonoBehaviour
{
    [SerializeField] private string musicPath = "/Resources";
    [SerializeField] private string defaultSong;
    [Space]
    [SerializeField] private bool playOnAwake;
    [SerializeField] private AudioSource audioSource;

    private const string resourcesMusicFolder = "Radio";
    private readonly List<string> musicPaths = new();

    private int currentSongIndex;
    public bool IsPaused { get; private set; }
    public bool IsLoaded { get; private set; }

    private CancellationTokenSource cancellationToken;


    private void Awake()
    {
        musicPaths.AddRange(GetMusicPaths());
        currentSongIndex = musicPaths.IndexOf(defaultSong);

        if (playOnAwake)
            Play().Forget();
    }

    private void OnEnable()
    {
        Application.lowMemory += ClearMemory;
    }

    private void OnDisable()
    {
        Application.lowMemory -= ClearMemory;
    }

    private void OnDestroy()
    {
        cancellationToken?.Cancel();
    }



    public async UniTask Play()
    {
        if (!IsLoaded)
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            audioSource.resource = await LoadSong(musicPaths[currentSongIndex], cancellationToken.Token);
            IsLoaded = true;
        }

        audioSource.Play();
        IsPaused = false;
    }

    public void Pause()
    {
        if (IsPaused)
            audioSource.UnPause();
        else
            audioSource.Pause();

        IsPaused = !IsPaused;
    }


    public async UniTask Previous()
    {
        currentSongIndex.DecreaseInBounds(musicPaths);
        IsLoaded = false;

        await Play();
    }

    public async UniTask Next()
    {
        currentSongIndex.IncreaseInBounds(musicPaths);
        IsLoaded = false;

        await Play();
    }



    public string GetCurrentSong() => musicPaths[currentSongIndex];




    private async UniTask<AudioClip> LoadSong(string path, CancellationToken token = default)
    {
        var clip = await Resources.LoadAsync<AudioClip>($"{resourcesMusicFolder}/{path}").WithCancellation(token);
        return clip as AudioClip;
    }

    private async void ClearMemory()
    {
        await Resources.UnloadUnusedAssets();
    }

    #region GetMusicPaths

    public string[] GetMusicPaths()
    {
        //Load as TextAsset
        TextAsset fileNamesAsset = Resources.Load<TextAsset>($"{resourcesMusicFolder}/MusicFileNames");
        //De-serialize it
        FileNameInfo fileInfoLoaded = JsonUtility.FromJson<FileNameInfo>(fileNamesAsset.text);

        return fileInfoLoaded.fileNames;
    }

    [Serializable]
    class FileNameInfo
    {
        public string[] fileNames;

        public FileNameInfo(string[] fileNames)
        {
            this.fileNames = fileNames;
        }
    }

#if UNITY_EDITOR
    [Button]
    public void CachePaths()
    {
        //The Resources folder path
        string resourcsPath = Application.dataPath + musicPath;
        string fileName = "MusicFileNames";

        //Get file names except the ".meta" extension
        string[] fileNames = Directory.GetFiles(resourcsPath)
            .Where(x => Path.GetExtension(x) != ".meta" && Path.GetFileNameWithoutExtension(x) != fileName).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();

        //Convert the Names to Json to make it easier to access when reading it
        FileNameInfo fileInfo = new FileNameInfo(fileNames);
        string fileInfoJson = JsonUtility.ToJson(fileInfo);

        //Save the json to the Resources folder as "MusicFileNames.nahuy"
        File.WriteAllText(resourcsPath + $"/{fileName}.txt", fileInfoJson);

        AssetDatabase.Refresh();
    }
#endif
    #endregion
}


