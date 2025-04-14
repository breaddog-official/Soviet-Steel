using Scripts.Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnyButton : MonoBehaviour
{
    public UnityEvent onAnyButtonPressed;


    private void OnEnable()
    {
        ControlsManager.Controls.UI.AnyButton.started += Invoke;
    }

    private void OnDisable()
    {
        ControlsManager.Controls.UI.AnyButton.started -= Invoke;
    }


    private void Invoke(InputAction.CallbackContext ctx = default)
    {
        onAnyButtonPressed?.Invoke();
    }
}
