using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Extensions;

using Cysharp.Threading.Tasks;
using System;

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


        public void SetValues(string name, int players, int maxPlayers, long serverId, ServerDiscoveryUI discovery = null, Uri uri = null)
        {
            nameText.SetText(name);
            playersText.SetText(players.ToString());
            maxPlayersText.SetText(maxPlayers.ToString());

            ServerId = serverId;
            Uri = uri;

            this.discovery = discovery;

            connectButton.interactable = players < maxPlayers;

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
            discovery?.ConnectServer(Uri);
        }
    }
}