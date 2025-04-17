using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Scripts.Extensions;
using Scripts.SaveManagement;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Scripts.Settings
{
    public class SettingsSaver : SettingObserver
    {
        [Space]
        [SerializeField] private bool saveOnChange = true;
        [SerializeField] private bool saveWithDelay = true;
        [MinValue(0f), ShowIf(nameof(saveWithDelay))]
        [SerializeField] private float saveDelay = 1f;
        [Space]
        [SerializeField] private Serializer serializer;

        [Space]
        [SerializeField] private bool autoSetDefaultSettings;

        [ShowIf(nameof(autoSetDefaultSettings))]
        [SerializeField] private SettingsSO defaultSettingsSO;

        private CancellationTokenSource cancellationToken;

        public string SavePath => Path.Combine(SaveManager.GetDataPath(DataLocation.PreferPersistent), "Configs", "Settings.nahuy");


        protected void Start()
        {
            if (SettingsManager.Settings == null)
            {
                if (SaveManager.Exists(SavePath))
                {
                    LoadSettings().Forget();
                }
                else if (autoSetDefaultSettings && defaultSettingsSO != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(SavePath));

                    SettingsManager.SetSettings(defaultSettingsSO);
                    SaveSettings().Forget();
                }
            }
        }

        public override void UpdateValue()
        {
            if (saveOnChange)
            {
                if (saveWithDelay)
                    SaveDelayed();
                else
                    SaveSettings().Forget();
            }
        }

        private void SaveDelayed()
        {
            

            SaveDelayedAsync().Forget();
        }
        private async UniTaskVoid SaveDelayedAsync()
        {
            cancellationToken?.ResetToken();
            cancellationToken = new();

            await UniTask.Delay(saveDelay.ConvertSecondsToMiliseconds(), cancellationToken: cancellationToken.Token);
            await SaveSettings();

            print("Settings saved succesfully");
        }

        public async UniTask SaveSettings()
        {
            await SaveManager.SerializeAndSaveAsync(SettingsManager.Settings, SavePath, serializer);
        }

        public async UniTask LoadSettings()
        {
            SettingsManager.SetSettings(await SaveManager.LoadAndDeserializeAsync<Settings>(SavePath, serializer));
        }
    }
}
