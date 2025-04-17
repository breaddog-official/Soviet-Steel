using NaughtyAttributes;
using Scripts.Audio;
using UnityEngine;
using UnityEngine.Audio;
using static Scripts.Settings.Settings;

namespace Scripts.Settings
{
    public class SObserverPostEffectsAudio : SettingHandler<ScreenPostEffects>
    {
        [Space]
        [SerializeField] protected AudioType type;
        [ShowIf(nameof(type), AudioType.AudioSource)]
        [SerializeField] protected AudioSource source;
        [ShowIf(nameof(type), AudioType.DynamicMusic)]
        [SerializeField] protected DynamicMusic dynamicMusic;
        [Space]
        [SerializeField] protected AudioMixerGroup defaultMixer;
        [SerializeField] protected AudioMixerGroup retroMixer;

        protected enum AudioType
        {
            AudioSource,
            DynamicMusic
        }


        public override void UpdateValue()
        {
            var mixer = Setting == ScreenPostEffects.Retro ? retroMixer : defaultMixer;

            if (type == AudioType.AudioSource)
                source.outputAudioMixerGroup = mixer;

            else if (type == AudioType.DynamicMusic)
            {
                foreach (var source in dynamicMusic.GetSources())
                    source.Value.outputAudioMixerGroup = mixer;
            }
        }
    }
}
