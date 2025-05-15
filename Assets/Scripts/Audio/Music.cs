using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.Audio
{
    public abstract class Music : MonoBehaviour
    {
        public abstract UniTask ShowMusic(CancellationToken token = default);
        public abstract UniTask HideMusic(CancellationToken token = default);


        protected virtual void OnEnable()
        {
            AudioManager.AddMusic(this);
        }

        protected virtual void OnDisable()
        {
            AudioManager.RemoveMusic(this);
        }
    }
}