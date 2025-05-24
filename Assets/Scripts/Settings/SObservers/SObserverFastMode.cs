using UnityEngine;
using Scripts.Extensions;

namespace Scripts.Settings
{
    public class SObserverFastMode : SettingHandler<bool>
    {
        [Space]
        [SerializeField] protected Camera fastModeCamera;

        public const RenderingPath defaultPath = RenderingPath.DeferredShading;
        public const RenderingPath fastModePath = RenderingPath.Forward;


        protected override void OnEnable()
        {
            ApplicationInfo.OnRenderPathChanged += UpdateValue;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            ApplicationInfo.OnRenderPathChanged -= UpdateValue;
            base.OnDisable();
        }


        public override void UpdateValue()
        {
            fastModeCamera.renderingPath = ApplicationInfo.RenderPath;
            //fastModeCamera.renderingPath = Setting ? fastModePath : defaultPath;
            //ApplicationInfo.SetRenderPath(fastModeCamera.renderingPath);
        }
    }
}
