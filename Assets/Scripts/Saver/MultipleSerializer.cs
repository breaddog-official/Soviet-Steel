using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "MultipleSerializer", menuName = "Scripts/Serializers/MultipleSerializer")]
    public class MultipleSerializer : Serializer
    {
        public Serializer[] serializers;

        public Serializer Serializer
        {
            get
            {
                foreach (var saver in serializers)
                    if (saver.IsAvailable())
                        return saver;

                return null;
            }
        }

        public override string Serialize(object value) => Serializer.Serialize(value);
        public override object Deserialize(string value) => Serializer.Deserialize(value);
        public override T Deserialize<T>(string value) => Serializer.Deserialize<T>(value);

        public override UniTask<string> SerializeAsync(object value, CancellationToken token = default) => Serializer.SerializeAsync(value, token);
        public override UniTask<object> DeserializeAsync(string value, CancellationToken token = default) => Serializer.DeserializeAsync(value, token);
        public override UniTask<T> DeserializeAsync<T>(string value, CancellationToken token = default) => Serializer.DeserializeAsync<T>(value, token);

        public override bool IsAvailable() => Serializer != null;
    }
}
