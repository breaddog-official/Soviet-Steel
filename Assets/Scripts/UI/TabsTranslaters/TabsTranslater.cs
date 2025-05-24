using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Scripts.Extensions;
using NaughtyAttributes;

namespace Scripts.UI.Tabs
{
    public abstract class TabsTranslater : MonoBehaviour
    {
        [SerializeField] private bool showInitialOnStart = true;
        [ShowIf(EConditionOperator.And, nameof(CanGetTabsGroups), nameof(showInitialOnStart))]
        [SerializeField, Dropdown(nameof(GetTabsRects))] private RectTransform initialTab;

        private CancellationTokenSource cancallationToken = new();
        private bool switchingTab;
        private Tab currentTab;


        protected virtual void Awake()
        {
            currentTab = FindTab(initialTab);
        }

        protected virtual void Start()
        {
            if (showInitialOnStart)
            {
                SwitchTabAsync(initialTab).Forget();
            }
        }


        public virtual async UniTask SwitchTabAsync(RectTransform tabGroup, CancellationToken token = default) => await SwitchTabAsync(FindTab(tabGroup), token);

        public virtual async UniTask SwitchTabAsync(Tab tab, CancellationToken token = default) => await SwitchTab(currentTab, tab, token);
        public virtual async UniTask ShowTabAsync(CancellationToken token = default) => await SwitchTab(null, currentTab, token);
        public virtual async UniTask HideTabAsync(CancellationToken token = default) => await SwitchTab(currentTab, null, token);



        public virtual void SwitchTab(RectTransform tabGroup) => SwitchTab(FindTab(tabGroup));

        public virtual void SwitchTab(Tab tab) => SwitchTab(currentTab, tab).Forget();
        public virtual void ShowTab() => SwitchTab(null, currentTab).Forget();
        public virtual void HideTab() => SwitchTab(currentTab, null).Forget();



        public virtual async UniTask SwitchTab(Tab from, Tab to, CancellationToken token = default, bool withSetCurrentTab = true)
        {
            if (switchingTab)
                return;

            switchingTab = true;

            cancallationToken?.ResetToken();
            cancallationToken = new();

            if (token == default)
                token = cancallationToken.Token;

            await VisualizeSwitchTabs(from, to, token);

            if (withSetCurrentTab)
                currentTab = to;

            switchingTab = false;
        }

        public Tab GetCurrentTab() => currentTab;



        public virtual void CancelSwitching()
        {
            switchingTab = false;
            cancallationToken?.ResetToken();
            cancallationToken = new();
        }

        public virtual Tab FindTab(RectTransform rect)
        {
            return GetTabs().Where(t => t.rect == rect).FirstOrDefault();
        }

        private void OnDestroy()
        {
            CancelSwitching();
        }


        public abstract UniTask VisualizeSwitchTabs(Tab oldTab, Tab newTab, CancellationToken token = default);

        protected abstract IReadOnlyCollection<Tab> GetTabs();

        protected virtual RectTransform[] GetTabsRects()
        {
            return GetTabs().Select(tab => tab.rect).ToArray();
        }

        protected virtual bool CanGetTabsGroups()
        {
            IReadOnlyCollection<Tab> tabs = GetTabs();
            return tabs != null && tabs.Count > 0;
        }
    }
}
