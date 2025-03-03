using System;
using UnityEngine;

namespace Scripts.Settings
{
    public static class SettingsManager
    {
        public static Settings Settings { get; private set; }

        /// <summary>
        /// Calls when settings was changed
        /// </summary>
        public static event Action OnSettingsChanged;



        public static void SetSettings(Settings settings)
        {
            Settings = settings;
            OnSettingsChanged?.Invoke();
        }



        /// <summary>
        /// Safely sets setting
        /// </summary>
        public static void SetSetting(string name, object value)
        {
            Settings?.SetValue(name, value);
            OnSettingsChanged?.Invoke();
        }

        /// <summary>
        /// Safely gets setting
        /// </summary>
        public static object GetSetting(string name)
        {
            return Settings?.GetValue(name);
        }

        /// <summary>
        /// Safely gets setting as T
        /// </summary>
        public static T GetSetting<T>(string name)
        {
            return Settings.GetValue<T>(name) ?? default;
        }
    }
}
