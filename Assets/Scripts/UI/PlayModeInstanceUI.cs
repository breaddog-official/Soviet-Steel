using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.UI.Tabs;
using UnityEngine;

public class PlayModeInstanceUI : MonoBehaviour
{
    [SerializeField] protected RectTransform buttonRect;
    [SerializeField] protected RectTransform interactableRect;
    [Space]
    [SerializeField] protected TabsTranslater translater;


    public async UniTask Show(CancellationToken token = default)
    {
        await translater.SwitchTabAsync(interactableRect, token);
    }

    public async UniTask Hide(CancellationToken token = default)
    {
        await translater.SwitchTabAsync(buttonRect, token);
    }
}
