using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Animation animator;
    [SerializeField] private AudioSource audioSource;
    [Scene]
    [SerializeField] private string scene;

    //private AsyncOperation operation;


    private void Start()
    {
#if YandexGamesPlatform_yg
        YG.YG2.onGetSDKData += EndLoad; // Init sdk
        if (YG.YG2.isSDKEnabled)
            EndLoad();
#else
        animator.Play();
        audioSource.Play();
#endif
    }

    public void BeginLoad()
    {
        /*Application.backgroundLoadingPriority = ThreadPriority.Low;

        operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;*/
    }

    public async void EndLoad()
    {
        /*operation.allowSceneActivation = true;

        Application.backgroundLoadingPriority = ThreadPriority.High;

        await operation;
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //await loadOperation;
        //await unloadOperation;

        Application.backgroundLoadingPriority = ThreadPriority.Normal;*/
        await Loader.LoadSceneAsync(scene, true);

#if YandexGamesPlatform_yg
        YG.YG2.GameReadyAPI();
#endif
    }
}
