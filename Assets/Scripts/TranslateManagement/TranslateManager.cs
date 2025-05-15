using System;
using Unity.Burst;
using UnityEngine;

namespace Scripts.TranslateManagement
{
    [BurstCompile]
    public static partial class TranslateManager
    {
        public static ApplicationLanguage GameLanguage { get; private set; } = ApplicationLanguage.Unknown;
        public static event Action GameLanguageChanged;

        public static Translation Translation { get; private set; }

        public static bool Initialized { get; private set; }

        #region Load & Save

        public static Translation LoadTranslation(ApplicationLanguage language)
        {
            if (TranslationConfig.Instance.TryGetTranslation(language, out var result))
            {
                return result.Translation;
            }
            else
            {
                throw new Exception($"{language} not founded");
            }
        }

        #endregion
        /// <summary>
        /// Returns the system language
        /// </summary>
        public static ApplicationLanguage GetSystemLanguage()
        {
#if YandexGamesPlatform_yg
            return YG.YG2.envir.language.HLToApplicationLanguage();
#else
            return Application.systemLanguage.ToApplicationLanguage();
#endif

        }
        /// <summary>
        /// Sets new language
        /// </summary>
        public static void ChangeLanguage(ApplicationLanguage newLanguage, bool withInvoke = true)
        {
            if (GameLanguage == newLanguage)
                return;

            Initialized = true;

            GameLanguage = newLanguage;
            Translation = LoadTranslation(GameLanguage);

            if (withInvoke) GameLanguageChanged?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SetSystemLanguage() => ChangeLanguage(GetSystemLanguage());

        public static string GetTranslationString(string name)
            => TranslateCacher.Get(name);
    }
}
