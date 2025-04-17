using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Scripts.Audio
{
    public class DynamicMusic : MonoBehaviour
    {
        [Range(0f, 1f)]
        [SerializeField] protected float volume;
        [SerializeField] protected AudioMixerGroup mixer;
        [Space]
        [SerializeField] protected MusicPart[] parts;

        protected readonly Dictionary<MusicPart, AudioSource> sources = new();



        private void Awake()
        {
            foreach (var part in parts)
            {
                var source = gameObject.AddComponent<AudioSource>();

                source.resource = part.clip;
                source.outputAudioMixerGroup = mixer;
                source.volume = 0f;
                source.loop = true;
                source.Play();

                sources.Add(part, source);
            }
        }

        public void SetPattern(params string[] parts)
        {
            var patternParts = this.parts.Where(p => parts.Contains(p.name)).Select(p => sources[p]);
            SmoothPlay(default, patternParts.ToArray()).Forget();
        }

        public List<string> GetParts()
        {
            return parts.Select(p => p.name).ToList();
        }

        protected async UniTask SmoothPlay(CancellationToken token = default, params AudioSource[] sources)
        {
            HashSet<UniTask> tasks = new();

            foreach (var source in this.sources)
            {
                float endValue = sources.Contains(source.Value) ? volume : 0f;
                tasks.Add(source.Value.DOFade(endValue, 0.5f).WithCancellation(token));
            }

            await UniTask.WhenAll(tasks);
        }


        public IReadOnlyDictionary<MusicPart, AudioSource> GetSources() => sources;

        [Serializable]
        public class MusicPart
        {
            public string name;
            public AudioResource clip;
        }
    }
}
