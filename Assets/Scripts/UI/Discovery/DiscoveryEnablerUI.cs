using Scripts.Network;
using UnityEngine;

namespace Scripts.UI
{
    public class DiscoveryEnablerUI : MonoBehaviour
    {
        private void OnEnable()
        {
            ServerDiscovery.Instance.StartDiscovery();
        }

        private void OnDisable()
        {
            ServerDiscovery.Instance.StopDiscovery();
        }
    }
}
