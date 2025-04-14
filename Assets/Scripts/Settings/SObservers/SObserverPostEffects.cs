using System;
using NaughtyAttributes;
using UnityEngine;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverPostEffects : SettingHandler<ScreenPostEffects>
    {
        [Space]
        [SerializeField] protected CRTPostEffecter retroEffecter;


        public override void UpdateValue()
        {
            retroEffecter.enabled = Setting == ScreenPostEffects.Retro;
        }
    }
}
