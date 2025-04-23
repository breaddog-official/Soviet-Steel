using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.UI
{
    public class LeaveUI : MonoBehaviour
    {
        public UnityEvent OnLeave;

        public void Leave()
        {
            OnLeave?.Invoke();
            NetworkManager.singleton.StopHost();
        }
    }
}
