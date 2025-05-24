using Cysharp.Threading.Tasks;
using Scripts.SceneManagement;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public Transform credits;
    public float speed = 1f;
    public float endHeight;

    public const string menuScene = "Menu";

    bool isLoading;

    protected virtual void Update()
    {
        var newPosition = credits.position;
        newPosition.y += speed * Time.deltaTime;

        credits.position = newPosition;

        if (credits.position.y >= endHeight && !isLoading)
        {
            isLoading = true;
            Loader.LoadSceneAsync(menuScene, true).Forget();
        }
    }
}
