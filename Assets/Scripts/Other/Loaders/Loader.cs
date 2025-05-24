using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.SceneManagement
{
    public abstract class Loader : MonoBehaviour
    {
        [SerializeField] private bool allowAutoStart = true;
        [Scene]
        [SerializeField] private string loadingScene;

        public static Loader Instance { get; private set; }

        protected static AsyncOperation operation;


        protected virtual void Awake()
        {
            Instance = this;
        }

        protected abstract UniTask ShowCurrentScene(CancellationToken token = default);
        protected abstract UniTask HideCurrentScene(CancellationToken token = default);
        protected abstract UniTask AutoStart(CancellationToken token = default);



        public static async UniTask LoadSceneAsync(string scene, bool skipLoadingScene = false)
        {
            // Now, we use old Instance
            if (Instance != null)
                await Instance.HideCurrentScene();

            if (!skipLoadingScene)
            {
                await SceneManager.LoadSceneAsync(Instance.loadingScene);

                // After loading, we use new Instance
                await Instance.ShowCurrentScene();
                operation = SceneManager.LoadSceneAsync(scene);
                operation.allowSceneActivation = false;

                while (operation.progress < 0.9f)
                {
                    await UniTask.NextFrame();
                }

                if (!Instance.allowAutoStart)
                {
                    await Instance.AutoStart();
                }

                await Instance.HideCurrentScene();
                operation.allowSceneActivation = true;
            }
            else
            {
                operation = SceneManager.LoadSceneAsync(scene);
            }

            await operation;
            if (Instance != null)
                Instance.ShowCurrentScene().Forget();
        }

        public static async UniTask LoadSceneAsync(int sceneIndex, bool skipLoadingScene = false)
        {
            string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            await LoadSceneAsync(sceneName, skipLoadingScene);
        }

        // For buttons
        public void LoadScene(string sceneName, bool skipLoadingScene = false)
        {
            LoadSceneAsync(sceneName, skipLoadingScene).Forget();
        }

        public void LoadScene(int sceneIndex, bool skipLoadingScene = false)
        {
            string sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            LoadSceneAsync(sceneName, skipLoadingScene).Forget();
        }
    }
}