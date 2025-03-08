using ArcadeVP;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private RoadManager roadManager;
    [SerializeField] private bool autoStartMatch = true;

    [field: SyncVar(hook = nameof(IsMatchUpdate))]
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



    public void StartMatch() => IsMatch = true;
    public void StopMatch() => IsMatch = false;


    private void IsMatchUpdate(bool oldIsMatch, bool isMatch)
    {
        if (NetworkClient.localPlayer != null && NetworkClient.localPlayer.TryGetComponent<ArcadeVehicleNetwork>(out var vehicle))
        {
            vehicle.SetState(isMatch);
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
