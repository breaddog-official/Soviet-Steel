using System.Collections.Generic;
using Scripts.Settings;
using TMPro;
using Unity.Burst;
using UnityEngine;

namespace Scripts.UI
{
    [BurstCompile]
    public class ResolutionSettingDropdown : DropdownInitializer
    {
        public override void InitializeDropdown()
        {
            base.InitializeDropdown();

            List<string> options = new(Screen.resolutions.Length);
            foreach (var resolution in Screen.resolutions)
            {
                options.Add($"{resolution.width}x{resolution.height}");
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(options);
        }
    }
}

