using Cysharp.Threading.Tasks;
using Scripts.Extensions;
using UnityEngine;
using TMPro;

public class LongLoading : MonoBehaviour
{
    public TMP_Text text;

    private static bool isShowed;

    void Awake()
    {
#if YandexGamesPlatform_yg
        if (isShowed)
            text.enabled = false;
        else
        {
            isShowed = true;
        }
#else
        text.enabled = false;
#endif
    }
}
