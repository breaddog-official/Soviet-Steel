using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class SelectMapInstanceUI : MonoBehaviour
    {
        [SerializeField] protected TMP_Text nameText;
        [SerializeField] protected TMP_Text descriptionText;
        [SerializeField] protected RawImage iconImage;
        [Space]
        [SerializeField] protected GameObject selector;

        protected IMap map;
        protected Action<IMap> selectAction;



        public void Initialize(IMap map, Action<IMap> onSelect = null)
        {
            this.map = map;
            this.selectAction = onSelect;

            nameText.SetText(map.Name);
            descriptionText.SetText(map.Description);

            iconImage.texture = map.Icon;
        }

        public void SelectThis()
        {
            selectAction?.Invoke(map);
        }



        public void SetSelectState(bool state)
        {
            selector.SetActive(state);
        }
    }
}