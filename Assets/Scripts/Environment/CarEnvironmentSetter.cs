using UnityEngine;
using UnityEngine.Rendering;

public class CarEnvironmentSetter : MonoBehaviour
{
    [SerializeField] private ReflectionProbe probe;
    [SerializeField] private Camera mirror;
    [Space]
    [SerializeField] private GameObject[] lights;



    private void OnEnable()
    {
        EnvironmentManager.OnEnvironmentChange += UpdateEnvironment;
        UpdateEnvironment();
    }

    private void OnDisable()
    {
        EnvironmentManager.OnEnvironmentChange -= UpdateEnvironment;
    }


    private void UpdateEnvironment()
    {
        if (EnvironmentManager.Instance == null)
            return;


        if (probe != null)
        {
            probe.backgroundColor = EnvironmentManager.Instance.skyColor;
            probe.clearFlags = EnvironmentManager.Instance.skybox ? ReflectionProbeClearFlags.Skybox : ReflectionProbeClearFlags.SolidColor;

            probe.RenderProbe();
        }

        if (mirror != null)
        {
            mirror.backgroundColor = EnvironmentManager.Instance.skyColor;
            mirror.clearFlags = EnvironmentManager.Instance.skybox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
        }
            

        if (lights != null)
        {
            foreach (var light in lights)
            {
                light.SetActive(EnvironmentManager.Instance.enableLights);
            }
        }
    }
}
