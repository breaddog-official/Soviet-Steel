using ArcadeVP;
using Mirror;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RoadManager roadManager;
    [SerializeField] private bool autoStartMatch = true;

    public bool IsMatch { get; private set; }




    private void Start()
    {
        if (autoStartMatch)
            StartMatch();
    }

    private void Update()
    {
        if (IsMatch)
        {
            roadManager.UpdatePlayers();
        }
    }

    private void OnEnable()
    {
        NetworkManagerExt.OnServerAddPlayerAction += AddPlayer;
        NetworkManagerExt.OnServerDisconnectAction += RemovePlayer;
    }

    private void OnDisable()
    {
        NetworkManagerExt.OnServerAddPlayerAction -= AddPlayer;
        NetworkManagerExt.OnServerDisconnectAction -= RemovePlayer;
    }



    public void StartMatch()
    {
        IsMatch = true;

        if (NetworkClient.active && NetworkClient.localPlayer.TryGetComponent<ArcadeVehicleNetwork>(out var vehicle))
        {
            vehicle.SetState(true);
        }
    }



    public void AddPlayer(NetworkConnectionToClient conn) => AddPlayer(conn.identity.gameObject);
    public void AddPlayer(GameObject player)
    {
        roadManager.AddPlayer(player);
    }


    public void RemovePlayer(NetworkConnectionToClient conn) => RemovePlayer(conn.identity.gameObject);
    public void RemovePlayer(GameObject player)
    {
        roadManager.RemovePlayer(player);
    }
}
