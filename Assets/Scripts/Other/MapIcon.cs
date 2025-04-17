using Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class MapIcon : MonoBehaviour
{
    [SerializeField] protected RawImage image;

    void Update()
    {
        image.texture = GameManager.GameMode?.Map?.Icon;
    }
}
