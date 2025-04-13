using System;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Color skyColor;
    public bool enableLights;

    public static event Action OnEnvironmnetChange;
    public static EnvironmentManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OnEnvironmnetChange?.Invoke();
    }
}
