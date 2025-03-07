using Mirror;
using System;

public class NetworkManagerExt : NetworkManager
{
    public static event Action<NetworkConnectionToClient> OnServerAddPlayerAction;
    public static event Action<NetworkConnectionToClient> OnServerDisconnectAction;


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
}
