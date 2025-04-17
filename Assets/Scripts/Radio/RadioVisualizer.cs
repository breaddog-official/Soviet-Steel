using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class RadioVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_Text songText;
    [SerializeField] private string loadingText = "Loading...";
    [Space]
    [SerializeField] private Radio radio;


    private void LateUpdate()
    {
        UpdateSongText();
    }


    [Button] public void Play() => radio.Play().Forget();
    [Button] public void Pause() => radio.Pause();
    [Button] public void Previous() => radio.Previous().Forget();
    [Button] public void Next() => radio.Next().Forget();

    private void UpdateSongText()
    {
        if (songText == null)
            return;

        if (radio.IsLoaded)
        {
            songText.text = radio.GetCurrentSong();
        }
        else
        {
            songText.text = loadingText;
        }
    }
}
