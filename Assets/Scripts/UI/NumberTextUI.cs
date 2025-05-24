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
        public virtual TMP_Text Text => valueText;

        private CancellationTokenSource source;
        private Vector3 cachedScale;
        private bool isAnimating;

        // Ётот ужас ввиде копипасты тут потому, что у object'а нету перегрузок ToString с более чем 0 аргументов и € не смогу сделать это через 
        // метод с аргументом object value

        private void Start()
        {
            if (valueText != null || TryGetComponent(out valueText))
                cachedScale = valueText.transform.localScale;
        }

        private void OnDisable()
        {
            source?.Cancel();
            isAnimating = false;
            Text.transform.localScale = cachedScale;
        }

        public virtual void UpdateValue(uint value)
        {
            string newText = value.ToString(FormatString);

            if (!string.Equals(valueText.text, newText))
            {
                Animate().Forget();
                valueText.text = newText;
            }
        }

        public virtual void UpdateValue(int value)
        {
            string newText = value.ToString(FormatString);

            if (!string.Equals(valueText.text, newText))
            {
                Animate().Forget();
                valueText.text = newText;
            }
        }

        public virtual void UpdateValue(float value)
        {
            string newText = value.ToString(FormatString);

            if (!string.Equals(valueText.text, newText))
            {
                Animate().Forget();
                valueText.text = newText;
            }
        }

        public virtual void UpdateValue(double value)
        {
            string newText = value.ToString(FormatString);

            if (!string.Equals(valueText.text, newText))
            {
                Animate().Forget();
                valueText.text = newText;
            }
        }

        public virtual void UpdateValue(string value)
        {
            if (!string.Equals(valueText.text, value))
            {
                Animate().Forget();
                valueText.text = value;
            }
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

            await Text.transform.DOScale(scale, duration / 2).WithCancellation(source.Token);
            await Text.transform.DOScale(cachedScale, duration / 2).WithCancellation(source.Token);

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
