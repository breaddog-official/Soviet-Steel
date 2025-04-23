using ArcadeVP;
using Cysharp.Threading.Tasks;
using Mirror;
using Scripts.SaveManagement;
using TMPro;
using UnityEngine;

public class NicknameUI : MonoBehaviour
{
    [SerializeField] private Saver saver;
    [SerializeField] private TMP_InputField inputField;

    private const string save_key = "nicknameInputCache";
    private ArcadeVehicleNetwork network;

    private async void Start()
    {
        inputField.text = saver.Exists(save_key) ? await saver.LoadAsync(save_key) : DefaultNick;

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
        saver.Save(save_key, nickname);
    }

    private string DefaultNick =>
#if YandexGamesPlatform_yg
        YG.YG2.player.auth ? YG.YG2.player.name : string.Empty;
#else
        string.Empty;
#endif
}
