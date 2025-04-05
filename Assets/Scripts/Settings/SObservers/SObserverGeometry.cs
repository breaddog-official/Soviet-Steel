using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Settings
{
    public class SObserverGeometry : SettingHandler<Settings.Geometry>
    {
        [SerializeField] private bool isLocal;
        [ShowIf(nameof(isLocal))]
        [SerializeField] private Terrain terrain;
        [SerializeField, HideIf(nameof(isLocal))] private float lowBias = 1f;
        [SerializeField, HideIf(nameof(isLocal))] private float mediumBias = 2f;
        [SerializeField, HideIf(nameof(isLocal))] private float highBias = 20f;
        [SerializeField, ShowIf(nameof(isLocal))] private int lowError = 150;
        [SerializeField, ShowIf(nameof(isLocal))] private int mediumError = 25;
        [SerializeField, ShowIf(nameof(isLocal))] private int highError = 0;

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
