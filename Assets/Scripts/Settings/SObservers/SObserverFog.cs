using Aura2API;
using NaughtyAttributes;
using System;
using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    [ExecuteInEditMode]
    public class SObserverFog : SettingHandler<FogType>
    {
        [Space]
        [SerializeField] protected AuraCamera auraCamera;
        [Space]
        [Space]
        [SerializeField] protected bool smoothRenderDistanceByFog;
        [Range(0, 100)]
        [ShowIf(EConditionOperator.And, nameof(smoothRenderDistanceByFog), nameof(IsLinear))]
        [SerializeField] protected float linearPercentage = 5f;
        [ShowIf(EConditionOperator.And, nameof(smoothRenderDistanceByFog), nameof(IsExponential))]
        [SerializeField] protected float dencityMultiplier = 1f;
        [ShowIf(nameof(smoothRenderDistanceByFog))]
        [SerializeField] protected float distanceThreshold = 1000f;
        [ShowIf(nameof(smoothRenderDistanceByFog))]
        [SerializeField] protected Camera renderDistanceCamera;

        protected bool IsLinear => RenderSettings.fogMode is FogMode.Linear;
        protected bool IsExponential => RenderSettings.fogMode is FogMode.Exponential or FogMode.ExponentialSquared;

        protected static event Action OnRenderDistanceChangeEvent;

        protected float cachedDencity;
        protected float cachedStartDistance;
        protected float cachedEndDistance;



        protected virtual void Start()
        {
            Cache();
        }


        protected override void OnEnable()
        {
            OnRenderDistanceChangeEvent += UpdateValue;

            base.OnEnable();

        }

        protected override void OnDisable()
        {
            OnRenderDistanceChangeEvent -= UpdateValue;

            base.OnDisable();
        }

        [Button]
        public override void UpdateValue()
        {
            if (Application.isPlaying)
            {
                auraCamera.enabled = Setting == FogType.VolumetricFog;
                RenderSettings.fog = Setting != FogType.None;
            }

            if (smoothRenderDistanceByFog && renderDistanceCamera.farClipPlane < distanceThreshold)
            {
                if (IsLinear)
                {
                    var targetStart = renderDistanceCamera.farClipPlane / 100f * (100f - linearPercentage);
                    var targetEnd = renderDistanceCamera.farClipPlane;

                    RenderSettings.fogStartDistance = targetStart < cachedStartDistance ? targetStart : cachedStartDistance;
                    RenderSettings.fogEndDistance = targetEnd < cachedEndDistance ? targetEnd : cachedEndDistance;
                }
                else if (IsExponential)
                {
                    var targetDensity = GetDensity(renderDistanceCamera.farClipPlane, RenderSettings.fogMode);

                    RenderSettings.fogDensity = targetDensity > cachedDencity ? targetDensity : cachedDencity;
                }
            }
            else if (Application.isPlaying)
            {
                ResetToCache();
            }
        }

        protected virtual float GetDensity(float renderDistance, FogMode mode)
        {
            return mode switch
            {
                FogMode.Exponential => Mathf.Log(1f / 0.0019f) / renderDistance * dencityMultiplier,
                FogMode.ExponentialSquared => Mathf.Sqrt(Mathf.Log(1f / 0.0019f)) / renderDistance * dencityMultiplier,
                _ => 0f
            };
        }

        public static void OnRenderDistanceChange()
        {
            OnRenderDistanceChangeEvent?.Invoke();
        }


        [Button]
        public void Cache()
        {
            cachedDencity = RenderSettings.fogDensity;
            cachedStartDistance = RenderSettings.fogStartDistance;
            cachedEndDistance = RenderSettings.fogEndDistance;
        }

        [Button]
        public void ResetToCache()
        {
            RenderSettings.fogStartDistance = cachedStartDistance;
            RenderSettings.fogEndDistance = cachedEndDistance;
            RenderSettings.fogDensity = cachedDencity;
        }
    }
}
