using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mirror;
using System.Threading;
using Unity.Cinemachine;

namespace Scripts.Extensions
{
    public static class Extensions
    {
        // Global
        #region CheckInitialization
        /// <summary>
        /// If already initialized, returns true, however if not initialized, <br />
        /// returns false and makes the field <see href="isInitialized"/> true.
        /// </summary>
        public static bool CheckInitialization(this ref bool isInitialized)
        {
            if (isInitialized)
                return true;

            isInitialized = true;
            return false;
        }

        #endregion

        #region IncreaseInBounds
        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref int index, Array array) => index.IncreaseInBounds(array.Length);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, Array array) => index.IncreaseInBounds((uint)array.Length);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref int index, int bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;
        }

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static void IncreaseInBounds(this ref uint index, uint bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;
        }
        #endregion

        #region DecreaseInBounds
        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref int index, Array array) => index.DecreaseInBounds(array.Length);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref uint index, Array array) => index.DecreaseInBounds((uint)array.Length);

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref int index, int bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                index--;
            else
                index = bounds - (dontCollideBounds ? 1 : 0);
        }

        /// <summary>
        /// Safely decrements index within array length
        /// </summary>
        public static void DecreaseInBounds(this ref uint index, uint bounds, bool dontCollideBounds = true)
        {
            if (index > 0)
                index--;
            else
                index = bounds - (dontCollideBounds ? 1u : 0u);

        }
        #endregion

        #region FindByType

        /// <summary>
        /// Looking for the first <see href="T"/>
        /// </summary>
        public static T FindByType<T>(this IEnumerable enumerable)
        {
            foreach (var t in enumerable)
            {
                if (t is T result)
                {
                    return result;
                }
            }
            return default;
        }

        /// <summary>
        /// Looking for the first <see href="T"/>
        /// </summary>
        public static object FindByType(this IEnumerable enumerable, Type type)
        {
            foreach (var t in enumerable)
            {
                if (t.GetType() == type)
                {
                    return t;
                }
            }
            return null;
        }

        #endregion

        #region SetAll

        /// <summary>
        /// Sets value to all elements in dictionary
        /// </summary>
        public static void SetAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, params TKey[] except)
        {
            foreach (var pair in dictionary.ToArray())
            {
                if (except.Contains(pair.Key))
                    continue;

                lock (dictionary)
                dictionary[pair.Key] = value;
            }
        }

        #endregion

        #region GetIfNull
        /// <summary>
        /// If null, causes <see href="GetComponent"/>, otherwise does nothing
        /// </summary>
        public static bool GetIfNull<T>(this T value, GameObject gameObject) where T : Component
        {
            ref T valueReference = ref value;
            return value == null && gameObject.TryGetComponent(out valueReference);
        }

        /// <summary>
        /// If null, causes <see href="GetComponent"/>, otherwise does nothing
        /// </summary>
        public static bool GetIfNull<T>(this GameObject gameObject, T value) where T : Component
        {
            ref T valueReference = ref value;
            return value == null && gameObject.TryGetComponent(out valueReference);
        }

        #endregion

        #region IfNotNull & IfNull
        /// <summary>
        /// Invokes an action if not null
        /// </summary>
        public static bool IfNotNull(this object value, Action action)
        {
            if (value != null)
                action?.Invoke();

            return value != null;
        }

        /// <summary>
        /// Invokes an action if not null
        /// </summary>
        public static bool IfNotNull<T>(this T value, Action<T> action)
        {
            if (value != null)
                action?.Invoke(value);

            return value != null;
        }




        /// <summary>
        /// Invokes an action if null
        /// </summary>
        public static bool IfNull(this object value, Action action)
        {
            if (value == null)
                action?.Invoke();

            return value == null;
        }

        #endregion

        #region AddIfNotNull
        /// <summary>
        /// Add value if is not null
        /// </summary>
        public static bool AddIfNotNull<T>(this ICollection<T> collection, T value, bool checkContains = false)
        {
            if (collection != null && value != null)
            {
                if (checkContains && collection.Contains(value))
                    return false;

                collection.Add(value);
            }

            return value != null;
        }

        #endregion

        #region InitializeIfNotNull
        /// <summary>
        /// Invokes <see cref="IInitializable.Initialize"/> if not null
        /// </summary>
        public static bool InitializeIfNotNull<T>(this T value) where T : IInitializable
        {
            //return value.IfNotNull(value.Initialize);
            return value != null ? value.Initialize() : false;
        }
        #endregion

        #region GetAs

        /// <summary>
        /// Finds and returns
        /// </summary>
        public static IEnumerable<T> GetAs<T>(this IEnumerable enumerable) where T : class
        {
            return from object obj in enumerable
                   where obj is T
                   select obj as T;
        }

        #endregion

        #region HasFlag

        /// <summary>
        /// Checks, is 'Enum' has 'flag'
        /// </summary>
        public static bool HasFlag(this int Enum, int flag)
        {
            return (Enum & flag) == flag;
        }

        #endregion

        #region ConvertInput

        /// <summary>
        /// Converts Vector2 input to Vector3
        /// </summary>
        public static Vector3 ConvertInputToVector3(this Vector2 input)
        {
            return new Vector3(input.x, 0.0f, input.y);
        }

        /// <summary>
        /// Converts Vector3 input to Vector2
        /// </summary>
        public static Vector2 ConvertInputToVector2(this Vector3 input)
        {
            return new Vector2(input.x, input.z);
        }

        #endregion

        #region ConvertSecondsToMiliseconds

        /// <summary>
        /// Converts float seconds to int miliseconds
        /// </summary>
        public static int ConvertSecondsToMiliseconds(this float seconds)
        {
            return (int)(seconds * 1000);
        }


        #endregion

        #region Reset Token

        /// <summary>
        /// Cancels and disposes a token
        /// </summary>
        public static void ResetToken(this CancellationTokenSource source)
        {
            if (source != null && source.Token.CanBeCanceled)
                source?.Cancel();

            source?.Dispose();
        }


        #endregion

        #region Vector Max

        /// <summary>
        /// Returns max axis in vector
        /// </summary>
        public static float Max(this Vector3 value)
        {
            Vector3 absValue = value.Abs();
            return ExtendedMath.Max(absValue.x, absValue.y, absValue.z);
        }

        #endregion

        #region Vector bool to integer

        /// <summary>
        /// Converts bools to floats with custom false and true presentation
        /// </summary>
        public static Vector3 ToVector(this GenVector3<bool> value, float falsePresent = 0, float truePresent = 1)
        {
            return new Vector3(ExtendedMath.ToNumber(value.x, falsePresent, truePresent),
                               ExtendedMath.ToNumber(value.y, falsePresent, truePresent),
                               ExtendedMath.ToNumber(value.z, falsePresent, truePresent));
        }

        #endregion

        #region Support paths

        /// <summary>
        /// Is platform supporting dataPath?
        /// </summary>
        public static bool SupportDataPath(this RuntimePlatform value)
        {
            return value is not (RuntimePlatform.IPhonePlayer or RuntimePlatform.Android or RuntimePlatform.WSAPlayerARM or RuntimePlatform.WSAPlayerX64 or RuntimePlatform.WSAPlayerX86);
        }

        /// <summary>
        /// Is platform supporting dataPath?
        /// </summary>
        public static bool SupportPersistentDataPath(this RuntimePlatform value)
        {
            return value is not (RuntimePlatform.tvOS or RuntimePlatform.WebGLPlayer or RuntimePlatform.WindowsEditor or RuntimePlatform.OSXEditor or RuntimePlatform.LinuxEditor);
        }

        #endregion


        // Gameplay
        #region Teleportate

        /// <summary>
        /// Teleportates gameobject via Rigidbody or Transform
        /// </summary>
        [Server]
        public static void Teleportate(this GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            // Try get predicted rigidbody and move them
            if (gameObject.TryGetComponent<PredictedRigidbody>(out var predictedRb))
                predictedRb.predictedRigidbody.Move(position, rotation);

            // Try get rigidbody and move them
            else if (gameObject.TryGetComponent<Rigidbody>(out var rb))
                rb.Move(position, rotation);

            // Otherwise, move via transform
            else
                gameObject.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>
        /// Teleportates gameobject via Rigidbody or Transform. If ignoreRotation is true, gameObject will not be rotated
        /// </summary>
        [Server]
        public static void Teleportate(this GameObject gameObject, Transform point, bool ignoreRotation = false)
            => gameObject.Teleportate(point.position, ignoreRotation ? gameObject.transform.rotation : point.rotation);

        #endregion

        #region DontDestroyOnLoad

        /// <summary>
        /// Sets object DontDestryOnLoad and if needs, moves to the root directory
        /// </summary>
        public static void DontDestroyOnLoad(this GameObject gameObject)
        {
            // Try get predicted rigidbody and move them
            if (gameObject.transform.parent != null)
                gameObject.transform.parent = null;

            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region FindByUid

        /// <summary>
        /// Finds identity by id
        /// </summary>
        public static NetworkIdentity FindByID(this uint ID)
        {
            return NetworkClient.spawned.GetValueOrDefault(ID);
        }

        /// <summary>
        /// Tryes find identity by id
        /// </summary>
        public static bool TryFindByID(this uint ID, out NetworkIdentity identity)
        {
            return NetworkClient.spawned.TryGetValue(ID, out identity);
        }



        /// <summary>
        /// Finds identity and component by id
        /// </summary>
        public static TComponent FindByID<TComponent>(this uint ID) where TComponent : Component
        {
            return FindByID(ID)?.GetComponent<TComponent>();
        }

        /// <summary>
        /// Tryes find identity and component by id
        /// </summary>
        public static bool TryFindByID<TComponent>(this uint ID, out TComponent component) where TComponent : Component
        {
            component = null;
            return TryFindByID(ID, out var identity) && identity.TryGetComponent(out component);
        }

        #endregion

        #region Spawn

        /// <summary>
        /// Same as NetworkServer.Spawn
        /// </summary>
        public static T Spawn<T>(this T component, NetworkConnection ownerConnection = null) where T : Component
        {
            NetworkServer.Spawn(component.gameObject, ownerConnection);
            return component;
        }

        /// <summary>
        /// Same as NetworkServer.Spawn
        /// </summary>
        public static Component Spawn(this Component component, NetworkConnection ownerConnection = null)
        {
            NetworkServer.Spawn(component.gameObject, ownerConnection);
            return component;
        }

        /// <summary>
        /// Same as NetworkServer.Spawn
        /// </summary>
        public static GameObject Spawn(this GameObject obj, NetworkConnection ownerConnection = null)
        {
            NetworkServer.Spawn(obj, ownerConnection);
            return obj;
        }

        #endregion
    }


    public static class ExtendedMath
    {
        #region Max

        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }

        public static float Max(params float[] values)
        {
            return Enumerable.Max(values);
        }

        #endregion

        #region ToInteger

        public static float ToNumber(bool value, float falsePresent = 0, float truePresent = 1)
        {
            return value ? truePresent : falsePresent;
        }

        #endregion
    }
}
