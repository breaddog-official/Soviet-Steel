using Mirror;
using UnityEngine;

namespace Scripts.UI
{
    public class LeaveUI : MonoBehaviour
    {
        public void Leave()
        {
            NetworkManager.singleton.StopHost();
        }
    }
}
