using System;
using Mirror;
using Scripts.Extensions;
using UnityEngine;

public class TransportSwitcher : MonoBehaviour
{
    [SerializeField]
    private Transport[] transports;

    private void Awake()
    {
        UpdateTransport();
    }

    public void UpdateTransport(ConnectionMode mode = ConnectionMode.None)
    {
        var currentPlatform = Application.platform.ToFlags();
        bool transportAssigned = false;

        foreach (var transport in transports)
        {
            bool available = transport.availablePlatforms.HasFlag(currentPlatform) && transport.availableConnectionModes.HasFlag(mode);

            if (available && !transportAssigned)
            {
                transportAssigned = true;
                transport.transport.enabled = available;
                NetworkManager.singleton.transport = transport.transport;
                Mirror.Transport.active = transport.transport;
            }
            else
                transport.transport.enabled = false;
        }
    }


    [Serializable]
    private struct Transport
    {
        public RuntimePlatformFlags availablePlatforms;
        public ConnectionMode availableConnectionModes;
        public Mirror.Transport transport;
    }
}

[Flags]
public enum ConnectionMode
{
    None = 0,
    Client = 1 << 0,
    Host = 1 << 1,
    Server = 1 << 2
}
