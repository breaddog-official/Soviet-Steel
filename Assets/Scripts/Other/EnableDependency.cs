using UnityEngine;

[ExecuteInEditMode]
public class EnableDependency : MonoBehaviour
{
    [SerializeField] protected GameObject[] dependencedObjects;


    private void OnEnable()
    {
        SetState(true);
    }

    private void OnDisable()
    {
        SetState(false);
    }



    private void SetState(bool state)
    {
        foreach (var obj in dependencedObjects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}
