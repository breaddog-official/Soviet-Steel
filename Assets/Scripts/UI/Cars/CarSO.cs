using ArcadeVP;
using NaughtyAttributes;
using Scripts.Extensions;
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
            if (string.IsNullOrWhiteSpace(car.name))
                car.name = name;

            if (car.salt == 0)
                GenerateSalt();

            /*if (car.CarPrefab != null && car.CarPrefab.TryGetComponent(out ArcadeVehicleController controller))
            {
                if (car.Weight == 0)
                    car.Weight = controller.mass;
            }*/
        }

        [Button]
        protected void GenerateSalt()
        {
            car.salt = RandomE.RandomLong();//Random.Range(int.MinValue, int.MaxValue);
        }
#endif
    }
}