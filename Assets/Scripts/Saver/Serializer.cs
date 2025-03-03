using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scripts.SaveManagement
{
    public abstract class Serializer : ScriptableObject
    {
        public abstract string Serialize(object value);
        public abstract object Deserialize(string value);

        public virtual T Deserialize<T>(string value) => (T)Deserialize(value);

#pragma warning disable CS1998 

        public async virtual UniTask<string> SerializeAsync(object value) => Serialize(value);
        public async virtual UniTask<object> DeserializeAsync(string value) => Deserialize(value);
        public async virtual UniTask<T> DeserializeAsync<T>(string value) => Deserialize<T>(value);

#pragma warning restore CS1998
    }
}
