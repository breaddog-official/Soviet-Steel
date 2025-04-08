using UnityEngine;
using Unity.Burst;
using UnityEngine.InputSystem;
using Mirror;
using NaughtyAttributes;

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
        //[SerializeField] protected string breakControl = "Break";
        [Space]
        [SerializeField] protected bool smoothInput = true;
        [ShowIf(nameof(smoothInput)), Range(0f, 1f)]
        [SerializeField] protected float smoothAmount = 0.2f;
        [Space]
        [SerializeField] protected bool clampInput = true;
        [ShowIf(nameof(clampInput)), MinMaxSlider(-1f, 1f)]
        [SerializeField] protected Vector2 clampBorders;

        protected InputAction moveAction;   
        protected InputAction breakAction;

        protected Vector2 lastMove;


        protected void Awake()
        {
            arcadeVehicleController = GetComponent<ArcadeVehicleController>();

            playerInput = GetComponent<PlayerInput>();
            var inputMap = playerInput.actions.FindActionMap(actionMap);


            if (!string.IsNullOrWhiteSpace(moveControl))
                moveAction = inputMap.FindAction(moveControl);

            //if (!string.IsNullOrWhiteSpace(breakControl))
            //    breakAction = inputMap.FindAction(breakControl);
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
            bool? breakInput = false;//breakAction?.IsPressed();

            if (moveInput.HasValue)
            {
                if (smoothInput)
                {
                    lastMove = Vector2.Lerp(lastMove, moveInput.Value, (1f - smoothAmount) * Time.deltaTime * 100f);
                    moveInput = lastMove;
                }
                
                if (clampInput)
                {
                    moveInput = new(moveInput.Value.x, Mathf.SmoothStep(-1f, 1f, Mathf.InverseLerp(clampBorders.x, clampBorders.y, moveInput.Value.y)));
                }
            }
            
            arcadeVehicleController.SetInput(moveInput ?? default, breakInput ?? default);
        }
    }
}
