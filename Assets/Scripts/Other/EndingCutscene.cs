using Aura2API;
using Cysharp.Threading.Tasks;
using Scripts.Audio;
using Scripts.Gameplay.Experience;
using Scripts.SaveManagement;
using Scripts.SceneManagement;
using Scripts.UI.Tabs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

[DefaultExecutionOrder(0)]
public class EndingCutscene : MonoBehaviour
{
    public PlayableDirector director;
    public Music music;
    public Saver saver;
    public CinemachineCamera cam;
    public TabsTranslater tabsTranslater;
    public AuraCamera auraCamera;
    [Range(0f, 1f)]
    public float extinction;

    private const string endingKey = "ending_showed";
#if YandexGamesPlatform_yg && !UNITY_EDITOR
    private const string creditsScene = "Menu";
#else
    private const string creditsScene = "Credits";
#endif
    private const int endLevel = 12;


    private void Start()
    {
        if (saver.Exists(endingKey) && bool.TryParse(saver.Load(endingKey), out bool result) && result == true)
            return;

        else if (ExperienceManager.GetCurrentLevel() >= endLevel)
        {
            StartCutscene();
        }
    }

    private void StartCutscene()
    {
        cam.enabled = true;
        tabsTranslater.HideTab();
        music.HideMusic().Forget();
        director.Play();
    }

    public void EndCutscene()
    {
        saver.Save(endingKey, true.ToString());
#if YandexGamesPlatform_yg
        if (YG.YG2.reviewCanShow)
            YG.YG2.ReviewShow();
#endif
        Loader.LoadSceneAsync(creditsScene, true).Forget();
    }

    private void Update()
    {
        if (auraCamera != null)
            auraCamera.frustumSettings.BaseSettings.extinction = extinction;
    }

    private void OnDisable()
    {
        auraCamera.frustumSettings.BaseSettings.extinction = 1f;
    }
}
