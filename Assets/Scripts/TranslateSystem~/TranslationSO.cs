using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Scripts.TranslateSystem
{
    [CreateAssetMenu(fileName = "Translation", menuName = "Scripts/Translation/Translation")]
    public class TranslationSO : ScriptableObject
    {
        public Translation translation;


#if UNITY_EDITOR

        public TranslationKeys keys;

        //[Button]
        public void Populate()
        {
            foreach (var key in keys.keys)
            {
                translation.GetValues().TryAdd(key, string.Empty);
            }
        }

#endif
    }
}
