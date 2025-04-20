using NaughtyAttributes;
using Scripts.Extensions;
using UnityEngine;

public class PlatformDisabler : MonoBehaviour
{
    [EnumFlags]
    public RuntimePlatformFlags disablePlatforms;

    private void Awake()
    {
        if (disablePlatforms.HasFlag(Application.platform.ToFlags()))
        {
            gameObject.SetActive(false);
        }
    }
}
