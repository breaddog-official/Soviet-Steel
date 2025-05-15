using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class FixedFov : MonoBehaviour
{
    protected enum AspectModes
    {
        All,
        OnlyLess,
        OnlyGreater
    }

    protected enum CameraMode
    {
        Camera,
        Cinemachine
    }

    [SerializeField] protected CameraMode cameraMode;
    [SerializeField] protected Camera cam;
    [ShowIf(nameof(cameraMode), CameraMode.Cinemachine)]
    [SerializeField] protected CinemachineCamera cinemaCam;
    [Space]
    [SerializeField] protected float targetFov = 60f;
    [SerializeField] protected float maxFov = 130f;
    [SerializeField] protected Vector2Int targetAspect = new(16, 9);
    [SerializeField] protected AspectModes aspectModes = AspectModes.All;


    protected float TargetAspect => ((float)targetAspect.x) / targetAspect.y;
    protected float CurrentAspect => ((float)cam.pixelWidth) / cam.pixelHeight;


    private void Start()
    {
        if (cam == null && Application.isEditor)
            cam = FindAnyObjectByType<Camera>();

        if (cinemaCam == null && Application.isEditor)
            cinemaCam = GetComponent<CinemachineCamera>();
    }

    protected virtual void Update()
    {
        if (cam == null || (cameraMode == CameraMode.Cinemachine && cinemaCam == null))
            return;

        float currentAspect;

        if (aspectModes == AspectModes.OnlyLess && TargetAspect < CurrentAspect)
            currentAspect = TargetAspect;

        else if (aspectModes == AspectModes.OnlyGreater && TargetAspect > CurrentAspect)
            currentAspect = TargetAspect;

        else
            currentAspect = CurrentAspect;

        float calculatedFov = targetFov * TargetAspect / currentAspect;
        SetFov(Mathf.Min(calculatedFov, maxFov));
    }

    protected virtual void SetFov(float fov)
    {
        switch (cameraMode)
        {
            case CameraMode.Camera:
                cam.fieldOfView = fov;
                break;

            case CameraMode.Cinemachine:
                cinemaCam.Lens.FieldOfView = fov;
                break;
        }
    }
}
