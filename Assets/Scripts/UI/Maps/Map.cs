using System;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace Scripts.UI
{
    [Serializable]
    public class Map
    {
        [field: SerializeField] public string Name { get; set; }
        [field: Space]
        [field: SerializeField] public string TranslateName { get; set; }
        [field: SerializeField] public string TranslateDescription { get; set; }
        [field: Scene]
        [field: SerializeField] public string Scene { get; set; }
        [field: SerializeField] public int Level { get; set; }
        [field: Space]
        [field: SerializeField] public Texture2D Icon { get; set; }
        [field: SerializeField] public int Salt { get; set; }

        /// <summary>
        /// Fully unique hash, based on salt + name
        /// </summary>
        public string MapHash => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Name}{Salt}"));
    }
}