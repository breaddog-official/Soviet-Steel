using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverGeometry : SettingHandler<Settings.Geometry>
    {
        [Space]
        [SerializeField] private bool isLocal;
        [Space]
        [ShowIf(nameof(isLocal))]
        [SerializeField] private Terrain terrain;
        [Space]
        [SerializeField, HideIf(nameof(isLocal))] private float lowBias = 1f;
        [SerializeField, HideIf(nameof(isLocal))] private float mediumBias = 2f;
        [SerializeField, HideIf(nameof(isLocal))] private float highBias = 20f;
        [Space]
        [SerializeField, ShowIf(nameof(isLocal))] private int lowError = 150;
        [SerializeField, ShowIf(nameof(isLocal))] private int mediumError = 25;
        [SerializeField, ShowIf(nameof(isLocal))] private int highError = 0;
        [Space]
        [SerializeField, ShowIf(nameof(isLocal)), Range(0f, 1f)] private float lowDensity = 0.128f;
        [SerializeField, ShowIf(nameof(isLocal)), Range(0f, 1f)] private float mediumDensity = 0.6f;
        [SerializeField, ShowIf(nameof(isLocal)), Range(0f, 1f)] private float highDensity = 1f;

        public override void UpdateValue()
        {
            if (isLocal)
            {
                terrain.heightmapPixelError = Setting switch
                {
                    Settings.Geometry.Low => lowError,
                    Settings.Geometry.Medium => mediumError,
                    Settings.Geometry.High => highError,
                    _ => 0
                };

                terrain.detailObjectDensity = Setting switch
                {
                    Settings.Geometry.Low => lowDensity,
                    Settings.Geometry.Medium => mediumDensity,
                    Settings.Geometry.High => highDensity,
                    _ => 0
                };
            }
            else
            {
                QualitySettings.lodBias = Setting switch
                {
                    Settings.Geometry.Low => lowBias,
                    Settings.Geometry.Medium => mediumBias,
                    Settings.Geometry.High => highBias,
                    _ => 5f
                };
            }
        }
    }
}
