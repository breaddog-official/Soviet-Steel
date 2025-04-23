using System;
using Mirror;
using Scripts.Gameplay;
using Scripts.Network;
using UnityEngine;

namespace Scripts.UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] protected bool autoStartDiscovery = true;
        [SerializeField] protected NetworkPlayMode defaultPlayMode;


        #region Network

        protected enum NetworkPlayMode
        {
            Server,
            Host,
            Client,
            Solo
        }

        protected NetworkManager NetworkManager => NetworkManager.singleton;
        protected ServerDiscovery Discovery => ServerDiscovery.Instance;

        protected NetworkPlayMode playMode;

        protected Uri uri;


        private void Start()
        {
            playMode = defaultPlayMode;

            if (autoStartDiscovery)
                Discovery.StartDiscovery();
        }

        public void ClientMode() => playMode = NetworkPlayMode.Client;
        public void ClientMode(Uri uri)
        {
            ClientMode();
            this.uri = uri;
        }
        public void HostMode() => playMode = NetworkPlayMode.Host;
        public void ServerMode() => playMode = NetworkPlayMode.Server;
        public void SoloMode() => playMode = NetworkPlayMode.Solo;


        public void Play()
        {
            UpdateTransport();

            switch (playMode)
            {
                case NetworkPlayMode.Server:
                    NetworkManager.StartServer();
                    Discovery.AdvertiseServer();
                    break;

                case NetworkPlayMode.Host:
                    NetworkManager.StartHost();
                    Discovery.AdvertiseServer();
                    break;

                case NetworkPlayMode.Client:
                    if (uri != null) NetworkManager.StartClient(uri);
                    else NetworkManager.StartClient();
                    Discovery.StopDiscovery();
                    break;

                case NetworkPlayMode.Solo:
                    SetMaxConnections(1);

                    NetworkManager.StartHost();
                    Discovery.StopDiscovery();

                    break;
            }
        }


        private void UpdateTransport()
        {
            if (NetworkManager.singleton.TryGetComponent(out TransportSwitcher switcher))
            {
                switcher.UpdateTransport(playMode switch
                {
                    NetworkPlayMode.Server => ConnectionMode.Server,
                    NetworkPlayMode.Host or NetworkPlayMode.Solo => ConnectionMode.Host,
                    NetworkPlayMode.Client => ConnectionMode.Client,
                    _ => ConnectionMode.None
                });
            }
        }


        public void SetAddress(string address) => NetworkManager.networkAddress = address;
        public void SetPort(string port)
        {
            if (NetworkManager.transport is PortTransport portTransport && ushort.TryParse(port, out ushort parsedPort))
            {
                portTransport.Port = parsedPort; 
            }
        }
        public void SetMaxConnections(string maxConnections)
        {
            if (int.TryParse(maxConnections, out int parsedMaxConnections) && parsedMaxConnections >= 0)
            {
                SetMaxConnections(parsedMaxConnections);
            }
        }

        public void SetMaxConnections(int maxConnections)
        {
            NetworkManager.maxConnections = maxConnections;
        }

        #endregion



        public static void SetBotsCount(string bots)
        {
            GameManager.GameMode.bots = int.Parse(bots);
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
