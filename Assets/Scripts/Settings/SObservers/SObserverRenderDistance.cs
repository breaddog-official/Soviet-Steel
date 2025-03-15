using Unity.Cinemachine;
using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverRenderDistance : SettingHandler<RenderDistance>
    {
        [SerializeField] protected Camera cinemachineCamera;
        [Space]
        [SerializeField] protected float lowDistance = 50;
        [SerializeField] protected float mediumDistance = 300;
        [SerializeField] protected float highDistance = 700;
        [SerializeField] protected float maxDistance = 1000;

        protected CinemachineBrain cinemachineBrain;


        protected override void Awake()
        {
            base.Awake();

            cinemachineBrain = cinemachineCamera.GetComponent<CinemachineBrain>();
        }

        public override void UpdateValue()
        {
            cinemachineCamera.farClipPlane = Setting switch
            {
                RenderDistance.Low => lowDistance,
                RenderDistance.Medium => mediumDistance,
                RenderDistance.High => highDistance,
                RenderDistance.Maximum => maxDistance,

                _ => cinemachineCamera.farClipPlane
            };

            if (cinemachineBrain.ActiveVirtualCamera is CinemachineCamera camera)
                camera.Lens.FarClipPlane = cinemachineCamera.farClipPlane;


            SObserverFog.OnRenderDistanceChange();
        }

        public float GetRenderDistance() => cinemachineCamera.farClipPlane;
    }
}
