using System.Linq;
using Scripts.Cars;
using Scripts.Gameplay;
using Scripts.SaveManagement;
using Scripts.UI;
using UnityEngine;

public class GameModeSaver : MonoBehaviour
{
    [SerializeField] private CarSO defaultCar;
    [SerializeField] private MapSO defaultMap;
    [SerializeField] private Saver saver;
    [Space]
    [SerializeField] private SelectCarUI selectCarUI;

    private Car cachedCar;
    private Map cachedMap;


    private const string carKey = "Selected_Car";
    private const string mapKey = "Selected_Map";


    private void OnValidate()
    {
        if (defaultCar == null)
            defaultCar = FindFirstObjectByType<NetworkManagerExt>().registeredCars.FirstOrDefault();

        if (defaultMap == null)
            defaultMap = FindFirstObjectByType<NetworkManagerExt>().registeredMaps.FirstOrDefault();
    }

    private void Start()
    {
        var car = GameManager.Car ?? defaultCar.car;
        var map = GameManager.GameMode.Map ?? defaultMap.map;

        if (saver.Exists(carKey))
            car = NetworkManagerExt.GetCar(saver.Load(carKey));

        if (saver.Exists(mapKey))
            map = NetworkManagerExt.GetMap(saver.Load(mapKey));      

        GameManager.SetCar(car);
        GameManager.SetMap(map);

        Cache();

        if (selectCarUI != null)
        {
            selectCarUI.Initialize();
            selectCarUI.Select(cachedCar);
        } 
    }

    private void Update()
    {
        if (cachedCar != GameManager.Car || cachedMap != GameManager.GameMode.Map)
        {
            Cache();
        }
    }

    private void Cache()
    {
        cachedCar = GameManager.Car;
        cachedMap = GameManager.GameMode.Map;
        saver.Save(carKey, GameManager.Car.CarHash);
        saver.Save(mapKey, GameManager.GameMode.Map.MapHash);
    }
}
