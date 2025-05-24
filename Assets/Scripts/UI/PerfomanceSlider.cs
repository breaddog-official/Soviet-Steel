using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.TranslateManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerfomanceSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextTranslater text;
    [SerializeField] List<PerfomanceLevel> perfomanceLevels;
    [Space, Min(1)]
    [SerializeField] private int bufferSize = 10;

    private List<float> buffer;


    private void OnEnable()
    {
        buffer = new(bufferSize);
    }

    private void Update()
    {
        float newFPS = 1f / Time.deltaTime;
        buffer.Insert(0, newFPS);

        if (buffer.Count > bufferSize)
            buffer.RemoveAt(buffer.Count - 1);

        float averageFps = buffer.Average();

        UpdateValue(averageFps);
    }

    public void UpdateValue(float value)
    {
        slider.value = value;

        foreach (var level in perfomanceLevels)
        {
            if (value <= level.value)
            {
                //print($"{text.TranslationString}: {lastFps}");
                text.SetName(level.text);
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