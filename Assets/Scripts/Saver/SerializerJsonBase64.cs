using System;
using System.Text;
using UnityEngine;

namespace Scripts.SaveManagement
{
    [CreateAssetMenu(fileName = "SerializerJsonBase64", menuName = "Scripts/Serializers/SerializerJsonBase64")]
    public class SerializerJsonBase64 : SerializerJson
    {
        public override string Serialize(object value)
        {
            string json = base.Serialize(value);

            byte[] bytesToEncode = Encoding.UTF8.GetBytes(json);
            string encodedText = Convert.ToBase64String(bytesToEncode);

            return encodedText;
        }

        public override object Deserialize(string value)
        {
            byte[] decodedBytes = Convert.FromBase64String(value);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);

            return base.Deserialize(decodedText);
        }

        public override T Deserialize<T>(string value)
        {
            byte[] decodedBytes = Convert.FromBase64String(value);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);

            return base.Deserialize<T>(decodedText);
        }

        public override bool IsAvailable() => true;
    }
}
