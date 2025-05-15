using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

public class RenderDistanceApplyer : MonoBehaviour
{
    private enum CameraMode
    {
        Camera,
        Cinemachine
    }

    [SerializeField] private CameraMode cameraMode = CameraMode.Cinemachine;
    [ShowIf(nameof(cameraMode), CameraMode.Camera)]
    [SerializeField] private Camera cam;
    [ShowIf(nameof(cameraMode), CameraMode.Cinemachine)]
    [SerializeField] private CinemachineCamera cinemachine;

    private float Distance => EnvironmentManager.Instance != null ? EnvironmentManager.Instance.renderDistance : DefaultDistance;

    private float DefaultDistance => cameraMode == CameraMode.Camera ? cam.farClipPlane : cinemachine.Lens.FarClipPlane;


    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (cam == null)
                cam = GetComponent<Camera>();

            if (cinemachine == null)
                cinemachine = GetComponent<CinemachineCamera>();
        }
    }

    private void OnEnable()
    {
        EnvironmentManager.OnEnvironmentChange += UpdateRenderDistance;
        UpdateRenderDistance();
    }

    private void OnDisable()
    {
        EnvironmentManager.OnEnvironmentChange += UpdateRenderDistance;
    }


    public void UpdateRenderDistance()
    {
        if (cameraMode == CameraMode.Camera)
            cam.farClipPlane = Distance;

        else if (cameraMode == CameraMode.Cinemachine)
            cinemachine.Lens.FarClipPlane = Distance;
    }
}
