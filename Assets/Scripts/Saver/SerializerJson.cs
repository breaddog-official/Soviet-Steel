using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "SerializerJson", menuName = "Scripts/Serializers/SerializerJson")]
    public class SerializerJson : Serializer
    {
        enum NamingStraregy
        {
            [Tooltip("camelCase")]
            CamelCase,
            [Tooltip("kebab-case")]
            KebabCase,
            [Tooltip("snake_case")]
            SnakeCase
        }

        [SerializeField] private Formatting formatting;
        [SerializeField] private NamingStraregy namingStrategy;

        public JsonSerializerSettings SerializerSettings => new()
        {
            Formatting = formatting,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = namingStrategy switch
                {
                    NamingStraregy.CamelCase => new CamelCaseNamingStrategy(),
                    NamingStraregy.KebabCase => new KebabCaseNamingStrategy(),
                    NamingStraregy.SnakeCase => new SnakeCaseNamingStrategy(),
                    _ => throw new NotImplementedException()
                }
            },
        };


        public override string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        public override object Deserialize(string value)
        {
            return JsonConvert.DeserializeObject(value, SerializerSettings);
        }

        public override T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }
    }
}
