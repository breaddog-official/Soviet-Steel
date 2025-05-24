using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInput : MonoBehaviour
{
    [SerializeField] protected GameObject mobileInputs;

    protected virtual void FixedUpdate()
    {
        mobileInputs.SetActive(IsMobileInput());
    }

    public bool IsMobileInput()
    {
#if YandexGamesPlatform_yg
        return YG.YG2.envir.device == YG.YG2.Device.Mobile || YG.YG2.envir.device == YG.YG2.Device.Tablet;
#else
        return Touchscreen.current != null;
#endif
    }
}
