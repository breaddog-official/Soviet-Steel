using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverShadowsSources : SettingHandler<ShadowsSources>
    {
        [Space]
        [SerializeField] protected Light[] lights;

        public override void UpdateValue()
        {
            var setting = Setting;

            foreach (var light in lights)
            {
                light.shadows = setting switch
                {
                    ShadowsSources.DirectionalOnly => LightShadows.None,
                    ShadowsSources.All => LightShadows.Soft,
                    _ => LightShadows.None
                };
            }
        }
    }
}
