using NaughtyAttributes;
using Scripts.SaveManagement;
using System.IO;
using UnityEngine;

namespace Scripts.Settings
{
    public class SettingsSaver : SettingObserver
    {
        [SerializeField] private bool saveOnChange = true;
        [Space]
        [SerializeField] private Serializer serializer;

        [Space]
        [SerializeField] private bool autoSetDefaultSettings;

        [ShowIf(nameof(autoSetDefaultSettings))]
        [SerializeField] private SettingsSO defaultSettingsSO;


        public string SavePath => Path.Combine(SaveManager.GetDataPath(DataLocation.PreferPersistent), "Configs", "Settings.nahuy");


        protected void Start()
        {
            if (SettingsManager.Settings == null)
            {
                if (SaveManager.Exists(SavePath))
                {
                    LoadSettings();
                }
                else if (autoSetDefaultSettings && defaultSettingsSO != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(SavePath));

                    SettingsManager.SetSettings(defaultSettingsSO);
                    SaveSettings();
                }
            }
        }

        public override void UpdateValue()
        {
            if (saveOnChange)
                SaveSettings();
        }


        public async void SaveSettings()
        {
            await SaveManager.SerializeAndSaveAsync(SettingsManager.Settings, SavePath, serializer);
        }

        public async void LoadSettings()
        {
            SettingsManager.SetSettings(await SaveManager.LoadAndDeserializeAsync<Settings>(SavePath, serializer));
        }
    }
}
