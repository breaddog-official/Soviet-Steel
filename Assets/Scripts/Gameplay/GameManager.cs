using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Threading;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.Gameplay
{
    public class GameManager : NetworkBehaviour
    {
        [field: SerializeField] public RoadManager RoadManager { get; private set; }
        [SerializeField] private bool autoStartMatch = true;


        [field: SyncVar(hook = nameof(IsMatchUpdate))]
        public bool IsMatch { get; private set; }
        public static event Action<bool> MatchStateChanged;


        [field: SyncVar]
        public double MatchTime { get; private set; }
        protected CancellationTokenSource timeCancellation;


        public static GameManager Instance { get; private set; }
        public static GameMode GameMode { get; private set; } = new GameMode();


        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
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




        public void AddPlayer(NetworkConnectionToClient conn) => AddPlayer(conn.identity.gameObject);
        public void AddPlayer(GameObject player)
        {
            RoadManager.AddPlayer(player);
        }


        public void RemovePlayer(NetworkConnectionToClient conn) => RemovePlayer(conn.identity.gameObject);
        public void RemovePlayer(GameObject player)
        {
            RoadManager.RemovePlayer(player);
        }    
    }
}
