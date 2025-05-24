using Mirror;
using Scripts.Gameplay;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.UI
{
    public class GameModeUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField botsInputField;
        [SerializeField] private TMP_InputField roundsInputField;
        [SerializeField] private TMP_InputField serverNameInputField;
        [SerializeField] private TMP_InputField serverPortInputField;
        [SerializeField] private TMP_InputField maxPlayersInputField;

        protected static NetworkManager NetworkManager => NetworkManager.singleton;


        private void Start()
        {
            UpdateInputFields();
        }


        public void SetBotsCount(string bots)
        {
            if (int.TryParse(bots, out int result))
            {
                GameManager.GameMode.bots = Mathf.Max(result, 0);
            }

            UpdateInputFields();
        }

        public void SetRoundsCount(string rounds)
        {
            if (int.TryParse(rounds, out int result))
            {
                GameManager.GameMode.rounds = Mathf.Max(result, 1);
            }

            UpdateInputFields();
        }

        public void SetAddress(string address)
        {
            NetworkManager.networkAddress = address;

            UpdateInputFields();
        }

        public void SetPort(string port)
        {
            if (NetworkManager.transport is PortTransport portTransport && ushort.TryParse(port, out ushort parsedPort))
            {
                portTransport.Port = parsedPort;
            }

            UpdateInputFields();
        }
        public void SetMaxConnections(string maxConnections)
        {
            if (int.TryParse(maxConnections, out int parsedMaxConnections) && parsedMaxConnections >= 0)
            {
                NetworkManager.maxConnections = parsedMaxConnections;
            }

            UpdateInputFields();
        }


        public void UpdateInputFields()
        {
            botsInputField.text = GameManager.GameMode.bots.ToString();
            roundsInputField.text = GameManager.GameMode.rounds.ToString();
            serverNameInputField.text = NetworkManager.networkAddress;
            serverPortInputField.text = NetworkManager.transport is PortTransport portTransport ? portTransport.Port.ToString() : "0";
            maxPlayersInputField.text = NetworkManager.maxConnections.ToString();
        }
    }
}