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


        #region Save

        public static bool Save(in string value, string path)
        {
            try
            {
                if (SupportIO)
                {
                    File.WriteAllText(path, value);
                }
                else
                {
                    PlayerPrefs.SetString(path.GetHashCode().ToString(), value);
                }
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SaveAsync(string value, string path)
        {
            try
            {
                if (SupportIO)
                {
                    await File.WriteAllTextAsync(path, value);
                }
                else
                {
                    PlayerPrefs.SetString(path.GetHashCode().ToString(), value);
                }
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region Load

        public static string Load(string path)
        {
            if (SupportIO)
            {
                return File.ReadAllText(path);
            }
            else
            {
                return PlayerPrefs.GetString(path.GetHashCode().ToString());
            }
        }

        public static async UniTask<string> LoadAsync(string path)
        {
            if (SupportIO)
            {
                return await File.ReadAllTextAsync(path);
            }
            else
            {
                return PlayerPrefs.GetString(path.GetHashCode().ToString());
            }
        }

        public static bool TryLoad(string path, out string value)
        {
            if (SupportIO)
            {
                try
                {
                    value = Load(path);
                    return true;
                }
                catch (Exception exp)
                {
                    Debug.LogException(exp);

                    value = string.Empty;
                    return false;
                }
            }
            else
            {
                value = PlayerPrefs.GetString(path.GetHashCode().ToString(), null);
                return value != null;
            }
        }

        #endregion

        #region Exists

        public static bool Exists(string path)
        {
            if (SupportIO)
            {
                return File.Exists(path);
            }
            else
            {
                return PlayerPrefs.HasKey(path.GetHashCode().ToString());
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

        public static bool SerializeAndSave(object value, string path, Serializer serializer)
        {
            try
            {
                string serialized = serializer.Serialize(value);
                return Save(serialized, path);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SerializeAndSaveAsync(object value, string path, Serializer serializer)
        {
            try
            {
                string serialized = await serializer.SerializeAsync(value);
                return await SaveAsync(serialized, path);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region LoadAndDeserialize

        public static object LoadAndDeserialize(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return serializer.Deserialize(loaded);
        }

        public static async UniTask<object> LoadAndDeserializeAsync(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return await serializer.DeserializeAsync(loaded);
        }

        #endregion

        #region LoadAndDeserialize<T>

        public static T LoadAndDeserialize<T>(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return serializer.Deserialize<T>(loaded);
        }

        public static async UniTask<T> LoadAndDeserializeAsync<T>(string path, Serializer serializer)
        {
            string loaded = Load(path);
            return await serializer.DeserializeAsync<T>(loaded);
        }

        #endregion

        #region TryLoadAndDeserialize

        public static bool TryLoadAndDeserialize(string path, Serializer serializer, out object value)
        {
            value = default;

            try
            {
                string loaded = Load(path);
                value = serializer.Deserialize(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static bool TryLoadAndDeserialize<T>(string path, Serializer serializer, out T value)
        {
            value = default;

            try
            {
                string loaded = Load(path);
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
