using ArcadeVP;
using Mirror;
using System.Collections.Generic;
using Scripts.Gameplay;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using Unity.VisualScripting;

public class BotsSpawner : NetworkBehaviour
{
    [SerializeField] protected string[] names;
    [Space]
    [SerializeField] protected bool variableSpawnDelay;
    [ShowIf(nameof(variableSpawnDelay)), MinMaxSlider(0, 10)]
    [SerializeField] protected Vector2 spawnDelay = new(2, 5);

    private static readonly List<int> notUsedNames = new();
    private static readonly List<int> notUsedCars = new();


    public override async void OnStartServer()
    {
        for (int i = 0; i < GameManager.GameMode.bots; i++)
        {
            var player = NetworkManagerExt.instance.SpawnPlayer(new()
            {
                name = GetName(),
                carHash = GetCar()
            }, ConfigureBot);

            if (variableSpawnDelay)
                await UniTask.Delay(Random.Range(spawnDelay.x, spawnDelay.y).ConvertSecondsToMiliseconds());
        }
    }

    private void ConfigureBot(GameObject bot)
    {
        NetworkServer.Spawn(bot, NetworkServer.localConnection);

        if (bot.TryGetComponent(out ArcadeVehicleNetwork network))
        {
            network.SetAI(true);
        }
    }

    private string GetCar()
    {
        if (notUsedCars.Count == 0)
            AddNotUsedCars();

        int notUsedIndex = Random.Range(0, notUsedCars.Count - 1);
        int carsIndex = notUsedCars[notUsedIndex];

        notUsedCars.Remove(notUsedIndex);

        return NetworkManagerExt.instance.registeredCars[carsIndex].car.CarHash;
    }

    private string GetName()
    {
        if (notUsedNames.Count == 0)
            AddNotUsedNames();

        int notUsedIndex = Random.Range(0, notUsedNames.Count - 1);
        int namesIndex = notUsedNames[notUsedIndex];

        notUsedNames.RemoveAt(notUsedIndex);

        return names[namesIndex];
    }

    private void AddNotUsedNames()
    {
        for (int i = 0; i < names.Length; i++)
        {
            notUsedNames.Add(i);
        }
    }

    private void AddNotUsedCars()
    {
        for (int i = 0; i < NetworkManagerExt.instance.registeredCars.Length; i++)
        {
            // Add index multiple times for more spawned cars
            for (int c = 0; c < NetworkManagerExt.instance.registeredCars[i].car.spawnBotChance; c++)
            {
                notUsedCars.Add(i);
            }
        }
    }
}
