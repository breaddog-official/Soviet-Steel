using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Scripts.Audio
{
    public class DynamicMusicInstance : MonoBehaviour
    {
        [SerializeField] protected bool activateOnEnable;
        [Space]
        [SerializeField] protected DynamicMusic music;
        [SerializeField] protected List<string> parts;


        private void OnEnable()
        {
            if (activateOnEnable)
                Activate();
        }

        public void Activate()
        {
            if (music == null)
                return;

            music.SetPattern(parts.ToArray());
        }

        public void SetDynamicMusic(DynamicMusic music)
        {
            this.music = music;
        }

#if UNITY_EDITOR

        [Dropdown(nameof(GetPartsNamesEditor)), OnValueChanged(nameof(SetPartEditor))]
        [SerializeField] protected string addPart;

        public const string default_dropdown_item = "Select part";


        public virtual List<string> GetPartsNamesEditor()
        {
            List<string> strings = music != null ? music.GetParts() : new();

            strings.Insert(0, default_dropdown_item);
            return strings;
        }

        public void SetPartEditor()
        {
            if (addPart == default_dropdown_item)
                return;

            parts.Add(addPart);
            addPart = default_dropdown_item;
        }

#endif
    }
}
