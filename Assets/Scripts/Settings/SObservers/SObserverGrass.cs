using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverGrass : SettingHandler<Settings.GrassDensity>
    {
        [Space]
        [SerializeField] private Terrain terrain;
        [Space]
        [SerializeField, Range(0f, 1f)] private float lowDensity = 0.128f;
        [SerializeField, Range(0f, 1f)] private float mediumDensity = 0.6f;
        [SerializeField, Range(0f, 1f)] private float highDensity = 1f;

        public override void UpdateValue()
        {
            terrain.detailObjectDensity = Setting switch
            {
                Settings.GrassDensity.Low => lowDensity,
                Settings.GrassDensity.Medium => mediumDensity,
                Settings.GrassDensity.High => highDensity,
                _ => 0
            };
        }
    }
}
