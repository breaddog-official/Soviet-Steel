using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Scripts.Settings
{
    public class SObserverShadersQualityGlobal : SettingHandler<Settings.ShadersQuality>
    {
        [Space]
        [SerializeField] protected Material[] materials;
        [Space]
        [SerializeField] protected Shader lowShader = null;
        [SerializeField] protected Shader mediumShader = null;
        [SerializeField] protected Shader highShader = null;
        [SerializeField] protected Shader extremeShader = null;


        public override void UpdateValue()
        {
            var quality = Setting;

            foreach (var material in materials)
            {
                if (TryGetShader(quality, out var shader))
                {
                    material.shader = shader;
                }
            }
        }


        protected virtual bool TryGetShader(Settings.ShadersQuality quality, out Shader shader)
        {
            shader = quality switch
            {
                Settings.ShadersQuality.Low => lowShader,
                Settings.ShadersQuality.Medium => mediumShader,
                Settings.ShadersQuality.High => highShader,
                Settings.ShadersQuality.Extreme => extremeShader,
                _ => throw new NotImplementedException()
            };

            if (shader == null && quality > 0)
            {
                return TryGetShader(quality - 1, out shader);
            }

            return shader != null;
        }
    }
}
