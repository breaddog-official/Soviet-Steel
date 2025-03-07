using UnityEngine;
using Unity.Burst;
using UnityEngine.InputSystem;
using Mirror;

namespace ArcadeVP
{
    [BurstCompile]
    public class ArcadeVehicleInput : NetworkBehaviour
    {
        protected ArcadeVehicleController arcadeVehicleController;
        protected PlayerInput playerInput;


        [SerializeField] protected string actionMap = "Player";
        [Space(10f)]
        [SerializeField] protected string moveControl = "Move";
        [SerializeField] protected string breakControl = "Break";

        protected InputAction moveAction;   
        protected InputAction breakAction;   


        protected void Awake()
        {
            arcadeVehicleController = GetComponent<ArcadeVehicleController>();

            playerInput = GetComponent<PlayerInput>();
            var inputMap = playerInput.actions.FindActionMap(actionMap);


            if (!string.IsNullOrWhiteSpace(moveControl))
                moveAction = inputMap.FindAction(moveControl);

            if (!string.IsNullOrWhiteSpace(breakControl))
                breakAction = inputMap.FindAction(breakControl);
        }

        private void Update()
        {
            ReadInput();
        }

        private void ReadInput()
        {
            if (!playerInput.enabled)
                return;

            Vector2? moveInput = moveAction?.ReadValue<Vector2>();
            bool? breakInput = breakAction?.IsPressed();

            arcadeVehicleController.ProvideInputs(moveInput ?? default, breakInput ?? default);
        }
    }
}
