using Unity.Cinemachine;
using UnityEngine;

[AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Look At Offset")]
[SaveDuringPlay]
[DisallowMultipleComponent]
[CameraPipeline(CinemachineCore.Stage.Aim)]
[RequiredTarget(RequiredTargetAttribute.RequiredTargets.LookAt)]
public class CinemachineLookAtOffset : CinemachineComponentBase
{
    /// <summary>True if component is enabled and has a LookAt defined</summary>
    public override bool IsValid { get => enabled && LookAtTarget != null; }

    /// <summary>Get the Cinemachine Pipeline stage that this component implements.
    /// Always returns the Aim stage</summary>
    public override CinemachineCore.Stage Stage { get => CinemachineCore.Stage.Aim; }

    [Tooltip("Offset from the LookAt target's origin, in target's local space.  The camera will look at this point.")]
    public Vector3 LookAtOffset = Vector3.zero;
    public Vector3 RotationOffset = Vector3.zero;

    void Reset()
    {
        LookAtOffset = Vector3.zero;
    }

    /// <summary>Applies the composer rules and orients the camera accordingly</summary>
    /// <param name="curState">The current camera state</param>
    /// <param name="deltaTime">Used for calculating damping.  If less than
    /// zero, then target will snap to the center of the dead zone.</param>
    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
        if (IsValid && curState.HasLookAt())
        {
            var offset = LookAtTargetRotation * LookAtOffset;
            var rotationOffset = Quaternion.Euler(RotationOffset);

            Vector3 dir = ((curState.ReferenceLookAt + offset) - curState.GetCorrectedPosition());
            if (dir.magnitude > Epsilon)
            {
                if (Vector3.Cross(dir.normalized, curState.ReferenceUp).magnitude < Epsilon)
                    curState.RawOrientation = Quaternion.FromToRotation(Vector3.forward, dir) * rotationOffset;
                else
                    curState.RawOrientation = Quaternion.LookRotation(dir, curState.ReferenceUp) * rotationOffset;
            }
        }
    }
}
