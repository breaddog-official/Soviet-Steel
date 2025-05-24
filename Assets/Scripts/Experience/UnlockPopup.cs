using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.Extensions;
using Scripts.TranslateManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UnlockPopup : MonoBehaviour
    {
        [Header("Links")]
        [SerializeField] private RawImage icon;
        [SerializeField] private Transform panel;
        [SerializeField] private CanvasGroup canvasGroup;
        [Header("Text")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [Header("Audio")]
        [SerializeField] private AudioSource showSource;
        [SerializeField] private AudioSource hideSource;
        [Header("Yandex Games")]
        [SerializeField] private int desktopLinkLevel;
        [SerializeField] private int reviewLevel;
        [Header("Animation")]
        [SerializeField] private float waitDuration = 3f;
        [SerializeField] private float showDuration = 1f;
        [SerializeField] private float hideDuration = 1f;
        [Space]
        [SerializeField] private Ease showEase = Ease.Linear;
        [SerializeField] private Ease hideEase = Ease.Linear;

        private CancellationTokenSource tokenSource;


        private void OnDisable()
        {
            Cancel();
        }

        public async UniTask Show(int level)
        {
            var cars = NetworkManagerExt.instance.registeredCars.Where(c => c.car.level == level);
            var maps = NetworkManagerExt.instance.registeredMaps.Where(c => c.map.Level == level);

            tokenSource?.ResetToken();
            tokenSource = new();

            foreach (var map in maps)
            {
                await Animate(TranslateManager.GetTranslationString(map.map.TranslateName), TranslateManager.GetTranslationString(map.map.TranslateDescription), map.map.Icon, level, tokenSource.Token);
            }

            foreach (var car in cars)
            {
                await Animate(TranslateManager.GetTranslationString(car.car.translateName), TranslateManager.GetTranslationString(car.car.translateDescription), car.car.icon, level, tokenSource.Token);
            }
        }

        private async UniTask Animate(string name, string description, Texture2D icon, int level, CancellationToken token = default)
        {
            if (this.icon != null)
                this.icon.texture = icon;

            if (nameText != null)
                nameText.text = name;

            if (descriptionText != null)
                descriptionText.text = description;


            panel.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;

            var groupTask = canvasGroup.DOFade(1f, showDuration).SetEase(showEase).WithCancellation(token);
            var scaleTask = panel.DOScale(Vector3.one, showDuration).SetEase(showEase).WithCancellation(token);

            if (showSource != null)
                showSource.Play();

            await UniTask.WhenAll(groupTask, scaleTask);
            await UniTask.Delay(waitDuration.ConvertSecondsToMiliseconds());

            groupTask = canvasGroup.DOFade(0f, hideDuration).SetEase(hideEase).WithCancellation(token);
            scaleTask = panel.DOScale(Vector3.zero, hideDuration).SetEase(hideEase).WithCancellation(token);

            if (hideSource != null)
                hideSource.Play();

            await UniTask.WhenAll(groupTask, scaleTask);

            panel.gameObject.SetActive(false);
            canvasGroup.blocksRaycasts = false;

#if YandexGamesPlatform_yg
            if (level == desktopLinkLevel && YG.YG2.gameLabelCanShow)
                YG.YG2.GameLabelShowDialog();

            if (level == reviewLevel && YG.YG2.reviewCanShow)
                YG.YG2.ReviewShow();
#endif
        }

        private void Cancel()
        {
            tokenSource?.ResetToken();
            tokenSource = new();

            panel.gameObject.SetActive(false);
            panel.localScale = Vector3.zero;

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
