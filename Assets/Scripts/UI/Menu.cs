using System;
using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.Gameplay;
using Scripts.Network;
using Scripts.SceneManagement;
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

        protected static NetworkManager NetworkManager => NetworkManager.singleton;
        protected static ServerDiscovery Discovery => ServerDiscovery.Instance;

        protected NetworkPlayMode playMode;

        protected static Uri uri;


        private void Start()
        {
            playMode = defaultPlayMode;
            uri = null;

            if (autoStartDiscovery)
                Discovery.StartDiscovery();
        }

        public void ClientMode() => playMode = NetworkPlayMode.Client;
        public void ClientMode(Uri clientUri)
        {
            ClientMode();
            uri = clientUri;
        }
        public void HostMode() => playMode = NetworkPlayMode.Host;
        public void ServerMode() => playMode = NetworkPlayMode.Server;
        public void SoloMode() => playMode = NetworkPlayMode.Solo;


        public void Play()
        {
            UpdateTransport();

            PerformPlay(playMode);
        }

        private static void PerformPlay(NetworkPlayMode playMode)
        {
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
                    NetworkManager.maxConnections = 1;

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

        #endregion



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
