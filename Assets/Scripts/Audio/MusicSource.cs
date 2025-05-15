using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Scripts.Audio
{
    public class MusicSource : Music
    {
        [SerializeField] private AudioSource source;
        [Space]
        [SerializeField] private float showDuration = 0.5f;
        [SerializeField] private float hideDuration = 0.5f;

        private float cachedVolume;


        private void Awake()
        {
            cachedVolume = source.volume;
            source.volume = 0f;
        }


        public override async UniTask ShowMusic(CancellationToken token = default)
        {
            source.volume = 0f;
            await source.DOFade(cachedVolume, showDuration).WithCancellation(token);
        }

        public override async UniTask HideMusic(CancellationToken token = default)
        {
            source.volume = cachedVolume;
            await source.DOFade(0f, hideDuration).WithCancellation(token);
        }
    }
}