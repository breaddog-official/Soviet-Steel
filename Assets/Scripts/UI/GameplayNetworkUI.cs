using ArcadeVP;
using Mirror;
using Scripts.Gameplay;
using UnityEngine;

namespace Scripts.UI
{
    public class GameplayNetworkUI : MonoBehaviour
    {
        [SerializeField] protected GameObject gameplayUI;
        [SerializeField] protected GameObject networkUI;
        [SerializeField] protected GameObject networkStartMatchUI;
        [SerializeField] protected GameObject scoresUI;


        private void Update()
        {
            SetState(GameManager.Instance.IsMatch);
        }


        public void SetState(bool state)
        {
            if (gameplayUI != null)
                gameplayUI.SetActive(state);

            if (networkUI != null)
                networkUI.SetActive(!state);

            if (networkStartMatchUI != null)
                networkStartMatchUI.SetActive(!state && NetworkServer.active);

            if (scoresUI != null)
                scoresUI.SetActive(ArcadeVehicleNetwork.LocalPlayerNetwork != null && ArcadeVehicleNetwork.LocalPlayerNetwork.IsWin);
        }
    }
}