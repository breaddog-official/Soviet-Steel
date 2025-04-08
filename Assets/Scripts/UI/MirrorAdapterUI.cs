using UnityEngine;

[ExecuteInEditMode]
public class MirrorAdapterUI : MonoBehaviour
{
    [SerializeField] protected RenderTexture renderTexture;

    protected RectTransform rect;
    protected Vector2 cache;


    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        cache = rect.sizeDelta;
    }

    private void Update()
    {
        Vector2Int resolution = new((int)(cache.x * rect.lossyScale.x), (int)(cache.y * rect.lossyScale.y));
        
        if (renderTexture.width != resolution.x || renderTexture.height != resolution.y)
        {
            //renderTexture.width = resolution.x;
            //renderTexture.height = resolution.y;
        }
    }
}
