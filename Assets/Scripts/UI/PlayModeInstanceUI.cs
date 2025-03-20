using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayModeInstanceUI : MonoBehaviour
{
    [SerializeField] protected CanvasGroup group;
    [Space]
    [SerializeField] protected float duration = 1f;
    [SerializeField] protected Ease ease = Ease.Linear;


    public async UniTask Show(CancellationToken token = default)
    {
        group.gameObject.SetActive(true);

        await group.DOFade(1f, duration).SetEase(ease)
                                        .WithCancellation(token);
    }

    public async UniTask Hide(CancellationToken token = default)
    {
        await group.DOFade(0f, duration).SetEase(ease)
                                        .WithCancellation(token);

        group.gameObject.SetActive(false);
    }
}
