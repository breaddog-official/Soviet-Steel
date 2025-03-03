using Scripts.Extensions;
using UnityEngine;
using NaughtyAttributes;

namespace Scripts 
{
    [ExecuteAlways]
    public class Initializator : MonoBehaviour
    {
        [field: SerializeField] public bool InitializeOnAwake { get; private set; } = true;
        [field: SerializeField] public IRef<IInitializable> Initializable { get; private set; }


        private void Awake()
        {
            if (InitializeOnAwake && Application.isPlaying)
                Initialize();
        }

        [Button]
        public void Initialize()
        {
            if (Initializable.Value != null)
            {
                Initializable.Value.Initialize();
                print($"{Initializable.Value} initialized!");
            }
        }
    }
}
