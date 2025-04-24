using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderGradient : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [Space]
    [SerializeField] private Gradient gradient;
    [SerializeField] private GradientApplyType gradientType;
    [ShowIf(nameof(gradientType), GradientApplyType.Image)]
    [SerializeField] private Image image;
    [ShowIf(nameof(gradientType), GradientApplyType.Text)]
    [SerializeField] private TMP_Text text;

    public void SetValue(float value)
    {
        if (slider == null && !TryGetComponent(out slider))
            return;

        if (gradientType == GradientApplyType.Image)
        {
            if (image == null && !TryGetComponent(out image))
                return;

            image.color = gradient.Evaluate(Mathf.Clamp01(value / slider.maxValue));
        }
            

        else if (gradientType == GradientApplyType.Text)
        {
            if (text == null && !TryGetComponent(out text))
                return;

            text.color = gradient.Evaluate(Mathf.Clamp01(value / slider.maxValue));
        }
    }

    public enum GradientApplyType
    {
        Image,
        Text
    }
}
