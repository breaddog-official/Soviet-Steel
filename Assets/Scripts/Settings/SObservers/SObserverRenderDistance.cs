using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverRenderDistance : SettingHandler<RenderDistance>
    {
        [SerializeField] protected float lowDistance = 50;
        [SerializeField] protected float mediumDistance = 300;
        [SerializeField] protected float highDistance = 700;
        [SerializeField] protected float maxDistance = 1000;


        protected override void Awake()
        {
            base.Awake();
        }

        public override void UpdateValue()
        {
            EnvironmentManager.Instance.SetRenderDistance(Setting switch
            {
                RenderDistance.Low => lowDistance,
                RenderDistance.Medium => mediumDistance,
                RenderDistance.High => highDistance,
                RenderDistance.Maximum => maxDistance,

                _ => throw new System.NotImplementedException()
            });

            SObserverFog.OnRenderDistanceChange();
        }
    }
}
