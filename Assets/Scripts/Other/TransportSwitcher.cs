using System;
using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using UnityEngine;

public class TransportSwitcher : MonoBehaviour
{
    [SerializeField]
    private Transport[] transports;

    private void Awake()
    {
        var currentPlatform = Application.platform.ToFlags();
        bool transportAssigned = false;

        foreach (var transport in transports)
        {
            bool available = transport.availablePlatforms.HasFlag(currentPlatform);

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
        public Mirror.Transport transport;
    }
}
