using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System.Threading;
using TMPro;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.UI
{
    [ExecuteInEditMode]
    public class NumberTextUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text valueText;
        [Space]
        [SerializeField] private bool addDigits;
        [ShowIf(nameof(addDigits)), MinValue(0)]
        [SerializeField] private int digits;
        [Space]
        [SerializeField] private bool animate;
        [ShowIf(nameof(animate))]
        [SerializeField] private AudioSource audioSource;
        [ShowIf(nameof(animate))]
        [SerializeField] private float duration = 0.5f;
        [ShowIf(nameof(animate))]
        [SerializeField] private Vector3 scale = Vector3.one;

        public virtual string FormatString => addDigits ? $"0.{GetZeros()}" : string.Empty;

        private CancellationTokenSource source;
        private Vector3 cachedScale;
        private bool isAnimating;

        // Ётот ужас ввиде копипасты тут потому, что у object'а нету перегрузок ToString с более чем 0 аргументов и € не смогу сделать это через 
        // метод с аргументом object value

        private void Start()
        {
            cachedScale = valueText.transform.localScale;
        }

        private void OnDisable()
        {
            source?.Cancel();
            isAnimating = false;
            valueText.transform.localScale = cachedScale;
        }

        public virtual void UpdateValue(uint value)
        {
            valueText.text = value.ToString(FormatString);
            Animate().Forget();
        }

        public virtual void UpdateValue(int value)
        {
            valueText.text = value.ToString(FormatString);
            Animate().Forget();
        }

        public virtual void UpdateValue(float value)
        {
            valueText.text = value.ToString(FormatString);
            Animate().Forget();
        }

        public virtual void UpdateValue(double value)
        {
            valueText.text = value.ToString(FormatString);
            Animate().Forget();
        }


        private async UniTask Animate()
        {
            if (!animate || isAnimating)
                return;

            source?.ResetToken();
            source = new();

            isAnimating = true;

            if (audioSource != null)
                audioSource.Play();

            await valueText.transform.DOScale(scale, duration / 2).WithCancellation(source.Token);
            await valueText.transform.DOScale(cachedScale, duration / 2).WithCancellation(source.Token);

            isAnimating = false;
        }


        // Ќе нашЄл более элегантного решени€
        public virtual string GetZeros()
        {
            string zeros = string.Empty;

            for (int i = 0; i < digits; i++)
            {
                zeros += "0";
            }

            return zeros;
        }
    }
}
