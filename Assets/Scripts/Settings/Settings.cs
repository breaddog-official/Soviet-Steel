using NaughtyAttributes;
using Scripts.TranslateManagement;
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

        public enum Geometry
        {
            Low,
            Medium,
            High,
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

        // Like in CS2 :)
        public enum ShadowsSources
        {
            DirectionalOnly,
            All
        }

        public enum ScreenPostEffects
        {
            Default,
            Retro
        }

        public enum MirrorQuality
        {
            Disabled,
            Simple,
            Standart,
            Advanced
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

        [Header("Global")]
        public ApplicationLanguage language = ApplicationLanguage.Unknown;

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
        public ShadowsSources shadowsSources = ShadowsSources.DirectionalOnly;
        public ShadersQuality shadersQuality = ShadersQuality.Medium;
        public AntiAliasing antiAliasing;
        public AntiAliasingMSAA antiAliasingMsaa;
        public ScreenPostEffects screenPostEffects;
        public FogType fogType;
        public RenderDistance renderDistance = RenderDistance.Maximum;
        public ResolutionType resolutionType = ResolutionType.Full;
        public Geometry geometry = Geometry.Medium;
        public bool fastMode;
        public MirrorQuality mirrorQuality = MirrorQuality.Standart;

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