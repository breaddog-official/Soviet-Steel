using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SliderGradient : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private Slider slider;
    [SerializeField] private GradientApplyType gradientType;
    private Image image;
    private TMP_Text text;

    public void SetValue(float value)
    {
        if (slider == null)
            return;

        if (gradientType == GradientApplyType.Image)
        {
            if (image == null)
                image = GetComponent<Image>();

            image.color = gradient.Evaluate(value / slider.maxValue);
        }
            

        else if (gradientType == GradientApplyType.Text)
        {
            if (text == null)
                text = GetComponent<TMP_Text>();

            text.color = gradient.Evaluate(value / slider.maxValue);
        }
    }

    public enum GradientApplyType
    {
        Image,
        Text
    }
}
