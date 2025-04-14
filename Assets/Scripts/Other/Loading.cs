using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [Scene]
    [SerializeField] private string menuScene;

    private AsyncOperation operation;


    public void BeginLoad()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        operation = SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
    }

    public async void EndLoad()
    {
        operation.allowSceneActivation = true;

        await operation;
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //await loadOperation;
        //await unloadOperation;

        Application.backgroundLoadingPriority = ThreadPriority.Normal;
    }
}
