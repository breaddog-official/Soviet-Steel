using UnityEngine;
using Scripts.UI;
using TMPro;
using System.Linq;
using Scripts.Gameplay;
using System.Collections.Generic;
using Scripts.Audio;


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
        [SerializeField] protected Transform carSpawnParent;
        [SerializeField] protected DynamicMusic dynamicMusic;

        private readonly Dictionary<Car, GameObject> spawnedCars = new();
        private GameObject currentSpawnedActiveCar;


        protected virtual void Awake()
        {
            values.AddRange(NetworkManagerExt.instance.registeredCars.Select(c => c.car));

            Select(GameManager.Car);
        }

        public override void ApplyCurrentValue()
        {
            var car = CurrentValue;

            nameText.text = car.Name;
            descriptionText.text = car.Description;

            GameManager.SetCar(car);


            if (spawnCar && car.CarModelPrefab != null)
            {
                if (currentSpawnedActiveCar != null)
                    currentSpawnedActiveCar.SetActive(false);

                if (spawnedCars.TryGetValue(car, out var spawnedCar))
                {
                    spawnedCar.SetActive(true);
                }
                else
                {
                    if (spawnedCars.Count >= cleanupCount)
                        ClearSpawnedCars();

                    spawnedCar = Instantiate(car.CarModelPrefab, carSpawnParent);
                    spawnedCars.Add(car, spawnedCar);

                    if (spawnedCar.TryGetComponent(out DynamicMusicInstance music))
                    {
                        music.SetDynamicMusic(dynamicMusic);
                        music.Activate();
                    }
                }
                currentSpawnedActiveCar = spawnedCar;
            }
        }

        private void ClearSpawnedCars()
        {
            foreach (var car in spawnedCars)
            {
                Destroy(car.Value);
            }

            spawnedCars.Clear();
        }
    }
}