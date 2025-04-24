using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Extensions;
using TMPro;

namespace Scripts.Gameplay.Experience 
{
    public class ExperienceVisualizer : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text minValueText;
        [SerializeField] private TMP_Text maxValueText;
        [Space]
        [SerializeField] private bool isSmooth;
        [ShowIf(nameof(isSmooth))]
        [SerializeField] private float duration = 1f;
        [ShowIf(nameof(isSmooth))]
        [SerializeField] private Ease ease = Ease.OutSine;

        private CancellationTokenSource source;
        private Slider slider;


        private void Awake()
        {
            slider = GetComponent<Slider>();
        }



        private void OnEnable()
        {
            ExperienceManager.OnExperienceChanged += UpdateExperience;
            UpdateExperience(true);
        }

        private void OnDisable()
        {
            ExperienceManager.OnExperienceChanged -= UpdateExperience;
            source?.Cancel();
        }


        private void UpdateExperience(uint old, uint cur) => UpdateExperience();

        private async void UpdateExperience(bool forceImmediatly = false)
        {
            slider.wholeNumbers = true;
            var experience = ExperienceManager.experience;

            if (isSmooth && !forceImmediatly && experience != (uint)slider.value)
            {
                source?.ResetToken();
                source = new();

                await slider.DOValue(experience, duration).SetEase(ease).WithCancellation(source.Token);
            }
            else
            {
                slider.value = experience;
            }

            slider.minValue = ExperienceManager.levels[ExperienceManager.GetCurrentLevel() - 1];
            slider.maxValue = ExperienceManager.levels[ExperienceManager.GetNextLevel() - 1];
            UpdateText();
        }

        private void UpdateText()
        {
            if (levelText != null) levelText.text = ExperienceManager.GetCurrentLevel().ToString();
            if (minValueText != null) minValueText.text = slider.minValue.ToString();
            if (maxValueText != null) maxValueText.text = slider.maxValue.ToString();
        }

#if UNITY_EDITOR

        [Button]
        public void Encrease100Experience()
        {
            ExperienceManager.EncreaseExperience(100);
        }

        [Button]
        public void Decrease100Experience()
        {
            ExperienceManager.DecreaseExperience(100);
        }

#endif
    }
}