using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Scripts.Extensions;
using System.Linq;
using Scripts.Audio;

namespace Scripts.UI
{
    [ExecuteAlways]
    public class ButtonDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region States

        [SerializeField] protected Graphic targetGraphics;

        [SerializeField, BoxGroup("Disabled"), Label("Animation"), EnumFlags] protected AnimateType disabledAnimation;
        [SerializeField, BoxGroup("Disabled"), Label("Color"), ShowIf(nameof(disabledAnimation), AnimateType.Color)] protected Color disabledColor = Color.white;
        [SerializeField, BoxGroup("Disabled"), Label("Move To"), ShowIf(nameof(disabledAnimation), AnimateType.Move)] protected Vector2 disabledPosition = Vector2.zero;
        [SerializeField, BoxGroup("Disabled"), Label("Scale"), ShowIf(nameof(disabledAnimation), AnimateType.Scale)] protected Vector2 disabledScale = Vector2.one;

        [SerializeField, BoxGroup("Highlighted"), Label("Animation"), EnumFlags] protected AnimateType highlightedAnimation;
        [SerializeField, BoxGroup("Highlighted"), Label("Color"), ShowIf(nameof(highlightedAnimation), AnimateType.Color)] protected Color highlightedColor = Color.white;
        [SerializeField, BoxGroup("Highlighted"), Label("Move To"), ShowIf(nameof(highlightedAnimation), AnimateType.Move)] protected Vector2 highlightedPosition = Vector2.zero;
        [SerializeField, BoxGroup("Highlighted"), Label("Scale"), ShowIf(nameof(highlightedAnimation), AnimateType.Scale)] protected Vector2 highlightedScale = Vector2.one;

        [SerializeField, BoxGroup("Selected"), Label("Animation"), EnumFlags] protected AnimateType selectedAnimation;
        [SerializeField, BoxGroup("Selected"), Label("Color"), ShowIf(nameof(selectedAnimation), AnimateType.Color)] protected Color selectedColor = Color.white;
        [SerializeField, BoxGroup("Selected"), Label("Move To"), ShowIf(nameof(selectedAnimation), AnimateType.Move)] protected Vector2 selectedPosition = Vector2.zero;
        [SerializeField, BoxGroup("Selected"), Label("Scale"), ShowIf(nameof(selectedAnimation), AnimateType.Scale)] protected Vector2 selectedScale = Vector2.one;

        [SerializeField, BoxGroup("Pressed"), Label("Animation"), EnumFlags] protected AnimateType pressedAnimation;
        [SerializeField, BoxGroup("Pressed"), Label("Color"), ShowIf(nameof(pressedAnimation), AnimateType.Color)] protected Color pressedColor = Color.white;
        [SerializeField, BoxGroup("Pressed"), Label("Move To"), ShowIf(nameof(pressedAnimation), AnimateType.Move)] protected Vector2 pressedPosition = Vector2.zero;
        [SerializeField, BoxGroup("Pressed"), Label("Scale"), ShowIf(nameof(pressedAnimation), AnimateType.Scale)] protected Vector2 pressedScale = Vector2.one;

        #endregion

        [Flags]
        protected enum AnimateType
        {
            None = 0,
            Color = 1 << 0,
            Move = 1 << 1,
            Scale = 1 << 2,

            Everything = Color | Move | Scale
        }

        protected enum AnimationState
        {
            Normal,
            Disabled,
            Highlighted,
            Selected,
            Pressed
        }

        protected Button button;
        protected RectTransform rect;

        protected Color cachedColor;
        protected Vector2 cachedPosition;
        protected Vector2 cachedScale;
        protected bool cachedInteractable;

        protected CancellationTokenSource animationToken;

        [SerializeField, HideInInspector]
        private bool autoSetCompleted;
        private bool initialized;
        private int frames;

        #region Initialization

        private void Reset()
        {
            Initialize();
        }

        protected void Initialize()
        {
            initialized = true;

            button = GetComponent<Button>();
            rect = GetComponent<RectTransform>();
            targetGraphics ??= GetComponent<Image>();

            Cache();

            // Editor sets
            if (!Application.isPlaying)
            {
                button.transition = Selectable.Transition.None;

                // Set default variables
                if (!autoSetCompleted)
                {
                    disabledColor = cachedColor;
                    highlightedColor = cachedColor;
                    selectedColor = cachedColor;
                    pressedColor = cachedColor;

                    autoSetCompleted = true;
                }
            }
        }

