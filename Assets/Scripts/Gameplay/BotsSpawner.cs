using ArcadeVP;
using Mirror;
using System.Collections.Generic;
using Scripts.Gameplay;
using UnityEngine;

public class BotsSpawner : NetworkBehaviour
{
    [SerializeField] protected string[] names;

    private readonly List<int> usedNames = new();


    public override void OnStartServer()
    {
        for (int i = 0; i < GameManager.GameMode.bots; i++)
        {
            var player = NetworkManagerExt.instance.SpawnPlayer(new()
            {
                name = GetName(),
                carHash = GetCar()
            });

            NetworkServer.Spawn(player, NetworkServer.localConnection);

            if (player.TryGetComponent(out ArcadeVehicleNetwork network))
            {
                network.SetAI(true);
            }
        }
    }

    private string GetCar()
    {
        int index = Random.Range(0, NetworkManagerExt.instance.registeredCars.Length);
        return NetworkManagerExt.instance.registeredCars[index].car.CarHash;
    }

    private string GetName()
    {
        int nameIndex = 0;

        if (usedNames.Count == names.Length)
            usedNames.Clear();

        do
        {
            nameIndex = Random.Range(0, names.Length);
        }
        while (usedNames.Contains(nameIndex));

        return names[nameIndex];
    }
}
