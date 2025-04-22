using UnityEngine;

namespace Scripts.TranslateSystem
{
    [CreateAssetMenu(fileName = "TranslationKeys", menuName = "Scripts/Translation/Keys")]
    public class TranslationKeys : ScriptableObject
    {
        public string[] keys;
    }
}
