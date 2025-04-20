using Scripts.Extensions;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "MusicReducer", menuName = "Scripts/MusicReducer")]
public class MusicReducer : ScriptableObject
#if UNITY_EDITOR
                                    //, IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
{
    public int callbackOrder => 0;

    public string defaultPath;
    public string excludePath;
    [Space]
    public string lowQualityPostfix = ".LQ";
    public AudioClip[] requiredMusic;
    public MusicOverride[] overrides;

    [Serializable]
    public struct MusicOverride
    {
        public RuntimePlatformFlags platforms;
        public bool onlyRequiredMusic;
        public bool lowQuality;
    }

#if UNITY_EDITOR

    /*private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(defaultPath))
            defaultPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
    }

    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

        AssetDatabase.MoveAsset(defaultPath, excludePath);
        AssetDatabase.Refresh();
    }

    public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));

        AssetDatabase.MoveAsset(excludePath, defaultPath);
        AssetDatabase.Refresh();
    }*/
#endif
}
