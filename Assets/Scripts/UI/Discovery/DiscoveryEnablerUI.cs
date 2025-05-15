using Mirror.Discovery;
using Scripts.Network;
using UnityEngine;

namespace Scripts.UI
{
    public class DiscoveryEnablerUI : MonoBehaviour
    {
        public ServerFoundUnityEvent<ServerDiscovery.Response> OnServerFound;

        private void OnEnable()
        {
            ServerDiscovery.Instance.StartDiscovery();
            ServerDiscovery.Instance.OnServerFound = OnServerFound;
        }

        private void OnDisable()
        {
            ServerDiscovery.Instance.StopDiscovery();
            ServerDiscovery.Instance.OnServerFound = null;
        }
    }
}
