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
        return YG.YG2.envir.isMobile || YG.YG2.envir.isTablet;
#else
        return Touchscreen.current != null;
#endif
    }
}
