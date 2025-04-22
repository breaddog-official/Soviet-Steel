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

        private List<ApplicationLanguage> languages;


        protected override void Awake()
        {
            languages = TranslationConfig.Instance.GetTranslations().Select(t => t.Language).ToList();

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
