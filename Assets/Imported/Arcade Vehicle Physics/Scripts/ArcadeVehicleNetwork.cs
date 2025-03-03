using UnityEngine;
using Unity.Burst;
using Mirror;
using Unity.Cinemachine;

namespace ArcadeVP
{
    [BurstCompile] 
    public class ArcadeVehicleNetwork : NetworkBehaviour
    {
        [SerializeField] private CinemachineCamera vehicleCinemachine;



        private void Awake()
        {
            SetCameraState(false);
        }

        public override void OnStartAuthority()
        {
            SetCameraState(true);
        }

        public override void OnStopAuthority()
        {
            SetCameraState(false);
        }



        public void SetCameraState(bool state)
        {
            vehicleCinemachine.gameObject.SetActive(state);
        }
    }
}
