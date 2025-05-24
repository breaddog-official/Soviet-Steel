using UnityEngine;

namespace Scripts.UI
{
    public class ToggleUI : MonoBehaviour
    {
        [SerializeField] protected GameObject enableObject;


        public virtual void Enable()
        {
            enableObject.SetActive(true);
        }

        public virtual void Disable()
        {
            enableObject.SetActive(false);
        }


        public virtual void Toggle()
        {
            if (enableObject.activeSelf)
                Disable();
            else
                Enable();
        }
    }
}