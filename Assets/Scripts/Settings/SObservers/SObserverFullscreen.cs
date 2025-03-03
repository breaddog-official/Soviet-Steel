using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverFullscreen : SettingHandler<FullScreenMode>
    {
        public override void UpdateValue()
        {
            Screen.fullScreenMode = Setting;
        }
    }
}
