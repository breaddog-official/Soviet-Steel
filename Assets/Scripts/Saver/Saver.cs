using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.SaveManagement
{
    public abstract class Saver : ScriptableObject
    {
        public abstract void Save(string path, string value);
        public abstract string Load(string path);
        public abstract bool Exists(string path);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask SaveAsync(string path, string value, CancellationToken token = default) => Save(path, value);
        public async virtual UniTask<string> LoadAsync(string path, CancellationToken token = default) => Load(path);

#pragma warning restore CS1998
    }
}
