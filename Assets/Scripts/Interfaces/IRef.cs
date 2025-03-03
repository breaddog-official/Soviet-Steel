using NaughtyAttributes;
using UnityEngine;

namespace Scripts
{
    [System.Serializable, ExecuteAlways]
    public class IRef<T> : ISerializationCallbackReceiver where T : class
    {
        [OnValueChanged(nameof(OnValidate))]
        public Object target;
        public T Value => target as T;

        public static implicit operator bool(IRef<T> iref) => iref.target != null;

        void OnValidate()
        {
            if (target is not T)
            {
                if (target is GameObject go)
                {
                    target = null;
                    foreach (Component c in go.GetComponents<Component>())
                    {
                        if (c is T)
                        {
                            target = c;
                            break;
                        }
                    }
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}