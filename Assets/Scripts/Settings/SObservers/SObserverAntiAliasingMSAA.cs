using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverAntiAliasingMSAA : SettingHandler<AntiAliasingMSAA>
    {
        public override void UpdateValue()
        {
            if (QualitySettings.antiAliasing > 0)
            {
                QualitySettings.antiAliasing = (int)Setting;
            }
        }
    }
}
