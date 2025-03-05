using UnityEngine;

namespace Scripts.UI
{
    public interface IMap
    {
        public string Name { get; }
        public string Description { get; }
        public string Scene { get; }

        public Texture2D Icon { get; }
    }
}