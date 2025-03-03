using UnityEngine;
using UnityEngine.Audio;

namespace Scripts.Settings
{
    public class SObserverVolume : SettingHandler<float>
    {
        [Space]
        [SerializeField] protected AudioMixerGroup mixer;
        [SerializeField] protected string volumeParameterName;

        private const float MIN_FLOAT = 0.00001f;
        

        public override void UpdateValue()
        {
            float volume = Setting;
            volume = Mathf.Clamp(volume, MIN_FLOAT, 1f);
            volume = Mathf.Log(volume) * 20f;

            mixer.audioMixer.SetFloat(volumeParameterName, volume);
        }
    }
}
