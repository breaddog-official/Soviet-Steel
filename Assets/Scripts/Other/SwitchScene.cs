using Mirror;
using NaughtyAttributes;
using Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneAttribute = NaughtyAttributes.SceneAttribute;

public class SwitchScene : MonoBehaviour
{
    [Scene]
    [SerializeField] protected List<string> scenes;

    protected int curScene;



    protected virtual void Start()
    {
        curScene = scenes.IndexOf(NetworkManager.singleton.onlineScene);
    }

    public void Switch()
    {
        curScene.IncreaseInBounds(scenes.Count);
        Apply();
    }

    public void Switch(int index)
    {
        curScene = index;
        Apply();
    }



    protected void Apply()
    {
        NetworkManager.singleton.onlineScene = scenes[curScene];
    }
}
