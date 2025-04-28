using UnityEngine;
using System;

namespace Scripts.SplitScreen
{
    public class SplitScreenMenu : MonoBehaviour
    {
        [SerializeField] private NicknameSettings nicknameValidator;

        private string playerName;
        private SplitScreenDevice playerDevice;

        public bool AddingPlayer { get; private set; }


        public void StartAddPlayer()
        {
            AddingPlayer = true;
        }

        public void EndAddPlayer()
        {
            SplitScreenManager.AddPlayer(playerName, playerDevice);
            AddingPlayer = false;
        }

        public void CancelAddPlayer()
        {
            AddingPlayer = false;
            playerName = default;
            playerDevice = default;
        }


        public void RemoveLastPlayer()
        {
            SplitScreenManager.RemovePlayer(SplitScreenManager.GetPlayers().Count - 1);
        }

        public void RemoveAll()
        {
            SplitScreenManager.RemoveAllPlayers();
        }


        public void SetNickname(string nick)
        {
            if (nicknameValidator.TryValidateNickname(nick, out string validatedNick))
            {
                playerName = validatedNick;
            }
        }

        public void SetDevice(string device)
        {
            playerDevice = Enum.Parse<SplitScreenDevice>(device);
        }


        public int GetCurrentAddingPlayerIndex() => SplitScreenManager.GetPlayers().Count - 1;
    }
}