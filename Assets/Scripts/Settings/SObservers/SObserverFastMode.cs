using UnityEngine;
using Scripts.Extensions;

namespace Scripts.Settings
{
    public class SObserverFastMode : SettingHandler<bool>
    {
        [Space]
        [SerializeField] protected Camera fastModeCamera;
        [Space]
        [SerializeField] protected RenderingPath defaultPath = RenderingPath.DeferredShading;
        [SerializeField] protected RenderingPath fastModePath = RenderingPath.Forward;


        public override void UpdateValue()
        {
            fastModeCamera.renderingPath = Setting ? fastModePath : defaultPath;
            ApplicationInfo.renderPath = fastModeCamera.renderingPath;
        }
    }
}
