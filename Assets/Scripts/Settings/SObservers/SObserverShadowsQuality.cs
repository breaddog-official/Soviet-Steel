using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverShadowsQuality : SettingHandler<ShadowsQuality>
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
            QualitySettings.shadowResolution = Setting switch
            {
                ShadowsQuality.Low => ShadowResolution.Low,
                ShadowsQuality.Medium => ShadowResolution.Medium,
                ShadowsQuality.High => ShadowResolution.High,
                ShadowsQuality.Ultra => ShadowResolution.VeryHigh,
                _ => throw new NotImplementedException()
            };

            QualitySettings.shadowCascades = Setting switch
            {
                ShadowsQuality.Medium => 2,
                ShadowsQuality.High => 3,
                ShadowsQuality.Ultra => 4,
                _ => 1
            };

            Profile.GetSetting<AmbientOcclusion>().quality.Override(Setting switch
            {
                ShadowsQuality.Low => AmbientOcclusionQuality.Low,
                ShadowsQuality.Medium => AmbientOcclusionQuality.Medium,
                ShadowsQuality.High => AmbientOcclusionQuality.High,
                ShadowsQuality.Ultra => AmbientOcclusionQuality.Ultra,
                _ => throw new NotImplementedException()
            });
        }
    }
}
