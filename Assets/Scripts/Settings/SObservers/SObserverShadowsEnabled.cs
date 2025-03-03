using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverShadowsEnabled : SettingHandler<bool>
    {
        [Space]
        [SerializeField] protected bool setCameraRenderPath;
        [ShowIf(nameof(setCameraRenderPath))]
        [SerializeField] protected Camera renderCamera;

        public override void UpdateValue()
        {
            QualitySettings.shadows = Setting ? ShadowQuality.HardOnly : ShadowQuality.Disable;

            if (setCameraRenderPath)
                renderCamera.renderingPath = Setting ? RenderingPath.DeferredShading : RenderingPath.Forward;
        }
    }
}
