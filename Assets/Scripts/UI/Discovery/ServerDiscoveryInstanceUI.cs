using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Extensions;

using Cysharp.Threading.Tasks;
using System;
using Scripts.Network;
using Scripts.Gameplay;

namespace Scripts.UI
{
    public class ServerDiscoveryInstanceUI : MonoBehaviour
    {
        [SerializeField] protected float removeAfter = 4f;
        [Space]
        [SerializeField] protected TMP_Text nameText;
        [SerializeField] protected TMP_Text playersText;
        [SerializeField] protected TMP_Text maxPlayersText;
        [Space]
        [SerializeField] protected Button connectButton;

        public long ServerId { get; protected set; }
        public Uri Uri { get; protected set; }

        protected CancellationTokenSource removeToken;
        protected ServerDiscoveryUI discovery;

        protected ServerDiscovery.Response response;


        public void SetValues(ServerDiscovery.Response response, ServerDiscoveryUI discovery = null)
        {
            nameText.SetText(name);
            playersText.SetText(response.playersCount.ToString());
            maxPlayersText.SetText(response.maxPlayers.ToString());

            ServerId = response.serverId;
            Uri = response.uri;

            this.discovery = discovery;

            connectButton.interactable = response.playersCount < response.maxPlayers;

            this.response = response;

            removeToken?.ResetToken();
            removeToken = new();

            RemoveTask().Forget();
        }

        private void OnDestroy()
        {
            removeToken?.ResetToken();

            discovery?.RemoveServer(ServerId);
        }


        protected async UniTaskVoid RemoveTask()
        {
            await UniTask.Delay(removeAfter.ConvertSecondsToMiliseconds(), cancellationToken: removeToken.Token);

            Destroy(gameObject);
        }

        public void Connect()
        {
            GameManager.response = response;
            discovery?.ConnectServer(Uri);
        }
    }
}