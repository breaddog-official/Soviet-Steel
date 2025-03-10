using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Scripts.UI.Tabs
{
    public class TabsTranslaterInterpolation : TabsTranslater
    {
        [SerializeField] private TabInterpolate[] tabs;
        [SerializeField] private bool fadeTogether;

        public override async UniTask VisualizeSwitchTabs(Tab oldTab, Tab newTab, CancellationToken token = default)
        {
            UniTask oldTabFade = UniTask.CompletedTask;
            UniTask newTabFade = UniTask.CompletedTask;

            if (oldTab != null && oldTab is TabInterpolate doOldTab)
            {
                oldTabFade = oldTab.canvasGroup.DOFade(0.0f, doOldTab.fadeDuration)
                                    .SetEase(doOldTab.ease)
                                    .OnComplete(() => doOldTab.canvasGroup.gameObject.SetActive(false))
                                    .WithCancellation(token);


                if (!fadeTogether)
                {
                    await oldTabFade;
                }
            }

            if (newTab != null && newTab is TabInterpolate doNewTab)
            {
                doNewTab.canvasGroup.gameObject.SetActive(true);

                newTabFade = newTab.canvasGroup.DOFade(1.0f, doNewTab.showDuration)
                                    .SetEase(doNewTab.ease)
                                    .WithCancellation(token);

                if (!fadeTogether)
                {
                    await newTabFade;
                }
            }

            if (fadeTogether)
            {
                await UniTask.WhenAll(oldTabFade, newTabFade);
            }
        }

        protected override IReadOnlyCollection<Tab> GetTabs() => tabs;

        [Serializable]
        public class TabInterpolate : Tab
        {
            public float fadeDuration = 1.0f;
            public float showDuration = 1.0f;
            public Ease ease = Ease.Linear;
        }
    }
}
