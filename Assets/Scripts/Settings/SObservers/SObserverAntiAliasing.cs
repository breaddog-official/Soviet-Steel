using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverAntiAliasing : SettingHandler<AntiAliasing>
    {
        [Space]
        [SerializeField] protected bool isGlobal;
        [HideIf(nameof(isGlobal))]
        [SerializeField] protected PostProcessLayer layer;
        [ShowIf(nameof(isGlobal))]
        [SerializeField] protected SObserverAntiAliasingMSAA antiAliasingMsaa;


        public override void UpdateValue()
        {
            if (isGlobal)
            {
                QualitySettings.antiAliasing = Setting == AntiAliasing.MSAA ? 1 : 0;

                if (antiAliasingMsaa != null)
                    antiAliasingMsaa.UpdateValue();
            }
            else
            {
                layer.antialiasingMode = Setting switch
                {
                    AntiAliasing.FXAA => PostProcessLayer.Antialiasing.FastApproximateAntialiasing,
                    AntiAliasing.SMAA => PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing,
                    AntiAliasing.TAA => PostProcessLayer.Antialiasing.TemporalAntialiasing,
                    _ => PostProcessLayer.Antialiasing.None
                };
            }
        }
    }
}
