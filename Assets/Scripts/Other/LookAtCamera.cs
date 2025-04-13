using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
    [SerializeField] protected bool executeInEditMode;
    [SerializeField] protected bool inverse;
    [Space]
    [SerializeField] protected bool disableByDistance;
    [SerializeField] protected bool resizeByDistance;
    [ShowIf(EConditionOperator.Or, nameof(disableByDistance), nameof(resizeByDistance)), MinValue(0f)]
    [SerializeField] protected float minDistance;
    [ShowIf(EConditionOperator.Or, nameof(disableByDistance), nameof(resizeByDistance)), MinValue(0f)]
    [SerializeField] protected float maxDistance = 50f;
    [ShowIf(nameof(resizeByDistance))]
    [SerializeField] protected Vector3 minSize = Vector3.one;
    [ShowIf(nameof(resizeByDistance))]
    [SerializeField] protected Vector3 maxSize = Vector3.one;
    [ShowIf(nameof(resizeByDistance)), CurveRange(0, 0, 1, 1, EColor.Indigo)]
    [SerializeField] protected AnimationCurve sizeEvalution;

    private Renderer[] renderers;


    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

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

        float curDistance = Vector3.Distance(cam.transform.position, transform.position);

        if (disableByDistance)
            SetVisible(curDistance < maxDistance && curDistance > minDistance);

        if (resizeByDistance)
        {
            var amount = sizeEvalution.Evaluate(curDistance / maxDistance);
            transform.localScale = Vector3.Lerp(minSize, maxSize, amount);
        }

        Vector3 relativePos = cam.transform.position - transform.position;

        if (inverse)
            relativePos = -relativePos;

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }

    protected virtual void SetVisible(bool visible)
    {
        foreach (Renderer renderer in renderers)
            renderer.enabled = visible;
    }
}
