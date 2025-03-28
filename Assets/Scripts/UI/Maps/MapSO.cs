using NaughtyAttributes;
using UnityEngine;

namespace Scripts.UI
{
    [CreateAssetMenu(fileName = "Map", menuName = "Scripts/Map")]
    public class MapSO : ScriptableObject, IMap
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField, ResizableTextArea] public string Description { get; protected set; }
        [field: Scene]
        [field: SerializeField] public string Scene { get; protected set; }
        [field: Space]
        [field: SerializeField] public Texture2D Icon { get; protected set; }
    }
}