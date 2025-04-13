using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Threading;
using UnityEngine;
using Scripts.Extensions;
using Scripts.Cars;

namespace Scripts.Gameplay
{
    public class GameManager : NetworkBehaviour
    {
        [field: SerializeField] public RoadManager RoadManager { get; private set; }
        [field: SerializeField] public WinManager WinManager { get; private set; }
        [SerializeField] private bool autoStartMatch = true;


        [field: SyncVar(hook = nameof(IsMatchUpdate))]
        public bool IsMatch { get; private set; }
        public static event Action<bool> MatchStateChanged;


        [field: SyncVar]
        public double MatchTime { get; private set; }
        protected CancellationTokenSource timeCancellation;

        [SyncVar]
        private GameMode clientGameMode;
        private static GameMode serverGameMode = new GameMode();

        public static GameManager Instance { get; private set; }
        public static GameMode GameMode
        {
            get => Instance?.clientGameMode ?? serverGameMode;
            set => serverGameMode = value;
        }

        public static Car Car { get; private set; }


        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            Extensions.Extensions.ClearNetworksCache();
        }

        private void Start()
        {
            if (autoStartMatch)
                StartMatch();
        }

        private void Update()
        {
            if (IsMatch)
            {
                RoadManager.UpdatePlayers();
            }
        }

        private void OnEnable()
        {
            NetworkManagerExt.OnServerAddPlayerAction += AddPlayer;
            NetworkManagerExt.OnServerDisconnectAction += RemovePlayer;
        }

        private void OnDisable()
        {
            NetworkManagerExt.OnServerAddPlayerAction -= AddPlayer;
            NetworkManagerExt.OnServerDisconnectAction -= RemovePlayer;
        }



        public void StartMatch() => IsMatch = true;
        public void StopMatch() => IsMatch = false;


        private void IsMatchUpdate(bool oldIsMatch, bool isMatch)
        {
            MatchStateChanged?.Invoke(isMatch);

            timeCancellation?.ResetToken();
            timeCancellation = new();

            if (isMatch == true && NetworkServer.active)
            {
                MatchTime = 0d;
                MatchTimer(timeCancellation.Token).Forget();
            }
        }

        private async UniTask MatchTimer(CancellationToken token = default)
        {
            while (IsMatch)
            {
                MatchTime += Time.deltaTime;

                await UniTask.NextFrame(cancellationToken: token);
            }
        }



        public static void SetCar(Car car)
        {
            Car = car;
        }

        public void AddPlayer(GameObject player)
        {
            RoadManager.AddPlayer(player);
        }

        public void RemovePlayer(GameObject player)
        {
            RoadManager.RemovePlayer(player);
        }    
    }
}
