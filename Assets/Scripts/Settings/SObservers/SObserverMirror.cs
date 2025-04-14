using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverMirror : SettingHandler<MirrorQuality>
    {
        private enum ObserverType
        {
            Global,
            Local,
            UI
        }

        [Space]
        [SerializeField] private ObserverType observerType;
        [ShowIf(nameof(observerType), ObserverType.Local)]
        [SerializeField] protected Camera mirror;
        [ShowIf(nameof(observerType), ObserverType.Global)]
        [SerializeField] protected Vector2Int resolution = new(1920, 1080);
        [ShowIf(nameof(observerType), ObserverType.Global)]
        [SerializeField] protected Vector2 textureResolution;
        [ShowIf(nameof(observerType), ObserverType.Global), Range(4, 32)]
        [SerializeField] protected int textureBits = 24;
        [ShowIf(nameof(observerType), ObserverType.UI)]
        [SerializeField] protected RawImage uiMirror;

        private static RenderTexture renderTexture;
        private static event Action OnRenderTextureChanged;


        /*protected override void OnEnable()
        {
            if (observerType == ObserverType.Local)
            {
                OnRenderTextureChanged += UpdateRenderTextures;
            }

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            if (observerType == ObserverType.Local)
            {
                OnRenderTextureChanged -= UpdateRenderTextures;
            }

            base.OnDisable();
        }

        private void OnDestroy()
        {
            if (observerType == ObserverType.Global)
            {
                ClearTexture();
            }
        }

        */public override void UpdateValue()
        {/*
            var setting = Setting;

            if (observerType == ObserverType.Global)
            {
                float settingMultiplier = setting switch
                {
                    MirrorQuality.Simple => 0.5f,
                    MirrorQuality.Standart => 1f,
                    _ => 1f
                };

                Vector2Int textureMultiplier = new(Screen.currentResolution.width / resolution.x, Screen.currentResolution.height / resolution.y);
                int width = (int)(textureResolution.x * settingMultiplier * textureMultiplier.x);
                int height = (int)(textureResolution.y * settingMultiplier * textureMultiplier.y);

                if (renderTexture == null || renderTexture.width != width || renderTexture.height != height)
                {
                    ClearTexture();

                    renderTexture = new RenderTexture(width, height, textureBits);
                    renderTexture.Create();

                    OnRenderTextureChanged?.Invoke();
                }
            }
            else if (observerType == ObserverType.Local)
            {
                mirror.enabled = setting != MirrorQuality.Disabled;
            }
            else if (observerType == ObserverType.UI)
            {
                uiMirror.enabled = setting != MirrorQuality.Disabled;
            }

            UpdateRenderTextures();
        */}/*

        private static void ClearTexture()
        {
            if (renderTexture == null)
                return;

            renderTexture.Release();
            renderTexture = null;
        }

        private void UpdateRenderTextures()
        {
            if (renderTexture == null)
                return;

            if (observerType == ObserverType.Local)
            {
                mirror.targetTexture = renderTexture;
            }
            else if (observerType == ObserverType.UI)
            {
                uiMirror.texture = renderTexture;
            }
        }*/
    }
}
