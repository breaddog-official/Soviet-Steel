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
        //[ShowIf(nameof(isSmooth))]
        //[SerializeField] private float duration = 1f;
        //[ShowIf(nameof(isSmooth))]
        //[SerializeField] private Ease ease = Ease.OutSine;
        [ShowIf(nameof(isSmooth)), MinValue(1)]
        [SerializeField] private int speed = 1;

        private CancellationTokenSource source;
        private Slider slider;

        private int currentLevel = 1;


        private void Awake()
        {
            slider = GetComponent<Slider>();
        }



        private void OnEnable()
        {
            ExperienceManager.OnExperienceChanged += UpdateExperience;
            UpdateExperience(true).Forget();
        }

        private void OnDisable()
        {
            ExperienceManager.OnExperienceChanged -= UpdateExperience;
            source?.Cancel();
        }


        private void UpdateExperience(uint old, uint cur) => UpdateExperience().Forget();

        private async UniTask UpdateExperience(bool forceImmediatly = false)
        {
            slider.wholeNumbers = true;
            var experience = ExperienceManager.Experience;
            var multiplier = slider.value < experience ? 1 : -1;

            if (isSmooth && !forceImmediatly/* && experience != (uint)slider.value*/)
            {
                source?.ResetToken();
                source = new();

                //await slider.DOValue(experience, duration).SetEase(ease).WithCancellation(source.Token);

                while (slider.value != experience)
                {
                    if (slider.value == slider.maxValue)
                        SetLevel(currentLevel + 1);
                    else if (slider.value == slider.minValue)
                        SetLevel(currentLevel - 1);

                    if (multiplier > 0 && experience - slider.value < speed)
                        slider.value = experience;
                    else if (multiplier < 0 && slider.value - experience < speed)
                        slider.value = experience;
                    else
                        slider.value += speed * multiplier;

                    await UniTask.NextFrame(cancellationToken: source.Token);
                }
            }
            else
            {
                SetLevel(ExperienceManager.GetCurrentLevel());
                slider.value = experience;
            }
        }

        private void SetLevel(int level)
        {
            currentLevel = level;
            slider.minValue = ExperienceManager.levels[currentLevel - 1];
            slider.maxValue = ExperienceManager.levels[currentLevel];
            UpdateText();
        }

        private void UpdateText()
        {
            if (levelText != null) levelText.text = currentLevel.ToString();
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