using NaughtyAttributes;
using System;
using UnityEngine;

namespace Scripts.Settings
{
    [Serializable]
    public class Settings
    {
        #region Enums

        public enum ShadersQuality
        {
            [Tooltip("Mobile Diffuse and VertexLit shaders")]
            Low,
            [Tooltip("Diffuse shaders")]
            Medium,
            [Tooltip("Diffuse and Standart shaders")]
            High,
            [Tooltip("Standart shaders (or Autodesk)")]
            Extreme
        }

        public enum ResolutionType
        {
            Low,
            Medium,
            High,
            Full
        }

        public enum AntiAliasing
        {
            None,
            FXAA,
            SMAA,
            TAA,
            MSAA
        }

        public enum AntiAliasingMSAA
        {
            x2 = 2,
            x4 = 4,
            x8 = 8,
        }

        public enum FogType
        {
            None,
            Fog,
            VolumetricFog
        }

        public enum RenderDistance
        {
            Low,
            Medium,
            High,
            Maximum
        }

        public enum ShadowsType
        {
            None,

            [Tooltip("Shadows")]
            Standart,

            [Tooltip("Shadows + SSAO")]
            Advanced
        }

        public enum ShadowsQuality
        {
            Low,
            Medium,
            High,
            Ultra
        }

        /*public enum ParticlesMode
        {
            Disabled,
            [Tooltip("Without lithing. Low cost shaders")]
            Simple,
            Normal
        }

        public enum TexturesResolution
        {
            Low,
            Medium,
            High
        }

        public enum AnisotropicFiltration
        {
            None,
            x2,
            x4,
            x8,
            x16,
        }*/

        #endregion

        #region Fields

        [Header("Volume")]
        public float soundsVolume;
        public float musicVolume;
        public float ambientVolume;

        [Header("Video")]
        public FullScreenMode fullscreenMode;
        public Resolution resolution;

        [Header("Graphics")]
        public ShadowsType shadows;
        public ShadowsQuality shadowsQuality = ShadowsQuality.Medium;
        public ShadersQuality shadersQuality = ShadersQuality.Medium;
        public AntiAliasing antiAliasing;
        public AntiAliasingMSAA antiAliasingMsaa;
        public FogType fogType;
        public RenderDistance renderDistance = RenderDistance.Maximum;
        public ResolutionType resolutionType = ResolutionType.Full;
        public bool fastMode;

        [HideInInspector]
        public string debugCode;

        #endregion

        #region Reflections

        public void SetValue(string name, object value)
        {
            typeof(Settings).GetField(name).SetValue(this, value);
        }


        public object GetValue(string name)
        {
            return typeof(Settings).GetField(name).GetValue(this);
        }

        public T GetValue<T>(string name)
        {
            return (T)GetValue(name);
        }

        #endregion
    }
}