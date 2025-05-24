 using Scripts.TranslateManagement;

namespace Scripts.Settings
{
    public class SObserverLanguage : SettingHandler<ApplicationLanguage>
    {
        public override void UpdateValue()
        {
#if YandexGamesPlatform_yg
            TranslateManager.ChangeLanguage(TranslateManager.GetSystemLanguage());
#else
            if (Setting == ApplicationLanguage.Unknown && TranslateManager.GameLanguage != ApplicationLanguage.Unknown)
            {
                SetSetting(TranslateManager.GameLanguage);
                return;
            }

            TranslateManager.ChangeLanguage(Setting);
#endif
        }
    }
}
