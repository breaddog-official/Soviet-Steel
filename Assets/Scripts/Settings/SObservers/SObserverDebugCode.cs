using Scripts.Gameplay.Experience;
using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverDebugCode : SettingHandler<string>
    {
        [SerializeField] private string experienceCode;
        [SerializeField] private uint experience = 20000;


        public override void UpdateValue()
        {
            string code = string.Empty;
#if YandexGamesPlatform_yg && !UNITY_EDITOR
            code = YG.YG2.envir.payload;
            print(code);
#else
            code = Setting;
#endif
            if (string.Equals(code, experienceCode) && ExperienceManager.Experience < experience)
            {
                ExperienceManager.SetExperience(experience);
            }
        }
    }
}
