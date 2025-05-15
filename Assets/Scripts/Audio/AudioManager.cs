using Scripts.Extensions;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Scripts.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [Header("Buttons Sounds")]
        [SerializeField] private AudioResource disabledClip;
        [SerializeField] private AudioResource highlightedClip;
        [SerializeField] private AudioResource unHighlightedClip;
        [SerializeField] private AudioResource selectedClip;
        [SerializeField] private AudioResource deselectedClip;
        [SerializeField] private AudioResource buttonDownClip;
        [SerializeField] private AudioResource buttonUpClip;

        public static AudioManager Instance { get; private set; }
        private readonly static HashSet<Music> musics = new();


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                gameObject.DontDestroyOnLoad();
            }
            else
            {
                Destroy(gameObject);
            }
        }


        #region Buttons

        public static void PlayDisabled()
        {
            TryPlayClip(Instance.disabledClip);
        }

        public static void PlayHighlighted()
        {
            TryPlayClip(Instance.highlightedClip);
        }

        public static void PlayUnHighlighted()
        {
            TryPlayClip(Instance.unHighlightedClip);
        }

        public static void PlaySelected()
        {
            TryPlayClip(Instance.selectedClip);
        }

        public static void PlayDeselected()
        {
            TryPlayClip(Instance.deselectedClip);
        }

        public static void PlayButtonDown()
        {
            TryPlayClip(Instance.buttonDownClip);
        }

        public static void PlayButtonUp()
        {
            TryPlayClip(Instance.buttonUpClip);
        }

        #endregion


        private static bool TryPlayClip(AudioResource clip)
        {
            if (clip != null && Instance.audioSource != null)
            {
                Instance.audioSource.resource = clip;
                Instance.audioSource.Play();

                return true;
            }

            return false;
        }


        public static void AddMusic(Music music)
        {
            musics.Add(music);
        }

        public static void RemoveMusic(Music music)
        {
            musics.Remove(music);
        }

        public static async UniTask ShowMusics(CancellationToken token = default)
        {
            foreach (var music in musics)
            {
                await music.ShowMusic(token);
            }
        }

        public static async UniTask HideMusics(CancellationToken token = default)
        {
            foreach (var music in musics)
            {
                await music.HideMusic(token);
            }
        }

        public static IReadOnlyCollection<Music> GetMusics() => musics;
    }
}