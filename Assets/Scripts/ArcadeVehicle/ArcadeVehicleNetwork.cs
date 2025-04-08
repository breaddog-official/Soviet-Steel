using UnityEngine;
using Unity.Burst;
using Mirror;
using UnityEngine.InputSystem;
using Scripts.Gameplay;
using TMPro;

namespace ArcadeVP
{
    [BurstCompile] 
    public class ArcadeVehicleNetwork : NetworkBehaviour
    {
        [SerializeField] public TMP_Text nickText;
        [Space]
        [SerializeField] public ArcadeVehicleController vehicleController;
        [SerializeField] public GameObject cinemachine;
        [SerializeField] public PlayerInput input;
        [SerializeField] public GameObject ui;
        [SerializeField] public GameObject mirrorCamera;
        [Space]
        [SerializeField] public bool autoStart;

        [field: SyncVar(hook = nameof(ApplyNickname))]
        public string Nickname { get; private set; }
        public bool State { get; private set; }
        public bool AI { get; private set; }

        private ArcadeVehicleInput vehicleInput;
        private ArcadeVehicleAI vehicleAI;

        private void Awake()
        {
            vehicleInput = GetComponent<ArcadeVehicleInput>();
            vehicleAI = GetComponent<ArcadeVehicleAI>();

            SetState(false);
        }

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



        public void SetNickname(string name)
        {
            Nickname = name;
        }

        public void ApplyNickname(string old, string cur)
        {
            nickText.text = cur;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            vehicleController.AccelerationMultiplier = multiplier;
            if (!AI && !isOwned)
                vehicleController.Sync(connectionToClient, multiplier);
        }

        public void SetAI(bool state)
        {
            AI = state;
        }

        public void SetState(bool state)
        {
            State = state;

            vehicleController.SetHandbrake(!state);
            vehicleAI.enabled = state && AI;

            if ((isOwned && !AI) || state == false)
            {
                ui.SetActive(state);
                cinemachine.SetActive(state);
                mirrorCamera.SetActive(state);
                input.enabled = state;
                vehicleInput.enabled = state;
                nickText.enabled = !state;
            }
        }
    }
}
