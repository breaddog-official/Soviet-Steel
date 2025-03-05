using UnityEngine;

public class TerrainColliderDisabler : MonoBehaviour
{
    [SerializeField] protected TerrainCollider terrainCollider;

    private void Start()
    {
        terrainCollider.enabled = false;
    }
}
