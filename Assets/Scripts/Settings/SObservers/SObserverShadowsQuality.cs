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
        [Space]
        [SerializeField] protected bool switchToMultiScale;
        [ShowIf(nameof(switchToMultiScale))]
        [SerializeField] protected ShadowsQuality switchToMultiScaleQuality = ShadowsQuality.High;

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
                ShadowsQuality.Low => 1,
                ShadowsQuality.Medium => 1,
                ShadowsQuality.High => 2,
                ShadowsQuality.Ultra => 3,
                _ => 1
            };

            var ambientOcclusion = Profile.GetSetting<AmbientOcclusion>();

            ambientOcclusion.quality.Override(Setting switch
            {
                ShadowsQuality.Low => AmbientOcclusionQuality.Low,
                ShadowsQuality.Medium => AmbientOcclusionQuality.Medium,
                ShadowsQuality.High => AmbientOcclusionQuality.High,
                ShadowsQuality.Ultra => AmbientOcclusionQuality.Ultra,
                _ => throw new NotImplementedException()
            });

            if (switchToMultiScale && Setting == switchToMultiScaleQuality)
                ambientOcclusion.mode.Override(AmbientOcclusionMode.MultiScaleVolumetricObscurance);
            else
                ambientOcclusion.mode.Override(AmbientOcclusionMode.ScalableAmbientObscurance);
        }
    }
}
