using System;
using UnityEngine;
using NaughtyAttributes;
using System.Text;

namespace Scripts.Cars
{
    [Serializable]
    public class Car
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField, ResizableTextArea] public string Description { get; set; }
        [field: Space]
        [field: SerializeField] public GameObject CarPrefab { get; set; }
        [field: SerializeField] public GameObject CarModelPrefab { get; set; }
        [field: SerializeField] public float Weight { get; set; }
        [field: Space]
        [field: SerializeField, Range(1, 4)] public int SpeedCategory { get; set; } = 3;
        [field: SerializeField, Range(1, 4)] public int TurnCategory { get; set; } = 2;
        [field: Space]
        [field: SerializeField] public int Salt { get; set; }

        /// <summary>
        /// Fully unique hash, based on salt + name + car parameters for reliability
        /// </summary>
        public string CarHash => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Name}{(SpeedCategory + TurnCategory) * Weight}{Salt}"));
    }
}