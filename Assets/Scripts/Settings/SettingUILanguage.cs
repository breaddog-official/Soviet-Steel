using Scripts.TranslateManagement;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Scripts.Settings
{
    public sealed class SettingUILanguage : SettingHandler<ApplicationLanguage>
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private DropdownInitializer initializer;

        private List<ApplicationLanguage> languages;


        protected override void Awake()
        {
            languages = TranslationConfig.Instance.GetTranslations().Select(t => t.Language).ToList();

            if (initializer != null)
                initializer.InitializeDropdown();

            base.Awake();
        }

        public override void UpdateValue()
        {
            dropdown.value = languages.IndexOf(Setting);
        }


        public void SetConvertedInt(int value)
        {
            SetSetting(languages[value]);
        }
    }
}
