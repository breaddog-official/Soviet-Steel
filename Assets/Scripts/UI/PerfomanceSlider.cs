using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerfomanceSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text text;
    [SerializeField] List<PerfomanceLevel> perfomanceLevels;
    [Space]
    [SerializeField] private float smoothAmount = 0.0005f;

    private float lastFps = 30f;


    private void OnEnable()
    {
        lastFps = 1f / Time.deltaTime;
    }

    private void Update()
    {
        float newFPS = 1.0f / Time.smoothDeltaTime;
        lastFps = Mathf.Lerp(lastFps, newFPS, smoothAmount);

        slider.value = lastFps;

        UpdateValue(slider.value);
    }

    public void UpdateValue(float value)
    {
        foreach (var level in perfomanceLevels)
        {
            if (value <= level.value)
            {
                //print($"{text.TranslationString}: {lastFps}");
                text.SetText(level.text);
                break;
            }
        }
    }
}

[Serializable]
public struct PerfomanceLevel
{
    public string text;
    public float value;
}