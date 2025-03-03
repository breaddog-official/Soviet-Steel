using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    [ExecuteAlways]
    public class LayoutPrefferedSize : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private LayoutGroup group;
        [Space]
        [SerializeField] private bool overrideX;
        [SerializeField] private bool overrideY;

        private LayoutGroupOptimizator optimizator;


        private void Start()
        {
            optimizator = GetComponent<LayoutGroupOptimizator>();

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();

            if (group == null)
                group = GetComponent<LayoutGroup>();
        }

        private void Update()
        {
            if (rectTransform == null || group == null || !group.enabled)
                return;

            if (!overrideX && !overrideY)
                return;

            Vector2 changedSizeDelta = new();
            changedSizeDelta.x = overrideX ? group.preferredWidth : rectTransform.sizeDelta.x;
            changedSizeDelta.y = overrideY ? group.preferredHeight : rectTransform.sizeDelta.y;

            // If has not changes, return
            if (rectTransform.sizeDelta.Equals(changedSizeDelta))
                return;

            if (optimizator != null && Application.isPlaying)
                optimizator.UpdateGroup();

            rectTransform.sizeDelta = changedSizeDelta;
        }
    }
}
