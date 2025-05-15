using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoading : MonoBehaviour
{
    [Scene]
    [SerializeField] private string scene;

    private AsyncOperation operation;


    public void BeginLoad()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
    }

    public async void EndLoad()
    {
        operation.allowSceneActivation = true;

        Application.backgroundLoadingPriority = ThreadPriority.High;

        await operation;
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //await loadOperation;
        //await unloadOperation;

        Application.backgroundLoadingPriority = ThreadPriority.Normal;
    }
}
