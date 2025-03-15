using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.UI
{
    [ExecuteAlways, SelectionBase]
    public class Horizonter : MonoBehaviour
    {
        [SerializeField] protected HorizonterElement prefab;
        [SerializeField] protected Transform spawnParent;
        [Space]
        [SerializeField] protected bool disableIfNotShown;
        [ShowIf(nameof(disableIfNotShown)), MinValue(1)]
        [SerializeField] protected int notShownCount = 1;
        [ShowIf(nameof(disableIfNotShown))]
        [SerializeField] protected GameObject disableObject;
        [Space]
        [SerializeField] protected List<HorizonterPreset> presets;
        [SerializeField] protected UnityEvent<int> OnValueChanged;

        protected readonly List<HorizonterElement> elements = new();

        public int Value { get; protected set; }


        private void Start()
        {
            // Auto set variables in editor
            if (Application.isEditor)
            {
                if (disableObject == null)
                    disableObject = gameObject;

                if (spawnParent == null)
                    spawnParent = transform;
            }

            // Remove trash
            ClearElements();
            UpdateElements();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                try
                {
                    UpdateElements();
                }
                catch (System.Exception exp)
                {
                    Debug.LogException(exp);
                    ClearElements();
                }
            }
        }

        protected virtual void ClearElements()
        {
            elements.Clear();

            foreach (Transform child in spawnParent)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        protected virtual void UpdateElements()
        {
            // Remove trash (like elements spawned in editor)
            foreach (Transform child in spawnParent.GetComponentsInChildren<Transform>())
            {
                if (child == spawnParent)
                    continue;

                if (child.TryGetComponent<HorizonterElement>(out var element) && elements.Contains(element))
                {
                    if (presets.Contains(element.GetPreset()))
                        continue;
                    else
                        elements.Remove(element);
                }

                DestroyImmediate(child.gameObject);
            }

            if (disableIfNotShown && presets.Where(p => p.IsShow).Count() <= notShownCount && Application.isPlaying)
            {
                disableObject.SetActive(false);
                return;
            }
            
            Fallback();

            // If spawned more then needed, reduce spawned
            if (elements.Count > presets.Count)
            {
                var last = elements.TakeLast(elements.Count - presets.Count);

                foreach (var element in last)
                {
                    elements.Remove(element);
                    if (element.gameObject != null)
                        DestroyImmediate(element.gameObject);
                }
            }
            

            // Spawn or update elements
            for (int i = 0; i < presets.Count; i++)
            {
                var preset = presets[i];

                if (elements.Count < presets.Count || elements[i] == null)
                {
                    var spawned = Instantiate(prefab, spawnParent);
                    spawned.transform.SetAsLastSibling();
                    spawned.Initialze(i, this);

                    if (elements.Count < presets.Count)
                        elements.Add(spawned);
                    else
                        elements[i] = spawned;
                }
                else
                {
                    elements[i].SetPreset(i);
                }
            }

            UpdateVisuals();
        }


        public void SetValue(int presetIndex)
        {
            Value = presetIndex;

            UpdateVisuals();
        }

        public void Select(int presetIndex)
        {
            Value = presetIndex;

            OnValueChanged?.Invoke(presetIndex);

            UpdateVisuals();
        }



        protected void Fallback()
        {
            if (!presets[Value].IsShow)
            {
                for (int i = Value; i >= 0; i--)
                {
                    if (presets[i].IsShow)
                    {
                        Select(i);
                        return;
                    }
                }
            }
        }

        protected void UpdateVisuals()
        {
            foreach (var element in elements)
            {
                if (element.Preset == Value)
                    element.SetVisualSelect(true);
                else
                    element.SetVisualSelect(false);
            }
        }

        public IReadOnlyList<HorizonterPreset> GetPresets() => presets;
    }
}