using ArcadeVP;
using Cysharp.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;

public class NicknameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private const string player_prefs_key = "nicknameInputCache";
    private ArcadeVehicleNetwork network;

    private async void Start()
    {
        inputField.text = PlayerPrefs.GetString(player_prefs_key, string.Empty);

        // Player connects not immediatly, so wait ~5 seconds
        for (int i = 0; i < 25; i++)
        {
            if (NetworkClient.localPlayer != null && NetworkClient.localPlayer.TryGetComponent(out network))
            {
                ApplyNick(inputField.text);
                break;
            }
                
            await UniTask.Delay(200);
        }
    }

    public void ApplyNick(string nickname)
    {
        if (network == null && (NetworkClient.localPlayer == null || !NetworkClient.localPlayer.TryGetComponent(out network)))
            return;

        network.SendRequestToNickname(nickname);
        PlayerPrefs.SetString(player_prefs_key, nickname);
    }
}
