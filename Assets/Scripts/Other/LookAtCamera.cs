using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    [SerializeField] protected bool executeInEditMode;
    [SerializeField] protected bool inverse;

    private void OnEnable()
    {
        Camera.onPreCull += LookAt;
    }

    private void OnDisable()
    {
        Camera.onPreCull -= LookAt;
    }


    protected virtual void LookAt(Camera cam)
    {
        if (!executeInEditMode && !Application.isPlaying)
            return;

        Vector3 relativePos = cam.transform.position - transform.position;

        if (inverse)
            relativePos = -relativePos;

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }
}
