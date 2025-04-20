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
        EnvironmentManager.OnEnvironmnetChange += UpdateEnvironment;
        UpdateEnvironment();
    }

    private void OnDisable()
    {
        EnvironmentManager.OnEnvironmnetChange -= UpdateEnvironment;
    }


    private void UpdateEnvironment()
    {
        if (EnvironmentManager.Instance == null)
            return;


        if (probe != null)
        {
            probe.backgroundColor = EnvironmentManager.Instance.skyColor;
            probe.clearFlags = EnvironmentManager.Instance.skyBox ? ReflectionProbeClearFlags.Skybox : ReflectionProbeClearFlags.SolidColor;

            probe.RenderProbe();
        }

        if (mirror != null)
        {
            mirror.backgroundColor = EnvironmentManager.Instance.skyColor;
            mirror.clearFlags = EnvironmentManager.Instance.skyBox ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
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
