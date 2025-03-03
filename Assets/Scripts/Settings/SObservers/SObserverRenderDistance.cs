using Unity.Cinemachine;
using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverRenderDistance : SettingHandler<RenderDistance>
    {
        [SerializeField] protected CinemachineCamera cinemachineCamera;
        [Space]
        [SerializeField] protected float lowDistance = 50;
        [SerializeField] protected float mediumDistance = 300;
        [SerializeField] protected float highDistance = 700;
        [SerializeField] protected float maxDistance = 1000;


        public override void UpdateValue()
        {
            cinemachineCamera.Lens.FarClipPlane = Setting switch
            {
                RenderDistance.Low => lowDistance,
                RenderDistance.Medium => mediumDistance,
                RenderDistance.High => highDistance,
                RenderDistance.Maximum => maxDistance,

                _ => cinemachineCamera.Lens.FarClipPlane
            };

            SObserverFog.OnRenderDistanceChange();
        }

        public float GetRenderDistance() => cinemachineCamera.Lens.FarClipPlane;
    }
}
