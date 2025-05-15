using Aura2API;
using NaughtyAttributes;
using System;
using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
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
        [SerializeField] protected Camera renderDistanceCamera;

        protected bool IsLinear => RenderSettings.fogMode is FogMode.Linear;
        protected bool IsExponential => RenderSettings.fogMode is FogMode.Exponential or FogMode.ExponentialSquared;

        protected static event Action OnRenderDistanceChangeEvent;

        [SerializeField, HideInInspector] protected float cachedDencity;
        [SerializeField, HideInInspector] protected float cachedStartDistance;
        [SerializeField, HideInInspector] protected float cachedEndDistance;



        protected override void Awake()
        {
            Cache();

            base.Awake();
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


        public override void UpdateValue()
        {
            auraCamera.enabled = Setting == FogType.VolumetricFog;
            RenderSettings.fog = Setting != FogType.None;

            float distance = EnvironmentManager.Instance.renderDistance;

            if (smoothRenderDistanceByFog && RenderSettings.fog == true)
            {
                if (IsLinear)
                {
                    var targetStart = distance / 100f * (100f - linearPercentage);
                    var targetEnd = distance;

                    RenderSettings.fogStartDistance = Mathf.Min(targetStart, cachedStartDistance);
                    RenderSettings.fogEndDistance = Mathf.Min(targetEnd, cachedEndDistance);
                }
                else if (IsExponential)
                {
                    var targetDensity = GetDensity(distance, RenderSettings.fogMode);

                    RenderSettings.fogDensity = Mathf.Max(targetDensity, cachedDencity);
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
            cachedStartDistance = RenderSettings.fogStartDistance;
            cachedEndDistance = RenderSettings.fogEndDistance;
            cachedDencity = RenderSettings.fogDensity;
        }


        public void ResetToCache()
        {
            RenderSettings.fogStartDistance = cachedStartDistance;
            RenderSettings.fogEndDistance = cachedEndDistance;
            RenderSettings.fogDensity = cachedDencity;
        }
    }
}
