using System;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Color skyColor;
    public bool skybox;
    public bool enableLights;
    public float renderDistance = 500f;

    public static event Action OnEnvironmentChange;
    public static EnvironmentManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OnEnvironmentChange?.Invoke();
    }

    public void SetRenderDistance(float distance)
    {
        renderDistance = distance;
        OnEnvironmentChange?.Invoke();
    }
}
