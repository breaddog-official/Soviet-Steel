using TMPro;
using UnityEngine;

namespace Scripts.UI
{
    public class HorizonterElement : MonoBehaviour
    {
        [SerializeField] protected TMP_Text textName;
        [SerializeField] protected GameObject selector;

        public Horizonter Horizonter { get; protected set; }
        public int Preset { get; protected set; }




        public HorizonterPreset GetPreset() => Horizonter.GetPresets()[Preset];


        protected virtual void OnEnable()
        {
            UpdateSelection();
        }




        private void UpdateSelection()
        {
            if (Horizonter != null)
            {
                if (Horizonter.Value == Preset)
                {
                    SetVisualSelect(true);
                }
                else
                {
                    SetVisualSelect(false);
                }
            }
        }




        public virtual void Initialze(int preset, Horizonter horizonter = null)
        {
            Horizonter = horizonter;
            UpdateSelection();

            SetPreset(preset);
        }




        public virtual void SetPreset(int preset)
        {
            Preset = preset;
            UpdatePreset();
        }

        public virtual void UpdatePreset()
        {
            textName.SetText(GetPreset().presetName);
        }




        public virtual void Select()
        {
            if (Horizonter != null)
                Horizonter.Select(Preset);
        }

        public virtual void Deselect()
        {
            SetVisualSelect(false);
        }


        public virtual void SetVisualSelect(bool state)
        {
            selector.SetActive(state);
        }
    }
}