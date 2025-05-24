using UnityEngine;
using Scripts.Extensions;

namespace Scripts.Settings
{
    public abstract class SettingObserver : MonoBehaviour
    {
        [SerializeField] protected bool dontDestroyOnLoad;


        protected virtual void Awake()
        {
            if (dontDestroyOnLoad)
            {
                gameObject.DontDestroyOnLoad();
            }
        }

        protected virtual void OnEnable()
        {
            SettingsManager.OnAnySettingChanged += UpdateValue;

            if (SettingsManager.Settings != null)
                UpdateValue();
        }

        protected virtual void OnDisable()
        {
            SettingsManager.OnAnySettingChanged -= UpdateValue;
        }


        public abstract void UpdateValue();
    }
}
