using Mirror;
using Scripts.Network;
using UnityEngine;

namespace Scripts.UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] protected bool autoStartDiscovery = true;


        #region Network

        protected NetworkManager NetworkManager => NetworkManager.singleton;
        protected ServerDiscovery Discovery => ServerDiscovery.Instance;


        private void Start()
        {
            if (autoStartDiscovery)
                Discovery.StartDiscovery();
        }

        public void StartClient()
        {
            NetworkManager.StartClient();
            Discovery.StopDiscovery();
        }

        public void StartHost()
        {
            NetworkManager.StartHost();
            Discovery.AdvertiseServer();
        }

        public void StartServer()
        {
            NetworkManager.StartServer();
            Discovery.AdvertiseServer();
        }



        public void StartSolo()
        {
            NetworkManager.StartHost();
            Discovery.StopDiscovery();

            SetMaxConnections(1);
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
