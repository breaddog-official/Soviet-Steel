using Mirror;
using UnityEngine;

public class LeaveUI : MonoBehaviour
{
    public void Leave()
    {
        NetworkManager.singleton.StopHost();
    }
}
