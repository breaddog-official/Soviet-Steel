using ArcadeVP;
using Mirror;
using System.Collections.Generic;
using Scripts.Gameplay;
using UnityEngine;
using System.Linq;

public class BotsSpawner : NetworkBehaviour
{
    [SerializeField] protected string[] names;

    private readonly List<int> usedNames = new();

    private int[] weightsWithSum;
    private int weightsSum;


    public override void OnStartServer()
    {
        CalculateWeights();

        for (int i = 0; i < GameManager.GameMode.bots; i++)
        {
            var player = NetworkManagerExt.instance.SpawnPlayer(new()
            {
                name = GetName(),
                carHash = GetCar()
            }, ConfigureBot);
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
        var index = 0;
        /*var number = Random.Range(0, weightsSum);

        for (int i = 0; i < weightsWithSum.Length; i++)
        {
            //print($"Number: {number}; Weight: {weightsWithSum[i]}; Length: {i}");
            bool last = i + 1 == weightsWithSum.Length;

            if (number >= weightsWithSum[i] && (last || number < weightsWithSum[i + 1]))
            {
                index = i;
                break;
            }
        }*/
        index = Random.Range(0, NetworkManagerExt.instance.registeredCars.Length);
        return NetworkManagerExt.instance.registeredCars[index].car.CarHash;
    }

    private string GetName()
    {
        if (usedNames.Count == names.Length)
            usedNames.Clear();

        int nameIndex;
        do
        {
            nameIndex = Random.Range(0, names.Length);
        }
        while (usedNames.Contains(nameIndex));

        usedNames.Add(nameIndex);

        return names[nameIndex];
    }

    private void CalculateWeights()
    {
        var weights = NetworkManagerExt.instance.registeredCars.Select(c => c.car.SpawnBotChance).ToArray();
        var sum = 0;

        weightsWithSum = new int[weights.Length];
        for (int i = 1; i < weights.Length; i++)
        {
            weightsWithSum[i] = sum + weights[i];
            sum += weights[i];
            print($"Sum: {sum}");
        }

        weightsSum = sum;
    }
}
