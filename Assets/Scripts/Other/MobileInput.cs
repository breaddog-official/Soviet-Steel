using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInput : MonoBehaviour
{
    [SerializeField] protected GameObject mobileInputs;

    protected virtual void FixedUpdate()
    {
        mobileInputs.SetActive(Touchscreen.current != null);
    }
}
