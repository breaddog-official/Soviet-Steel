using NaughtyAttributes;
using Scripts.Extensions;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverShadowsEnabled : SettingHandler<ShadowsType>
    {
        [Space]
        [SerializeField] protected bool isGlobal;
        [ShowIf(nameof(isGlobal))]
        [SerializeField] protected PostProcessProfile profile;
        [HideIf(nameof(isGlobal))]
        [SerializeField] protected PostProcessVolume volume;

        protected PostProcessProfile Profile => isGlobal ? profile : volume.profile;


        public override void UpdateValue()
        {
            switch (Setting)
            {
                case ShadowsType.None:

                    SetAmbientOcclusion(false);
                    SetShadows(false);
                    break;

                //case ShadowsType.Simple:
                //
                //    SetAmbientOcclusion(true);
                //    SetShadows(false);
                //    break;

                case ShadowsType.Standart:

                    SetAmbientOcclusion(false);
                    SetShadows(true);
                    break;

                case ShadowsType.Advanced:

                    SetAmbientOcclusion(true);
                    SetShadows(true);
                    break;
            }
        }

        protected virtual void SetAmbientOcclusion(bool state)
        {
            Profile.GetSetting<AmbientOcclusion>().enabled.Override(state);
            ApplicationInfo.SetRenderPath(state ? SObserverFastMode.defaultPath : SObserverFastMode.fastModePath);
        }

        protected virtual void SetShadows(bool state)
        {
            QualitySettings.shadows = state ? ShadowQuality.HardOnly : ShadowQuality.Disable;
        }
    }
}
