using NaughtyAttributes;
using Scripts.Extensions;
using System;
using System.Linq;
using UnityEngine;
using static Scripts.Extensions.RuntimePlatformConverter;

namespace Scripts.UI
{
    [Serializable]
    public class HorizonterPreset
    {
        public string presetName;
        public NameMode nameMode = NameMode.Translate;
        [Space]
        [EnumFlags]
        public RuntimePlatformFlags platforms;
        [EnumFlags]
        public PlatformSpecifies specifies;


        public enum NameMode
        {
            Raw,
            Translate
        }

        public bool IsShow
        {
            get
            {
                var systemSpecifies = ApplicationInfo.GetSpecifies().FlagsToArray();
                var presetSpecifies = specifies.FlagsToArray();

                if (presetSpecifies.Count() > 0)
                {
                    foreach (var specifie in presetSpecifies)
                    {
                        if (!systemSpecifies.Contains(specifie))
                            return false;
                    }
                }

                return platforms.HasFlag(Application.platform.ToFlags());
            }
        }
    }
}