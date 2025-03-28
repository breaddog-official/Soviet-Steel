using UnityEngine;
using Unity.Burst;
using Mirror;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Scripts.Gameplay;

namespace ArcadeVP
{
    [BurstCompile] 
    public class ArcadeVehicleNetwork : NetworkBehaviour
    {
        [SerializeField] private ArcadeVehicleController vehicleController;
        [SerializeField] private CinemachineCamera vehicleCinemachine;
        [SerializeField] private PlayerInput vehicleInput;
        [SerializeField] private GameObject ui;
        [Space]
        [SerializeField] private bool autoStart;


        /*private void OnEnable() => GameManager.MatchStateChanged += SetState;
        private void OnDisable() => GameManager.MatchStateChanged -= SetState;

        public override void OnStartClient() => SetState(GameManager.Instance.IsMatch);
        public override void OnStopClient() => SetState(GameManager.Instance.IsMatch);

        public override void OnStartAuthority() => SetState(autoStart || GameManager.Instance.IsMatch);
        public override void OnStopAuthority() => SetState(false);*/


        private void Update()
        {
            SetState(autoStart || GameManager.Instance.IsMatch);
        }





        public void SetState(bool state)
        {
            vehicleController.Handbrake(!state);

            if (isOwned || state == false)
            {
                ui.SetActive(state);
                vehicleCinemachine.enabled = state;
                vehicleInput.enabled = state;
            }
        }
    }
}
