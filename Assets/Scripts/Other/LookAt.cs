using Unity.Burst;
using UnityEngine;

namespace Scripts.Gameplay
{
    [BurstCompile]
    public class LookAt : MonoBehaviour
    {
        [SerializeField] protected Transform lookAt;

        protected void Update()
        {
            transform.LookAt(lookAt);
        }
    }
}