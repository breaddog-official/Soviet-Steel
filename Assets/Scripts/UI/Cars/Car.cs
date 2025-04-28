using System;
using UnityEngine;
using System.Text;

namespace Scripts.Cars
{
    [Serializable]
    public class Car
    {

        [SerializeField] public string name;
        [Space]
        [SerializeField] public string translateName;
        [SerializeField] public string translateDescription;
        [Space]
        [SerializeField] public GameObject carPrefab;
        [SerializeField] public GameObject carModelPrefab;
        [Space]
        [SerializeField, Range(1, 10)] public int spawnBotChance = 4;
        [SerializeField] public int level;
        [Space]
        [SerializeField] public string[] musicPatterns;
        [Space]
        [SerializeField] public Texture2D icon;
        [SerializeField] public long salt;

        /// <summary>
        /// Fully unique hash, based on name + salt for reliability
        /// </summary> 
        public string CarHash => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{name}{salt}")); // Я знаю что это толком не хэш а просто base64, мне просто лень было делать хэш функцию, да и наверное она здесь не очень то и нужна
    }
}