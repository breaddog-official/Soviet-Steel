using Mirror;
using Scripts.Cars;
using Scripts.Gameplay;
using Scripts.UI;
using System;
using System.Linq;
using UnityEngine;

public class NetworkManagerExt : NetworkManager
{
    public CarSO[] registeredCars;
    public MapSO[] registeredMaps;

    public static event Action<NetworkConnectionToClient> OnServerAddPlayerAction;
    public static event Action<NetworkConnectionToClient> OnServerDisconnectAction;

    public static NetworkManagerExt instance;


    public override void Awake()
    {
        instance = this;

        spawnPrefabs.AddRange(registeredCars
                        .Where(c => !spawnPrefabs.Contains(c.car.CarPrefab))
                        .Select(c => c.car.CarPrefab));

        base.Awake();
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        OnServerAddPlayerAction?.Invoke(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        OnServerDisconnectAction?.Invoke(conn);

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


        // Spawn player
        var car = registeredCars.Where(c => c.car.CarHash == message.carHash).FirstOrDefault();

        if (car == default)
        {
            throw new Exception("Car is not supported on server.");
        }

        var carPrefab = car.car.CarPrefab;

        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(carPrefab, startPos.position, startPos.rotation)
            : Instantiate(carPrefab);
        player.name = $"{carPrefab.name} [connId={conn.connectionId}]";

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public struct ConnectMessage : NetworkMessage
    {
        public string carHash;
    }
}
