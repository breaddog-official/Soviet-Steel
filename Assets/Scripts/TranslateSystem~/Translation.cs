using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.TranslateSystem
{
    [Serializable]
    public class Translation
    {
        [SerializeField]
        private TranslationDictionary values;

        public IDictionary<string, string> GetValues() => values;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(TranslationDictionary))]
    public class TranslationDictionaryDrawer : DictionaryDrawer<string, string> { }
#endif
    public class TranslationDictionary : SerializableDictionary<string, string> { }
}
