using UnityEngine;
using UnityEngine.EventSystems;

public class FirstSelectUI : MonoBehaviour
{
    [SerializeField] protected GameObject selectObject;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(selectObject);
    }
}
