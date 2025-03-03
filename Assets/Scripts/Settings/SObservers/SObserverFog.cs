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
        [SerializeField] protected bool useExponentialFog;
        [ShowIf(nameof(useExponentialFog))]
        [SerializeField] protected bool useExponentialSquaredFog;
        [ShowIf(nameof(useExponentialFog))]
        [SerializeField] protected float exponentialFogDensity = 0.03f;
        [ShowIf(EConditionOperator.And, nameof(smoothRenderDistanceByFog), nameof(useExponentialFog))]
        [SerializeField] protected float smoothRenderDistanceExpMultiplier = 1f;
        [Space]
        [SerializeField] protected bool smoothRenderDistanceByFog;
        [ShowIf(EConditionOperator.And, nameof(smoothRenderDistanceByFog), nameof(NonExponential))]
        [SerializeField] protected float linearFogSize = 5f;
        [ShowIf(nameof(smoothRenderDistanceByFog))]
        [SerializeField] protected Camera renderDistanceCamera;

        public bool NonExponential => !useExponentialFog;

        protected static event Action OnRenderDistanceChangeEvent;


        protected override void OnEnable()
        {
            base.OnEnable();

            OnRenderDistanceChangeEvent += UpdateValue;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            OnRenderDistanceChangeEvent -= UpdateValue;
        }


        public override void UpdateValue()
        {
            // Cache setting for not use reflections
            var setting = Setting;

            auraCamera.enabled = setting == FogType.VolumetricFog;
            RenderSettings.fog = setting != FogType.None;


            if (useExponentialFog)
            {
                RenderSettings.fogMode = useExponentialSquaredFog ? FogMode.ExponentialSquared : FogMode.Exponential;

                if (smoothRenderDistanceByFog)
                {
                    var neededDensity = GetDensity(renderDistanceCamera.farClipPlane, RenderSettings.fogMode);
                    RenderSettings.fogDensity = neededDensity > exponentialFogDensity ? neededDensity : exponentialFogDensity;
                }
                else
                {
                    RenderSettings.fogDensity = exponentialFogDensity;
                }
            }
            else if (smoothRenderDistanceByFog)
            {
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = renderDistanceCamera.farClipPlane - linearFogSize;
                RenderSettings.fogEndDistance = renderDistanceCamera.farClipPlane;
            }
        }

        protected virtual float GetDensity(float renderDistance, FogMode mode)
        {
            return mode switch
            {
                FogMode.Exponential => Mathf.Log(1f / 0.0019f) / renderDistance * smoothRenderDistanceExpMultiplier,
                FogMode.ExponentialSquared => Mathf.Sqrt(Mathf.Log(1f / 0.0019f)) / renderDistance * smoothRenderDistanceExpMultiplier,
                _ => 0f
            };
        }

        public static void OnRenderDistanceChange()
        {
            OnRenderDistanceChangeEvent?.Invoke();
        }
    }
}
