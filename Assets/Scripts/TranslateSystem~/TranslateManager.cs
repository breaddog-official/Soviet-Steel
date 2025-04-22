using System.Collections.Generic;

namespace Scripts.TranslateSystem
{
    public static class TranslateManager
    {
        private readonly static HashSet<string> TranslationKeys;
        private readonly static Dictionary<string, Translation> LoadedTranslations;

        public static Translation CurrentTranslation { get; private set; }


        #region Adding & Removing

        public static void AddKeys(IReadOnlyCollection<string> keys)
        {
            foreach (var key in keys)
            {
                AddKey(key);
            }
        }

        public static void RemoveKeys(IReadOnlyCollection<string> keys)
        {
            foreach (var key in keys)
            {
                RemoveKey(key);
            }
        }

        public static void AddKey(string key) => TranslationKeys.Add(key);
        public static void RemoveKey(string key) => TranslationKeys.Remove(key);

        public static void AddTranslation(string language, Translation translation) => LoadedTranslations.Add(language, translation);
        public static void RemoveTranslation(string language) => LoadedTranslations.Remove(language);

        #endregion

        public static bool LoadTranslation(string language, Translation translation)
        {
            return LoadedTranslations.TryAdd(language, translation);
        }

        public static bool UnloadTranslation(string language)
        {
            return LoadedTranslations.Remove(language);
        }

        public static bool TrySetTranslation(string language)
        {
            if (TryGetTranslation(language, out var translation))
            {
                CurrentTranslation = translation;
                return true;
            }
            return false;
        }

        public static bool TryGetTranslation(string language, out Translation translation)
        {
            return LoadedTranslations.TryGetValue(language, out translation);
        }


        public static IReadOnlyCollection<string> GetKeys() => TranslationKeys;
        public static IReadOnlyDictionary<string, Translation> GetTranslations() => LoadedTranslations;
    }
}