        protected void Cache()
        {
            cachedColor = targetGraphics.color;
            cachedPosition = rect.localPosition;
            cachedScale = rect.localScale;
            cachedInteractable = button.interactable;
        }

        #endregion

        private void Update()
        {
            if (frames <= 2)
                frames++;

            // Canvas groups update in first update, so we initializes in second
            if (frames == 2)
                Initialize();
            

            if (frames == 2 && cachedInteractable != button.interactable)
            {
                cachedInteractable = button.interactable;
                Animate(cachedInteractable ? AnimationState.Normal : AnimationState.Disabled).Forget();
            }
        }


        private void OnEnable()
        {
            if (initialized)
                Cache();
        }

        private void OnDisable()
        {
            animationToken?.Cancel();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            Animate(AnimationState.Highlighted).Forget();
            AudioManager.PlayHighlighted();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animate(AnimationState.Normal).Forget();
            AudioManager.PlayUnHighlighted();
        }



        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerEnter != gameObject)
            {
                Animate(AnimationState.Normal).Forget();
                return;
            }
            else if (ApplicationInfo.IsSelectableInput())
            {
                Animate(AnimationState.Highlighted).Forget();
            }
            else
            {
                Animate(AnimationState.Selected).Forget();
            }

            AudioManager.PlayButtonUp();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Animate(AnimationState.Pressed).Forget();
            AudioManager.PlayButtonDown();
        }



        public async void OnSelect(BaseEventData eventData)
        {
            if (ApplicationInfo.IsSelectableInput())
            {
                Animate(AnimationState.Selected).Forget();
                AudioManager.PlaySelected();
            }
            else
            {
                // If set new selected gameobject in the same frame, throwing exception "Attempting to select while already selecting an object."
                await UniTask.NextFrame();
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (ApplicationInfo.IsSelectableInput())
            {
                Animate(AnimationState.Normal).Forget();
                AudioManager.PlayDeselected();
            }
        }



        protected async UniTask Animate(AnimationState state, CancellationToken token = default)
        {
            if (!Application.isPlaying)
                return;

            animationToken?.ResetToken();
            animationToken = new();

            if (token == default)
                token = animationToken.Token;

            float duration = 0.5f;

            var colorTask = UniTask.CompletedTask;
            var positionTask = UniTask.CompletedTask;
            var scaleTask = UniTask.CompletedTask;


            if (StateToAnimation(state).HasFlag(AnimateType.Color))
            {
                colorTask = targetGraphics.DOColor(state switch
                {
                    AnimationState.Normal => cachedColor,
                    AnimationState.Disabled => disabledColor,
                    AnimationState.Highlighted => highlightedColor,
                    AnimationState.Selected => selectedColor,
                    AnimationState.Pressed => pressedColor,
                    _ => targetGraphics.color

                }, duration).WithCancellation(token);
            }

            if (StateToAnimation(state).HasFlag(AnimateType.Move))
            {
                positionTask = rect.DOLocalMove(state switch
                {
                    AnimationState.Normal => cachedPosition,
                    AnimationState.Disabled => cachedPosition + disabledPosition,
                    AnimationState.Highlighted => cachedPosition + highlightedPosition,
                    AnimationState.Selected => cachedPosition + selectedPosition,
                    AnimationState.Pressed => cachedPosition + pressedPosition,
                    _ => rect.localPosition

                }, duration).WithCancellation(token);
            }

            if (StateToAnimation(state).HasFlag(AnimateType.Scale))
            {
                scaleTask = rect.DOScale(state switch
                {
                    AnimationState.Normal => cachedScale,
                    AnimationState.Disabled => disabledScale,
                    AnimationState.Highlighted => highlightedScale,
                    AnimationState.Selected => selectedScale,
                    AnimationState.Pressed => pressedScale,
                    _ => rect.localScale

                }, duration).WithCancellation(token);
            }

            await UniTask.WhenAll(colorTask, positionTask, scaleTask);
        }

        protected AnimateType StateToAnimation(AnimationState state)
        {
            return state switch
            {
                AnimationState.Normal => AnimateType.Everything,
                AnimationState.Disabled => disabledAnimation,
                AnimationState.Highlighted => highlightedAnimation,
                AnimationState.Selected => selectedAnimation,
                AnimationState.Pressed => pressedAnimation,
                _ => AnimateType.None
            };
        }

        public void Click()
        {
            ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }
}