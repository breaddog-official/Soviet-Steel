using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

namespace Scripts.Audio
{
    public class DynamicMusic : Music
    {
        [Range(0f, 1f)]
        [SerializeField] protected float volume;
        [SerializeField] protected AudioMixerGroup mixer;
        [Space]
        [SerializeField] private float showDuration = 0.5f;
        [SerializeField] private float hideDuration = 0.5f;
        [Space]
        [SerializeField] protected MusicPart[] parts;

        protected readonly Dictionary<MusicPart, AudioSource> sources = new();
        protected readonly HashSet<UniTask> tasksCache = new();

        protected float volumeMultiplier = 1f;
        protected float Volume => volume * volumeMultiplier;


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
            UpdateVolumes(default, patternParts.ToArray()).Forget();
        }

        public List<string> GetParts()
        {
            return parts.Select(p => p.name).ToList();
        }

        protected async UniTask UpdateVolumes(CancellationToken token = default, params AudioSource[] sources)
        {
            tasksCache.Clear();

            foreach (var source in this.sources)
            {
                bool contains = sources.Contains(source.Value);
                float endValue = contains ? Volume : 0f;
                float duration = contains ? showDuration : hideDuration;

                source.Key.isPlaying = contains;

                tasksCache.Add(source.Value.DOFade(endValue, duration).WithCancellation(token));
            }

            await UniTask.WhenAll(tasksCache);
        }



        public override async UniTask ShowMusic(CancellationToken token = default)
        {
            volumeMultiplier = 1f;
            await UpdateVolumes(token, sources.Where(s => s.Key.isPlaying).Select(s => s.Value).ToArray());
        }

        public override async UniTask HideMusic(CancellationToken token = default)
        {
            volumeMultiplier = 0f;
            await UpdateVolumes(token, sources.Where(s => s.Key.isPlaying).Select(s => s.Value).ToArray());
        }


        public IReadOnlyDictionary<MusicPart, AudioSource> GetSources() => sources;

        [Serializable]
        public class MusicPart
        {
            public string name;
            public AudioResource clip;
            [HideInInspector]
            public bool isPlaying;
        }
    }
}
