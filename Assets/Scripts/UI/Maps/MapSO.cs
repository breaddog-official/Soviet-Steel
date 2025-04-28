using NaughtyAttributes;
using UnityEngine;

namespace Scripts.UI
{
    [CreateAssetMenu(fileName = "Map", menuName = "Scripts/Map")]
    public class MapSO : ScriptableObject
    {
        public Map map;

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(map.TranslateName))
                map.TranslateName = name;

            if (map.Salt == 0)
                GenerateSalt();
        }

        [Button]
        protected void GenerateSalt()
        {
            map.Salt = Random.Range(int.MinValue, int.MaxValue);
        }
#endif
    }
}