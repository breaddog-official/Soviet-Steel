using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class ScreenshotMaker : MonoBehaviour
{
    public string filename = "screenshot";
    [Range(1, 16)]
    public int resolutionMultiplier = 8;

    [Button]
    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(filename + ".png", resolutionMultiplier);
    }
}
