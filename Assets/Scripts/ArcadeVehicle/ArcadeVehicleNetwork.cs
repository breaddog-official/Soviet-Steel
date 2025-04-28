using UnityEngine;
using Unity.Burst;
using Mirror;
using UnityEngine.InputSystem;
using Scripts.Gameplay;
using TMPro;
using Scripts.SplitScreen;
using Unity.Cinemachine;

namespace ArcadeVP
{
    [BurstCompile] 
    public class ArcadeVehicleNetwork : NetworkBehaviour
    {
        [SerializeField] public TMP_Text nickText;
        [SerializeField] public NicknameSettings nicknameSettings;
        [Space]
        [SerializeField] public ArcadeVehicleController vehicleController;
        [SerializeField] public CinemachineCamera cinemachine;
        [SerializeField] public PlayerInput input;
        [SerializeField] public GameObject ui;
        [SerializeField] public GameObject mirrorCamera;
        [SerializeField] public PhysicsMaterial playerMaterial; 
        [SerializeField] public PhysicsMaterial aiMaterial;
        [Space]
        [SerializeField] public bool autoStart;

        [field: SyncVar(hook = nameof(ApplyNickname))]
        public string Nickname { get; private set; }
        [field: SyncVar(hook = nameof(ApplySpeedMultiplier))]
        public float SpeedMultiplier { get; private set; } = 1f;


        [field: SyncVar]
        public bool AI { get; private set; }
        public bool State { get; private set; }

        [field: SyncVar(hook = nameof(ApplyWin))]
        public bool IsWin { get; private set; }

        private ArcadeVehicleInput vehicleInput;
        private ArcadeVehicleAI vehicleAI;

        public static ArcadeVehicleNetwork LocalPlayerNetwork { get; private set; }



        private void Awake()
        {
            vehicleInput = GetComponent<ArcadeVehicleInput>();
            vehicleAI = GetComponent<ArcadeVehicleAI>();
            
            SetState(autoStart);
        }


        public override void OnStartLocalPlayer()
        {
            LocalPlayerNetwork = this;
        }

        private void Update()
        {
            SetState(autoStart || GameManager.Instance.IsMatch);
        }


        [Command]
        public void SendRequestToNickname(string nickname)
        {
            if (nicknameSettings.TryValidateNickname(nickname, out string newNickname))
            {
                SetNickname(newNickname);
            }
        }

        public void SetNickname(string name) => Nickname = name;
        private void ApplyNickname(string old, string cur) => nickText.text = cur;

        public void SetSpeedMultiplier(float multiplier)
        {
            SpeedMultiplier = multiplier;
            if (NetworkServer.active) ApplySpeedMultiplier();
        }
        public void ApplySpeedMultiplier(float old = default, float cur = default)
        {
            vehicleController.AccelerationMultiplier = SpeedMultiplier;
        }

        public void SetAI(bool state)
        {
            AI = state;
        }

        public void SetState(bool state)
        {
            State = state;

            bool stateAi = State && AI;
            bool statePlayer = State && isOwned && !AI;

            vehicleController.SetHandbrake(!state);
            vehicleController.SetColliderMaterial(AI ? aiMaterial : playerMaterial);

            ui.SetActive(statePlayer);
            cinemachine.gameObject.SetActive(statePlayer);
            mirrorCamera.SetActive(isOwned && !AI);

            input.enabled = statePlayer;
            vehicleInput.enabled = statePlayer;
            vehicleAI.enabled = stateAi;

            nickText.enabled = !statePlayer || !state;
        }

        public void SetWin(bool state)
        {
            IsWin = state;
        }

        public void ApplyWin(bool old, bool cur)
        {
            if (!AI && isOwned)
            {
                SetAI(cur);
                CinemachineHelicopter.SetState(cur);
            }
        }



        public void SetSplitScreenPlayer(SplitScreenPlayer splitScreenPlayer, int index)
        {
            cinemachine.OutputChannel = SplitScreenSpawner.GetChannel(index);

            (string, InputDevice) controlScheme = splitScreenPlayer.device switch
            {
                SplitScreenDevice.KeyboardLeft => new("KeyboardLeft", Keyboard.current),
                SplitScreenDevice.KeyboardRight => new("KeyboardRight", Keyboard.current),
                SplitScreenDevice.Gamepad => new("Gamepad", Gamepad.current),
                SplitScreenDevice.Joystick => new("Joystick", Joystick.current),
                _ => throw new System.NotImplementedException()
            };

            input.neverAutoSwitchControlSchemes = true;
            input.SwitchCurrentControlScheme(controlScheme.Item1, controlScheme.Item2);
        }
    }
}
