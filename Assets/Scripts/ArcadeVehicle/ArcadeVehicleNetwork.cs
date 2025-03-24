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
        [SerializeField] private bool autoStart;



        private void Start()
        {
            SetState(GameManager.Instance.IsMatch);
        }



        private void OnEnable() => GameManager.Instance.MatchStateChanged += SetState;
        private void OnDisable() => GameManager.Instance.MatchStateChanged -= SetState;



        public override void OnStartAuthority()
        {
            if (autoStart)
                SetState(true);
        }

        public override void OnStopAuthority()
        {
            SetState(false);
        }



        public void SetState(bool state)
        {
            vehicleController.Handbrake(!state);
            vehicleCinemachine.enabled = state;
            vehicleInput.enabled = state;
        }
    }
}
