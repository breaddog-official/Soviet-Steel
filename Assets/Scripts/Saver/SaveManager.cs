using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using Scripts.Extensions;

namespace Scripts.SaveManagement
{
    public enum DataLocation
    {
        PreferDefault,
        PreferPersistent
    }

    public static class SaveManager
    {
        #region Constants

        /// <summary>
        /// Persistent path for player's data
        /// </summary>
        
        // Application.productName needed for platforms like UWP, because their persistentDataPath only has a company in path

        public static string PlayerDataPath => Path.Combine(GetDataPath(DataLocation.PreferPersistent), $"{Application.productName}PlayerData");

        /// <summary>
        /// Path for configs that are updated with the game
        /// </summary>
        public static string ConfigsPath => Path.Combine(GetDataPath(DataLocation.PreferDefault), "Configs");


        public static bool SupportIO => Application.platform.SupportDataPath() || Application.platform.SupportPersistentDataPath();

        #endregion


        #region TrySave

        public static bool TrySave(in string value, string path, Saver saver)
        {
            try
            {
                saver.Save(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> TrySaveAsync(string value, string path, Saver saver)
        {
            try
            {
                await saver.SaveAsync(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region TryLoad

        public static bool TryLoad(string path, Saver saver, out string value)
        {
            try
            {
                value = saver.Load(path);
                return value != null;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);

                value = string.Empty;
                return false;
            }
        }

        #endregion


        #region GetDataPath

        public static string GetDataPath(DataLocation preferLocation = DataLocation.PreferDefault)
        {
            var platform = Application.platform;

            var dataPath = platform.SupportDataPath() ? Application.dataPath : null;
            var persistentPath = platform.SupportPersistentDataPath() ? Application.persistentDataPath : null;


            return preferLocation switch
            {
                DataLocation.PreferPersistent => persistentPath ?? dataPath,
                _ => dataPath ?? persistentPath
            };
        }

        #endregion


        #region SerializeAndSave

        public static bool SerializeAndSave(object value, string path, Saver saver, Serializer serializer)
        {
            try
            {
                string serialized = serializer.Serialize(value);
                return TrySave(serialized, path, saver);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SerializeAndSaveAsync(object value, string path, Saver saver, Serializer serializer)
        {
            try
            {
                string serialized = await serializer.SerializeAsync(value);
                return await TrySaveAsync(serialized, path, saver);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region LoadAndDeserialize

        public static object LoadAndDeserialize(string path, Saver saver, Serializer serializer)
        {
            string loaded = saver.Load(path);
            return serializer.Deserialize(loaded);
        }

        public static async UniTask<object> LoadAndDeserializeAsync(string path, Saver saver, Serializer serializer)
        {
            string loaded = await saver.LoadAsync(path);
            return await serializer.DeserializeAsync(loaded);
        }

        #endregion

        #region LoadAndDeserialize<T>

        public static T LoadAndDeserialize<T>(string path, Saver saver, Serializer serializer)
        {
            string loaded = saver.Load(path);
            return serializer.Deserialize<T>(loaded);
        }

        public static async UniTask<T> LoadAndDeserializeAsync<T>(string path, Saver saver, Serializer serializer)
        {
            string loaded = await saver.LoadAsync(path);
            return await serializer.DeserializeAsync<T>(loaded);
        }

        #endregion

        #region TryLoadAndDeserialize

        public static bool TryLoadAndDeserialize(string path, Saver saver, Serializer serializer, out object value)
        {
            value = default;

            try
            {
                string loaded = saver.Load(path);
                value = serializer.Deserialize(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static bool TryLoadAndDeserialize<T>(string path, Saver saver, Serializer serializer, out T value)
        {
            value = default;

            try
            {
                string loaded = saver.Load(path);
                value = serializer.Deserialize<T>(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion
    }
}
