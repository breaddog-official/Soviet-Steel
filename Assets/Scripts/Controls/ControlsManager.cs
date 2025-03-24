using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Controls
{
    public class ControlsManager
    {
        [SerializeField] protected ControlsPreset[] presets;

        public static ControlsManager Instance { get; private set; }


        protected void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Instance = this;
        }
    }

    public class ControlsPreset : ScriptableObject
    {
        public const string actionMap = "Player";
        public static InputAction action;
        public Vector2 lastValue;


        public string moveControl = "Move";
        [Space]
        public bool isNormalize;
        public bool isMultiplyDeltaTime;
        public bool isSmooth;
        [ShowIf(nameof(isSmooth)), Range(0f, 1f)]
        public float smoothAmount;

        public Vector2 GetMove()
        {
            if (action == null)
            {
                var inputMap = InputSystem.actions.FindActionMap(actionMap);
                action = inputMap.FindAction(moveControl);
            }

            var input = action.ReadValue<Vector2>();

            if (isNormalize)
            {
                input.Normalize();
            }

            if (isSmooth)
            {
                input = Vector2.Lerp(lastValue, input, smoothAmount * Time.deltaTime * 100f);
                lastValue = input;
            }

            return input;
        }
    }
}