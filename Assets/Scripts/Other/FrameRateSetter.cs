using UnityEngine;

public class FrameRateSetter : MonoBehaviour
{
    [Min(0)]
    public int targetFrameRate = 60;

    protected virtual void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
