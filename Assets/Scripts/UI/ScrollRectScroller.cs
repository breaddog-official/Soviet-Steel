using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Unity.Burst;

namespace Scripts.UI
{
    [BurstCompile]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float scrollSpeed = 10f;
        [SerializeField] private InputActionReference scrollInputAction;

        private bool mouseOver = false;

        private List<Selectable> m_Selectables = new List<Selectable>();
        private ScrollRect m_ScrollRect;

        private Vector2 m_NextScrollPosition = Vector2.up;

        private bool scrollInputAction_Pressed;

        void OnEnable()
        {
            if (m_ScrollRect)
            {
                m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
            }
            scrollInputAction.ToInputAction().started += context => scrollInputAction_Pressed = true;
            scrollInputAction.ToInputAction().canceled += context => scrollInputAction_Pressed = false;
        }
        void OnDisable()
        {
            scrollInputAction.ToInputAction().started -= context => scrollInputAction_Pressed = true;
            scrollInputAction.ToInputAction().canceled -= context => scrollInputAction_Pressed = false;
        }
        void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
        }
        void Start()
        {
            if (m_ScrollRect)
            {
                m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
            }
            ScrollToSelected(true);
        }
        void Update()
        {
            if (scrollInputAction_Pressed)
            {
                ScrollToSelected(false);
            }
            if (!mouseOver)
            {
                // Lerp scrolling code.
                m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
            }
            else
            {
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
        }
        void ScrollToSelected(bool quickScroll)
        {
            int selectedIndex = -1;
            Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

            if (selectedElement)
            {
                selectedIndex = m_Selectables.IndexOf(selectedElement);
            }
            if (selectedIndex > -1)
            {
                m_NextScrollPosition = new Vector2(0.0f, 1 - Math.Clamp(selectedIndex / ((float)m_Selectables.Count - 1), 0.0f, 1.0f));
                if (quickScroll)
                {
                    m_ScrollRect.normalizedPosition = m_NextScrollPosition;
                }
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
            ScrollToSelected(false);
        }
    }
}
