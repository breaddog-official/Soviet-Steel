using ArcadeVP;
using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Cars
{
    [CreateAssetMenu(fileName = "Car", menuName = "Scripts/Car")]
    public class CarSO : ScriptableObject
    {
        public Car car;

#if UNITY_EDITOR

        public void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(car.Name))
                car.Name = name;

            if (car.Salt == 0)
                GenerateSalt();

            if (car.CarPrefab != null && car.CarPrefab.TryGetComponent(out ArcadeVehicleController controller))
            {
                if (car.MaxSpeed == 0)
                    car.MaxSpeed = controller.maxSpeed;

                if (car.Weight == 0)
                    car.Weight = controller.mass;
            }
        }

        [Button]
        protected void GenerateSalt()
        {
            car.Salt = Random.Range(int.MinValue, int.MaxValue);
        }
#endif
    }
}