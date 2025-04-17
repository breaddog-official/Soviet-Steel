using System;
using UnityEngine;
using NaughtyAttributes;
using System.Text;
using UnityEngine.Audio;

namespace Scripts.Cars
{
    [Serializable]
    public class Car
    {
        [SerializeField] public string name;
        [SerializeField, ResizableTextArea] public string description;
        [Space]
        [SerializeField] public GameObject carPrefab;
        [SerializeField] public GameObject carModelPrefab;
        [Space]
        [SerializeField, Range(1, 10)] public int spawnBotChance = 4;
        [Space]
        [SerializeField] public MusicType musicType;
        [SerializeField] public AudioResource customMusic;
        [SerializeField] public string[] musicPatterns;
        [Space]
        [SerializeField] public long salt;

        public enum MusicType
        {
            None,
            Patterns,
            Music
        }

        /// <summary>
        /// Fully unique hash, based on name + salt for reliability
        /// </summary> 
        public string CarHash => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{name}{salt}")); // Я знаю что это толком не хэш а просто base64, мне просто лень было делать хэш функцию, да и наверное она здесь не очень то и нужна
    }
}