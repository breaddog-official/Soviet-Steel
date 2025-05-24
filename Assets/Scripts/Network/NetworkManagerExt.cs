using ArcadeVP;
using Mirror;
using Scripts.Cars;
using Scripts.Gameplay;
using Scripts.SceneManagement;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    protected override void OnClientSceneInternal(SceneMessage msg)
    {

        // This needs to run for host client too. NetworkServer.active is checked there
        if (NetworkClient.isConnected)
            ClientChangeScene(msg.sceneName, msg.sceneOperation, msg.customHandling);
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



    public override async void ServerChangeScene(string newSceneName)
    {
        if (string.IsNullOrWhiteSpace(newSceneName))
        {
            Debug.LogError("ServerChangeScene empty scene name");
            return;
        }

        if (NetworkServer.isLoadingScene && newSceneName == networkSceneName)
        {
            Debug.LogError($"Scene change is already in progress for {newSceneName}");
            return;
        }

        // Throw error if called from client
        // Allow changing scene while stopping the server
        if (!NetworkServer.active && newSceneName != offlineScene)
        {
            Debug.LogError("ServerChangeScene can only be called on an active server.");
            return;
        }

        // Debug.Log($"ServerChangeScene {newSceneName}");
        NetworkServer.SetAllClientsNotReady();
        networkSceneName = newSceneName;

        // Let server prepare for scene change
        OnServerChangeScene(newSceneName);

        // set server flag to stop processing messages while changing scenes
        // it will be re-enabled in FinishLoadScene.
        NetworkServer.isLoadingScene = true;

        var loadTask = Loader.LoadSceneAsync(newSceneName);

        // ServerChangeScene can be called when stopping the server
        // when this happens the server is not active so does not need to tell clients about the change
        if (NetworkServer.active)
        {
            // notify all clients about the new scene
            NetworkServer.SendToAll(new SceneMessage
            {
                sceneName = newSceneName
            });
        }

        startPositionIndex = 0;
        startPositions.Clear();

        await loadTask;

        // Beacause we use await, we can alreade finish scene loading
        FinishLoadScene();
    }


    protected override async void ClientChangeScene(string newSceneName, SceneOperation sceneOperation = SceneOperation.Normal, bool customHandling = false)
    {
        if (string.IsNullOrWhiteSpace(newSceneName))
        {
            Debug.LogError("ClientChangeScene empty scene name");
            return;
        }
        print("on client change");
        //Debug.Log($"ClientChangeScene newSceneName: {newSceneName} networkSceneName{networkSceneName}");

        // Let client prepare for scene change
        OnClientChangeScene(newSceneName, sceneOperation, customHandling);

        // After calling OnClientChangeScene, exit if server since server is already doing
        // the actual scene change, and we don't need to do it for the host client
        if (NetworkServer.active)
            return;

        // set client flag to stop processing messages while loading scenes.
        // otherwise we would process messages and then lose all the state
        // as soon as the load is finishing, causing all kinds of bugs
        // because of missing state.
        // (client may be null after StopClient etc.)
        // Debug.Log("ClientChangeScene: pausing handlers while scene is loading to avoid data loss after scene was loaded.");
        NetworkClient.isLoadingScene = true;

        // Cache sceneOperation so we know what was requested by the
        // Scene message in OnClientChangeScene and OnClientSceneChanged
        clientSceneOperation = sceneOperation;

        // scene handling will happen in overrides of OnClientChangeScene and/or OnClientSceneChanged
        // Do not call FinishLoadScene here. Custom handler will assign loadingSceneAsync and we need
        // to wait for that to finish. UpdateScene already checks for that to be not null and isDone.
        //if (customHandling)
        //    return;

        switch (sceneOperation)
        {
            case SceneOperation.Normal:
                await Loader.LoadSceneAsync(newSceneName);
                break;
            case SceneOperation.LoadAdditive:
                // Ensure additive scene is not already loaded on client by name or path
                // since we don't know which was passed in the Scene message
                if (!SceneManager.GetSceneByName(newSceneName).IsValid() && !SceneManager.GetSceneByPath(newSceneName).IsValid())
                    await SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
                else
                {
                    Debug.LogWarning($"Scene {newSceneName} is already loaded");

                    // Reset the flag that we disabled before entering this switch
                    NetworkClient.isLoadingScene = false;
                }
                break;
            case SceneOperation.UnloadAdditive:
                // Ensure additive scene is actually loaded on client by name or path
                // since we don't know which was passed in the Scene message
                if (SceneManager.GetSceneByName(newSceneName).IsValid() || SceneManager.GetSceneByPath(newSceneName).IsValid())
                    await SceneManager.UnloadSceneAsync(newSceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                else
                {
                    Debug.LogWarning($"Cannot unload {newSceneName} with UnloadAdditive operation");

                    // Reset the flag that we disabled before entering this switch
                    NetworkClient.isLoadingScene = false;
                }
                break;
        }

        // don't change the client's current networkSceneName when loading additive scene content
        if (sceneOperation == SceneOperation.Normal)
            networkSceneName = newSceneName;

        // Beacause we use await, we can alreade finish scene loading
        FinishLoadScene();
    }

    protected override void UpdateScene() { }




    public struct ConnectMessage : NetworkMessage
    {
        public string name;
        public string carHash;
    }
}
