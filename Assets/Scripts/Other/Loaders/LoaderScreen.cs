using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.SceneManagement
{
    public class LoaderScreen : LoaderScene
    {
        [Space]
        [SerializeField] private Image progressBar;

        private bool autoStarted;


        public void SetAutoStart()
        {
            autoStarted = true;
        }

        protected virtual void Update()
        {
            if (progressBar == null)
                return;

            float progress = 0f;

            if (operation != null)
            {
                progress = Mathf.InverseLerp(0f, 0.9f, operation.progress);
            }

            progressBar.fillAmount = progress;
        }

        protected override async UniTask AutoStart(CancellationToken token = default)
        {
            while (!autoStarted)
            {
                await UniTask.NextFrame();
            }
        }
    }
}