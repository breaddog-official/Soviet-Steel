using System.Collections.Generic;
using Unity.Burst;
using Scripts.TranslateManagement;
using System.Linq;

namespace Scripts.UI
{
    [BurstCompile]
    public class LanguageSettingDropdown : DropdownInitializer
    {
        public override void InitializeDropdown()
        {
            base.InitializeDropdown();

            // Select all languages as list of strings
            List<string> languages = new(from t in TranslationConfig.Instance.GetTranslations()
                                        select t.Language.ToString());

            dropdown.ClearOptions();
            dropdown.AddOptions(languages);
        }
    }
}

