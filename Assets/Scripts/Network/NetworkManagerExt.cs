using ArcadeVP;
using Mirror;
using Scripts.Cars;
using Scripts.Gameplay;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkManagerExt : NetworkManager
{
    public CarSO[] registeredCars;
    public MapSO[] registeredMaps;

    public static event Action<GameObject> OnServerAddPlayerAction;
    public static event Action<GameObject> OnServerDisconnectAction;

    public static NetworkManagerExt instance;


    public override void Awake()
    {
        instance = this;

        spawnPrefabs.AddRange(registeredCars
                        .Where(c => !spawnPrefabs.Contains(c.car.carPrefab))
                        .Select(c => c.car.carPrefab));

        base.Awake();
    }



    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        OnServerDisconnectAction?.Invoke(conn.identity.gameObject);

        base.OnServerDisconnect(conn);
    }



    public override void OnStartServer()
    {
        NetworkServer.UnregisterHandler<AddPlayerMessage>();
        NetworkServer.RegisterHandler<ConnectMessage>(RecieveRequestToSpawn);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        SendRequestToSpawn();
    }

    /// <summary>
    /// Sends request to spawn player
    /// </summary>
    [Client]
    public virtual void SendRequestToSpawn()
    {
        // Return if player already spawned
        if (NetworkClient.connection.identity != null)
            return;
        
        ConnectMessage message = new()
        {
            name = "anonymous",
            carHash = GameManager.Car.CarHash
        };

        NetworkClient.Send(message);
    }

    /// <summary>
    /// Configures and spawns player
    /// </summary>
    [Server]
    protected void RecieveRequestToSpawn(NetworkConnectionToClient conn, ConnectMessage message)
    {
        // Return if player already spawned
        if (conn.identity != null)
            return;

        var player = SpawnPlayer(message, pl => NetworkServer.AddPlayerForConnection(conn, pl));
    }

    [Server]
    public GameObject SpawnPlayer(ConnectMessage message, Action<GameObject> configurePlayer = null)
    {
        if (!TryGetCar(message.carHash, out Car car))
        {
            throw new Exception("Car is not supported on server.");
        }

        var carPrefab = car.carPrefab;

        // Spawn player
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(carPrefab, startPos.position, startPos.rotation)
            : Instantiate(carPrefab);
        player.name = $"{carPrefab.name} [name={message.name}]";

        if (player.TryGetComponent(out ArcadeVehicleNetwork network))
        {
            network.SetNickname(message.name);
        }

        configurePlayer?.Invoke(player);

        print($"{player.name} is connected!");

        // Notify about new player
        OnServerAddPlayerAction?.Invoke(player);

        return player;
    }

    public static Map GetMap(string hash) => instance.registeredMaps.Where(m => m.map.MapHash == hash).FirstOrDefault().map;
    public static Car GetCar(string hash) => instance.registeredCars.Where(m => m.car.CarHash == hash).FirstOrDefault().car;


    public static bool TryGetMap(string hash, out Map map)
    {
        map = instance.registeredMaps.Where(m => m.map.MapHash == hash).FirstOrDefault().map;
        return map != default;
    }

    public static bool TryGetCar(string hash, out Car car)
    {
        car = instance.registeredCars.Where(m => m.car.CarHash == hash).FirstOrDefault().car;
        return car != default;
    }


    public struct ConnectMessage : NetworkMessage
    {
        public string name;
        public string carHash;
    }
}
