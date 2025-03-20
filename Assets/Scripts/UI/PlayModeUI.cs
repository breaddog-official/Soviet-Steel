using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.UI.Tabs;
using UnityEngine;
using UnityEngine.Events;

public class PlayModeUI : TabsTranslater
{
    [SerializeField] protected List<TabPlayMode> playModes;
    [Space]
    [SerializeField] protected Vector2 defaultTabSize;
    [SerializeField] protected float switchDuration = 1f;
    [Space]
    [SerializeField] protected RectTransform playModesPanel;
    [SerializeField] protected float playModesPanelOffset = 400f;
    [SerializeField] protected UnityEvent backAction;



    public override async UniTask VisualizeSwitchTabs(Tab oldTab, Tab newTab, CancellationToken token = default)
    {
        UniTask oldTask = UniTask.CompletedTask;
        UniTask newTask = UniTask.CompletedTask;

        UniTask oldTaskInstance = UniTask.CompletedTask;
        UniTask newTaskInstance = UniTask.CompletedTask;

        if (oldTab != null)
        {
            oldTask = oldTab.rect.DOSizeDelta(defaultTabSize, switchDuration).WithCancellation(token);

            if (oldTab is TabPlayMode oldPlayMode)
            {
                oldTaskInstance = oldPlayMode.instance.Hide(token);
            }
        }

        if (newTab != null)
        {
            newTask = newTab.rect.DOSizeDelta(GetResolutionSize(), switchDuration).WithCancellation(token);

            if (newTab is TabPlayMode newPlayMode)
            {
                newTaskInstance = newPlayMode.instance.Show(token);
            }
        }

        await UniTask.WhenAll(Centralize(newTab, token), newTask, oldTask, newTaskInstance, oldTaskInstance);
    }

    protected virtual async UniTask Centralize(Tab to, CancellationToken token = default)
    {
        Vector2 moveTo = to != null ? new Vector2(playModesPanelOffset * (((playModes.Count - 1) / 2f) - playModes.IndexOf(to as TabPlayMode)), 0f) : Vector2.zero;

        await playModesPanel.DOLocalMove(moveTo, switchDuration).WithCancellation(token);
    }

    protected Vector2 GetResolutionSize() => new(Screen.currentResolution.width, Screen.currentResolution.height);

    public void Back()
    {
        if (GetCurrentTab() != null)
        {
            HideTab();
        }
        else
        {
            backAction?.Invoke();
        }
    }

    protected override IReadOnlyCollection<Tab> GetTabs() => playModes;


    public class TabPlayMode : Tab
    {
        public PlayModeInstanceUI instance;
    }
}
