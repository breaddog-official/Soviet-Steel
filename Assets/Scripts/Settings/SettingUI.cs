using NaughtyAttributes;
using Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Settings
{
    public sealed class SettingUI : SettingHandler
    {
        enum SettingType
        {
            Toggle,
            Slider,
            Dropdown,
            InputField,
            Horizonter
        }

        [Space]
        [SerializeField] private SettingType settingType;

        [ShowIf(nameof(settingType), SettingType.Toggle)]
        [SerializeField] private Toggle toggle;

        [ShowIf(nameof(settingType), SettingType.Slider)]
        [SerializeField] private Slider slider;

        [ShowIf(nameof(settingType), SettingType.Dropdown)]
        [SerializeField] private TMP_Dropdown dropdown;

        [ShowIf(nameof(settingType), SettingType.InputField)]
        [SerializeField] private TMP_InputField inputField;

        [ShowIf(nameof(settingType), SettingType.Horizonter)]
        [SerializeField] private Horizonter horizonter;


        public override void UpdateValue()
        {
            switch (settingType)
            {
                case SettingType.Toggle:
                    toggle.isOn = (bool)Setting;
                    break;

                case SettingType.Slider:
                    slider.value = (float)Setting;
                    break;

                case SettingType.Dropdown:
                    dropdown.value = (int)Setting;
                    break;

                case SettingType.InputField:
                    inputField.text = (string)Setting;
                    break;

                case SettingType.Horizonter:
                    horizonter.SetValue((int)Setting);
                    break;
            }
        }


        public void SetInt(int value) => SetSetting(value);
        public void SetFloat(float value) => SetSetting(value);
        public void SetString(string value) => SetSetting(value);
        public void SetBool(bool value) => SetSetting(value);
    }
}
