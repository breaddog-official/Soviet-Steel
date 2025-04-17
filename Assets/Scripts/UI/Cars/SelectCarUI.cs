using UnityEngine;
using Scripts.UI;
using TMPro;
using System.Linq;
using Scripts.Gameplay;
using System.Collections.Generic;
using Scripts.Audio;
using Scripts.Extensions;


namespace Scripts.Cars
{
    public class SelectCarUI : SelectUI<Car>
    {
        [Space]
        [SerializeField] protected TMP_Text nameText;
        [SerializeField] protected TMP_Text descriptionText;
        [Space]
        [SerializeField] protected bool spawnCar;
        [Min(1)]
        [SerializeField] protected int cleanupCount = 8;
        [SerializeField] protected Transform[] carSpawnParents;
        [SerializeField] protected DynamicMusic dynamicMusic;

        private readonly Dictionary<Car, GameObject[]> spawnedCars = new();
        private Car currentActiveCar;

        private bool isInitialized;


        protected virtual void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (isInitialized.CheckInitialization())
                return;

            values.AddRange(NetworkManagerExt.instance.registeredCars.Select(c => c.car));

            Select(GameManager.Car);
        }

        public override void ApplyCurrentValue()
        {
            var car = CurrentValue;

            nameText.text = car.name;
            descriptionText.text = car.description;

            GameManager.SetCar(car);


            if (spawnCar && car.carModelPrefab != null)
            {
                if (currentActiveCar != null)
                    SetCarsState(currentActiveCar, false);

                if (spawnedCars.ContainsKey(car))
                {
                    SetCarsState(car, true);
                }
                else
                {
                    if (spawnedCars.Count >= cleanupCount)
                        ClearSpawnedCars();

                    var spawnedCarsModels = SpawnCars(car);

                    spawnedCars.Add(car, spawnedCarsModels);
                }
                currentActiveCar = car;
            }
        }

        private GameObject[] SpawnCars(Car car)
        {
            var prefab = car.carModelPrefab;
            var array = new GameObject[carSpawnParents.Length];

            for (int i = 0; i < carSpawnParents.Length; i++)
            {
                var spawnedCar = Instantiate(prefab, carSpawnParents[i]);

                array[i] = spawnedCar;
            }

            return array;
        }

        private void SetCarsState(Car car, bool state)
        {
            foreach (var carModel in spawnedCars[car])
            {
                if (state && isActiveAndEnabled && car.musicType == Car.MusicType.Patterns && dynamicMusic != null)
                {
                    dynamicMusic.SetPattern(car.musicPatterns);
                }

                carModel.SetActive(state);
            }
        }

        private void ClearSpawnedCars()
        {
            foreach (var car in spawnedCars)
            {
                foreach (var carModel in car.Value)
                {
                    Destroy(carModel);
                }
            }

            spawnedCars.Clear();
        }
    }
}