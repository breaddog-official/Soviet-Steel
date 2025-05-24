using NaughtyAttributes;
using Scripts.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class ScreenshotMaker : MonoBehaviour
{
    public string filename = "screenshot";
    [Range(1, 16)]
    public int resolutionMultiplier = 8;


    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            ControlsManager.Controls.UI.Screenshot.performed += TakeScreenshot;
        }
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            ControlsManager.Controls.UI.Screenshot.performed -= TakeScreenshot;
        }
    }



    private void TakeScreenshot(InputAction.CallbackContext ctx) => TakeScreenshot();

    [Button]
    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(filename + Time.time + ".png", resolutionMultiplier);
    }
}
