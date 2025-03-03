using UnityEngine;
using UnityEngine.Audio;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SunDirectionWithFog : SettingHandler<FogType>
    {
        [Space]
        [SerializeField] protected Vector3 disabledAngle;
        [SerializeField] protected Vector3 enabledAngle;


        public override void UpdateValue()
        {
            var angle = Setting == FogType.VolumetricFog ? enabledAngle : disabledAngle;

            transform.rotation = Quaternion.Euler(angle);
        }
    }
}
