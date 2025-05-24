using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.SaveManagement
{
    public abstract class Serializer : ScriptableObject
    {
        public abstract string Serialize(object value);
        public abstract object Deserialize(string value);

        public virtual T Deserialize<T>(string value) => (T)Deserialize(value);

        public virtual bool IsAvailable() => true;

#pragma warning disable CS1998 

        public async virtual UniTask<string> SerializeAsync(object value, CancellationToken token = default) => Serialize(value);
        public async virtual UniTask<object> DeserializeAsync(string value, CancellationToken token = default) => Deserialize(value);
        public async virtual UniTask<T> DeserializeAsync<T>(string value, CancellationToken token = default) => Deserialize<T>(value);

#pragma warning restore CS1998
    }
}
