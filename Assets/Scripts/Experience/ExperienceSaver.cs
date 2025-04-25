using GameAnalyticsSDK;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.SaveManagement;
using UnityEngine;

namespace Scripts.Gameplay.Experience 
{
    public class ExperienceSaver : MonoBehaviour
    {
        [SerializeField] private Saver saver;
        [SerializeField] private string experienceKey = "Experience";
        [Space]
        [SerializeField] private bool sendMetrics;
        [ShowIf(nameof(sendMetrics))]
        [SerializeField] private string experienceMetrica = "Experience";
        [ShowIf(nameof(sendMetrics))]
        [SerializeField] private string levelMetrica = "Level";
        [Space]
        [SerializeField] private bool leaderboard;
        [ShowIf(nameof(leaderboard))]
        [SerializeField] private string leaderboardKey = "ExperienceLeaderboard";

        public static ExperienceSaver Instance { get; private set; }


        private void Awake()
        {
            if (Instance == null)
            {
                gameObject.DontDestroyOnLoad();
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }


            if (saver.Exists(experienceKey) && uint.TryParse(saver.Load(experienceKey), out uint experience))
                ExperienceManager.SetExperience(experience);
            else
                saver.Save(experienceKey, ExperienceManager.Experience.ToString());
        }


        private void OnEnable()
        {
            ExperienceManager.OnExperienceChanged += UpdateExperience;
        }

        private void OnDisable()
        {
            ExperienceManager.OnExperienceChanged -= UpdateExperience;
        }


        private void UpdateExperience(uint old, uint cur)
        {
            if (sendMetrics)
            {
                if (old < cur)
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, experienceMetrica, cur - old, experienceMetrica, experienceMetrica);
                else if (old > cur)
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, experienceMetrica, old - cur, experienceMetrica, experienceMetrica);

                int oldLevel = ExperienceManager.GetLevel(old);
                int curLevel = ExperienceManager.GetLevel(cur);

                if (oldLevel < curLevel)
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, levelMetrica, curLevel - oldLevel, levelMetrica, levelMetrica);
                else if (oldLevel > curLevel)
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, levelMetrica, oldLevel - curLevel, levelMetrica, levelMetrica);

#if YandexGamesPlatform_yg
                if (oldLevel < curLevel)
                    YG.YG2.MetricaSend(GetLevelKey(curLevel));

                if (leaderboard)
                    YG.YG2.SetLeaderboard(leaderboardKey, (int)cur);
#endif
            }

            saver.Save(experienceKey, cur.ToString());
        }

        private string GetLevelKey(int level) => $"level{level}";
    }
}