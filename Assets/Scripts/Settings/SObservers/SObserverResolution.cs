using System;
using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverResolution : SettingHandler<Settings.ResolutionType>
    {
        [Space]
        [SerializeField, Range(0, 100)] protected int lowResolution = 25;
        [SerializeField, Range(0, 100)] protected int mediumResolution = 50;
        [SerializeField, Range(0, 100)] protected int highResolution = 75;
        [SerializeField, Range(0, 100)] protected int fullResolution = 100;


        public override void UpdateValue()
        {
            var percents = Setting switch
            {
                Settings.ResolutionType.Low => lowResolution,
                Settings.ResolutionType.Medium => mediumResolution,
                Settings.ResolutionType.High => highResolution,
                Settings.ResolutionType.Full => fullResolution,
                _ => 100f
            };
            var multiplier = percents / 100f;

            QualitySettings.resolutionScalingFixedDPIFactor = multiplier;
            ScalableBufferManager.ResizeBuffers(multiplier, multiplier);
        }
    }
}
