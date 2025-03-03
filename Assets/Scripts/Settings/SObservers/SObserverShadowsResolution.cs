using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverShadowsResolution : SettingHandler<ShadowResolution>
    {
        public override void UpdateValue()
        {
            QualitySettings.shadowResolution = Setting;
        }
    }
}
