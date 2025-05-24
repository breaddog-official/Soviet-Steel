using System;
using UnityEngine;

namespace Scripts.Settings
{
    public static class SettingsManager
    {
        public static Settings Settings { get; private set; }

        /// <summary>
        /// Calles when any Setting has been changed
        /// </summary>
        public static event Action OnAnySettingChanged;

        /// <summary>
        /// Calles when some setting has been changed
        /// </summary>
        public static event Action<string> OnSettingChanged;

        /// <summary>
        /// Calles when Settings has been changed
        /// </summary>
        public static event Action OnSettingsChanged;



        public static void SetSettings(Settings settings)
        {
            Settings = settings;
            OnAnySettingChanged?.Invoke();
            OnSettingsChanged?.Invoke();
        }



        /// <summary>
        /// Safely sets setting
        /// </summary>
        public static void SetSetting(string name, object value)
        {
            Settings?.SetValue(name, value);
            OnAnySettingChanged?.Invoke();
            OnSettingChanged?.Invoke(name);
        }

        /// <summary>
        /// Safely gets setting
        /// </summary>
        public static object GetSetting(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Field name {name} is invalid");

            return Settings?.GetValue(name);
        }

        /// <summary>
        /// Safely gets setting as T
        /// </summary>
        public static T GetSetting<T>(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Field name {name} is invalid");

            return Settings.GetValue<T>(name) ?? default;
        }
    }
}
