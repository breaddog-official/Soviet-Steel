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


        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static int IncreaseInBoundsReturn(this int index, Array array) => index.IncreaseInBoundsReturn(array.Length);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static uint IncreaseInBoundsReturn(this uint index, Array array) => index.IncreaseInBoundsReturn((uint)array.Length);

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static int IncreaseInBoundsReturn(this int index, int bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;

            return index;
        }

        /// <summary>
        /// Safely increments index within array length
        /// </summary>
        public static uint IncreaseInBoundsReturn(this uint index, uint bounds, bool dontCollideBounds = true)
        {
            index++;

            if (index > bounds - (dontCollideBounds ? 1 : 0))
                index = 0;

            return index;
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

        #region FlagsToArray

        public static IEnumerable<T> FlagsToArray<T>(this T @enum) where T : struct
        {
            var array = @enum.ToString()
                .Split(new string[] { ", " }, StringSplitOptions.None)
                .Select(i => Enum.Parse<T>(i));

            var first = array.First().ToString();
            var names = Enum.GetNames(typeof(T));

            if (first == names[0])
                array = array.Skip(1);

            else if (first == "-1")
            {
                array = names.Select(i => Enum.Parse<T>(i));
                array = array.Skip(1);
            }

            return array;
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

    #region RuntimePlatformFlags

    [Flags]
    public enum RuntimePlatformFlags
    {
        None = 0,
        OSXEditor = 1 << 0,
        OSXPlayer = 1 << 1,
        WindowsPlayer = 1 << 2,
        WindowsEditor = 1 << 3,
        IPhonePlayer = 1 << 4,
        Android = 1 << 5,
        LinuxPlayer = 1 << 6,
        LinuxEditor = 1 << 7,
        WebGLPlayer = 1 << 8,
        WSAPlayerX86 = 1 << 9,
        WSAPlayerX64 = 1 << 10,
        WSAPlayerARM = 1 << 11,
        PS4 = 1 << 12,
        XboxOne = 1 << 13,
        tvOS = 1 << 14,
        Switch = 1 << 15,
        PS5 = 1 << 16,
        LinuxServer = 1 << 17,
        WindowsServer = 1 << 18,
        OSXServer = 1 << 19,
        VisionOS = 1 << 20
    }

    public static class RuntimePlatformConverter
    {
        public static RuntimePlatformFlags ToFlags(this RuntimePlatform platform)
        {
            return platform switch
            {
                RuntimePlatform.OSXEditor => RuntimePlatformFlags.OSXEditor,
                RuntimePlatform.OSXPlayer => RuntimePlatformFlags.OSXPlayer,
                RuntimePlatform.WindowsPlayer => RuntimePlatformFlags.WindowsPlayer,
                RuntimePlatform.WindowsEditor => RuntimePlatformFlags.WindowsEditor,
                RuntimePlatform.IPhonePlayer => RuntimePlatformFlags.IPhonePlayer,
                RuntimePlatform.Android => RuntimePlatformFlags.Android,
                RuntimePlatform.LinuxPlayer => RuntimePlatformFlags.LinuxPlayer,
                RuntimePlatform.LinuxEditor => RuntimePlatformFlags.LinuxEditor,
                RuntimePlatform.WebGLPlayer => RuntimePlatformFlags.WebGLPlayer,
                RuntimePlatform.WSAPlayerX86 => RuntimePlatformFlags.WSAPlayerX86,
                RuntimePlatform.WSAPlayerX64 => RuntimePlatformFlags.WSAPlayerX64,
                RuntimePlatform.WSAPlayerARM => RuntimePlatformFlags.WSAPlayerARM,
                RuntimePlatform.PS4 => RuntimePlatformFlags.PS4,
                RuntimePlatform.XboxOne => RuntimePlatformFlags.XboxOne,
                RuntimePlatform.tvOS => RuntimePlatformFlags.tvOS,
                RuntimePlatform.Switch => RuntimePlatformFlags.Switch,
                RuntimePlatform.PS5 => RuntimePlatformFlags.PS5,
                RuntimePlatform.LinuxServer => RuntimePlatformFlags.LinuxServer,
                RuntimePlatform.WindowsServer => RuntimePlatformFlags.WindowsServer,
                RuntimePlatform.OSXServer => RuntimePlatformFlags.OSXServer,
                RuntimePlatform.VisionOS => RuntimePlatformFlags.VisionOS,
                _ => RuntimePlatformFlags.None
            };
        }

        public static RuntimePlatform ToRuntimePlatform(this RuntimePlatformFlags flags)
        {
            return flags switch
            {
                RuntimePlatformFlags.OSXEditor => RuntimePlatform.OSXEditor,
                RuntimePlatformFlags.OSXPlayer => RuntimePlatform.OSXPlayer,
                RuntimePlatformFlags.WindowsPlayer => RuntimePlatform.WindowsPlayer,
                RuntimePlatformFlags.WindowsEditor => RuntimePlatform.WindowsEditor,
                RuntimePlatformFlags.IPhonePlayer => RuntimePlatform.IPhonePlayer,
                RuntimePlatformFlags.Android => RuntimePlatform.Android,
                RuntimePlatformFlags.LinuxPlayer => RuntimePlatform.LinuxPlayer,
                RuntimePlatformFlags.LinuxEditor => RuntimePlatform.LinuxEditor,
                RuntimePlatformFlags.WebGLPlayer => RuntimePlatform.WebGLPlayer,
                RuntimePlatformFlags.WSAPlayerX86 => RuntimePlatform.WSAPlayerX86,
                RuntimePlatformFlags.WSAPlayerX64 => RuntimePlatform.WSAPlayerX64,
                RuntimePlatformFlags.WSAPlayerARM => RuntimePlatform.WSAPlayerARM,
                RuntimePlatformFlags.PS4 => RuntimePlatform.PS4,
                RuntimePlatformFlags.XboxOne => RuntimePlatform.XboxOne,
                RuntimePlatformFlags.tvOS => RuntimePlatform.tvOS,
                RuntimePlatformFlags.Switch => RuntimePlatform.Switch,
                RuntimePlatformFlags.PS5 => RuntimePlatform.PS5,
                RuntimePlatformFlags.LinuxServer => RuntimePlatform.LinuxServer,
                RuntimePlatformFlags.WindowsServer => RuntimePlatform.WindowsServer,
                RuntimePlatformFlags.OSXServer => RuntimePlatform.OSXServer,
                RuntimePlatformFlags.VisionOS => RuntimePlatform.VisionOS,
                _ => throw new ArgumentException("Invalid platform flag")
            };
        }

        #endregion

    #region ApplicationInfo

        [Flags]
        public enum PlatformSpecifies
        {
            None = 0,
            ComputeShaders = 1 << 0,
            ShaderLevel_3 = 1 << 1,
            ShaderLevel_4 = 1 << 2,
            ShaderLevel_4_5 = 1 << 3,
            ShaderLevel_5 = 1 << 4,
            Texutres2K = 1 << 5,
            Texutres4K = 1 << 6,
            Texutres8K = 1 << 7,
            Texutres16K = 1 << 8,
            Textures2DArray = 1 << 9,
            Textures3DVolume = 1 << 10,
            AnsotropicTextures = 1 << 11,
            Shadows = 1 << 12,
            MotionVectors = 1 << 13,
        }

        public static class ApplicationInfo
        {
            public static PlatformSpecifies GetSpecifies()
            {
                PlatformSpecifies specifies = 0;

                int shaderLevel = SystemInfo.graphicsShaderLevel;
                int textureSize = SystemInfo.maxTextureSize;

                if (shaderLevel >= 30) specifies = specifies | PlatformSpecifies.ShaderLevel_3;
                if (shaderLevel >= 40) specifies = specifies | PlatformSpecifies.ShaderLevel_4;
                if (shaderLevel >= 45) specifies = specifies | PlatformSpecifies.ShaderLevel_4_5;
                if (shaderLevel >= 50) specifies = specifies | PlatformSpecifies.ShaderLevel_5;

                if (SystemInfo.supportsComputeShaders) specifies = specifies | PlatformSpecifies.ComputeShaders;

                if (textureSize >= 2048) specifies = specifies | PlatformSpecifies.Texutres2K;
                if (textureSize >= 4096) specifies = specifies | PlatformSpecifies.Texutres4K;
                if (textureSize >= 8192) specifies = specifies | PlatformSpecifies.Texutres8K;
                if (textureSize >= 16384) specifies = specifies | PlatformSpecifies.Texutres16K;

                if (SystemInfo.supports2DArrayTextures) specifies = specifies | PlatformSpecifies.Textures2DArray;
                if (SystemInfo.supports3DTextures) specifies = specifies | PlatformSpecifies.Textures3DVolume;

                if (SystemInfo.supportsAnisotropicFilter) specifies = specifies | PlatformSpecifies.AnsotropicTextures;
                if (SystemInfo.supportsShadows) specifies = specifies | PlatformSpecifies.Shadows;

                if (SystemInfo.supportsMotionVectors) specifies = specifies | PlatformSpecifies.MotionVectors;

                return specifies;
            }
        }

   #endregion
    }
}
