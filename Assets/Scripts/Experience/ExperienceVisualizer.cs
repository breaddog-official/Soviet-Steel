using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts.UI;
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
        [SerializeField] private UnlockPopup unlockPopup;
        [Space]
        [SerializeField] private bool isSmooth;
        [ShowIf(nameof(isSmooth)), MinValue(1)]
        [SerializeField] private int speed = 1;
        [Space]
        [SerializeField] private AudioSource newLevelSource;

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
            UpdateExperience(true, true).Forget();
        }

        private void OnDisable()
        {
            ExperienceManager.OnExperienceChanged -= UpdateExperience;
            source?.Cancel();
        }


        private void UpdateExperience(uint old, uint cur) => UpdateExperience().Forget();

        private async UniTask UpdateExperience(bool forceImmediatly = false, bool silently = false)
        {
            slider.wholeNumbers = true;
            var experience = ExperienceManager.Experience;
            var multiplier = slider.value < experience ? 1 : -1;

            if (isSmooth && !forceImmediatly)
            {
                source?.ResetToken();
                source = new();

                while (slider.value != experience)
                {
                    if (slider.value == slider.maxValue && multiplier > 0)
                        SetLevel(currentLevel + 1, silently);
                    else if (slider.value == slider.minValue && multiplier < 0)
                        SetLevel(currentLevel - 1, silently);

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
                SetLevel(ExperienceManager.GetCurrentLevel(), silently);
                slider.value = experience;
            }
        }

        private void SetLevel(int level, bool silently = false)
        {
            if (newLevelSource != null && !silently)
                newLevelSource.Play();

            currentLevel = level;
            slider.minValue = ExperienceManager.levels[currentLevel - 1];
            slider.maxValue = ExperienceManager.levels[currentLevel];
            UpdateText();

            if (unlockPopup != null && !silently)
                unlockPopup.Show(level).Forget();
        }

        private void UpdateText()
        {
            if (levelText != null) levelText.text = currentLevel.ToString();
            if (minValueText != null) minValueText.text = slider.minValue.ToString();
            if (maxValueText != null) maxValueText.text = slider.maxValue.ToString();
        }
    }
}