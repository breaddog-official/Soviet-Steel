using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverFastMode : SettingHandler<bool>
    {
        [Space]
        [SerializeField] protected Camera fastModeCamera;


        public override void UpdateValue()
        {
            fastModeCamera.renderingPath = Setting ? RenderingPath.Forward : RenderingPath.DeferredShading;
        }
    }
}
