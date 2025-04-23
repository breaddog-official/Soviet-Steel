using UnityEngine;

public class FrameRateSetter : MonoBehaviour
{
    //[Min(0)]
    //public int targetFrameRate = 60;

    protected virtual void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;//targetFrameRate;
        else
            Application.targetFrameRate = -1;
    }
}
