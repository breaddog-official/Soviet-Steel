using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Settings
{
    public abstract class SettingHandler : SettingObserver
    {
        [field: SerializeField]
        public virtual string Name { get; protected set; }

        public virtual object Setting => SettingsManager.GetSetting(Name);

        #region Editor
#if UNITY_EDITOR

        [field: Dropdown(nameof(GetSettingsNamesEditor)), OnValueChanged(nameof(SetSettingEditor))]
        public string setName;

        public const string default_dropdown_item = "Select setting";


        public virtual List<string> GetSettingsNamesEditor()
        {
            List<string> strings = GetFieldsNames();

            strings.Insert(0, default_dropdown_item);
            return strings;
        }

        public virtual List<string> GetFieldsNames()
        {
            return typeof(Settings).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                                               .Select(f => f.Name)
                                               .ToList();
        }

        public void SetSettingEditor()
        {
            if (setName == default_dropdown_item)
                return;

            Name = setName;
            setName = default_dropdown_item;
        }

#endif
        #endregion

        protected virtual void SetSetting(object value)
        {
            SettingsManager.SetSetting(Name, value);
        }


        protected override void OnEnable()
        {
            SettingsManager.OnSettingChanged += UpdateSetting;
            SettingsManager.OnSettingsChanged += UpdateValue;

            if (SettingsManager.Settings != null)
                UpdateValue();
        }

        protected override void OnDisable()
        {
            SettingsManager.OnSettingChanged -= UpdateSetting;
            SettingsManager.OnSettingsChanged -= UpdateValue;
        }


        protected virtual void UpdateSetting(string name)
        {
            if (string.Equals(name, Name))
                UpdateValue();
        }
    }

    /// <summary>
    /// Generic version of <see cref="SettingHandler"/>
    /// </summary>
    public abstract class SettingHandler<T> : SettingHandler
    {
        public new T Setting => SettingsManager.GetSetting<T>(Name);

        private static bool spawnedInstance;

        protected override void Awake()
        {
            base.Awake();

            if (dontDestroyOnLoad && IsSpawnedInstance())
            {
                Destroy(gameObject);
            }
            else
            {
                spawnedInstance = true;
            }
        }

        protected virtual bool IsSpawnedInstance() => spawnedInstance;

        #region Editor
#if UNITY_EDITOR

        public override List<string> GetFieldsNames()
        {
            return typeof(Settings).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                                               .Where(f => f.FieldType == typeof(T))
                                               .Select(f => f.Name)
                                               .ToList();
        }

#endif
        #endregion


        protected virtual void SetSetting(T value)
        {
            SettingsManager.SetSetting(Name, value);
        }
    }
}
