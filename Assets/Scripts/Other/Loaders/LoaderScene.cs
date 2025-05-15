using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.Audio;
using UnityEngine;

namespace Scripts.SceneManagement
{
    public class LoaderScene : Loader
    {
        [Space]
        [SerializeField] private CanvasGroup canvasGroup;
        [Space]
        [SerializeField] private Ease showEase = Ease.Linear;
        [SerializeField] private float showDuration = 1f;
        [Space]
        [SerializeField] private Ease hideEase = Ease.Linear;
        [SerializeField] private float hideDuration = 1f;

        private CancellationTokenSource source = new();


        protected override void Awake()
        {
            //if (canvasGroup != null)
            //    canvasGroup.alpha = 1f;

            base.Awake();
        }

        protected virtual void OnDestroy()
        {
            source?.Cancel();
        }


        protected override async UniTask ShowCurrentScene(CancellationToken token = default)
        {
            UniTask alphaTask = UniTask.CompletedTask;
            UniTask audioTask = UniTask.CompletedTask;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                alphaTask = canvasGroup.DOFade(0f, showDuration).SetEase(showEase).WithCancellation(source.Token);
            }

            audioTask = AudioManager.ShowMusics(token);

            await UniTask.WhenAll(alphaTask, audioTask);
        }

        protected override async UniTask HideCurrentScene(CancellationToken token = default)
        {
            UniTask alphaTask = UniTask.CompletedTask;
            UniTask audioTask = UniTask.CompletedTask;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                alphaTask = canvasGroup.DOFade(1f, hideDuration).SetEase(hideEase).WithCancellation(source.Token);
            }

            audioTask = AudioManager.HideMusics(token);

            await UniTask.WhenAll(alphaTask, audioTask);
        }


        protected override async UniTask AutoStart(CancellationToken token = default)
        {
            await UniTask.CompletedTask;
        }
    }
}