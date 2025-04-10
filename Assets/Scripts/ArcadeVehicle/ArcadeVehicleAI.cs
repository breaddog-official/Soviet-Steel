using System.Linq;
using Mirror;
using NaughtyAttributes;
using Scripts.Gameplay;
using Unity.Burst;
using UnityEngine;

namespace ArcadeVP
{
    [BurstCompile]
    public class ArcadeVehicleAI : NetworkBehaviour
    {
        [SerializeField] protected bool smoothInput = true;
        [ShowIf(nameof(smoothInput)), MinMaxSlider(0f, 1f)]
        [SerializeField] protected Vector2 smoothAmountRange = new(0.2f, 0.8f);
        [Space]
        [SerializeField, MinMaxSlider(0, 90)] private Vector2 maxRotationRange = new(5, 15);
        [Space, Range(1, 5)]
        [SerializeField] private int markerSamples = 2;
        [SerializeField] private float maxMarkerDistance = 50f;
        [CurveRange(0, 0, 1, 1, EColor.Green)]
        [SerializeField] private AnimationCurve markerBlend;
        [Space]
        [SerializeField, Range(0f, 1f)] private float minSpeed;
        [SerializeField, Range(0f, 1f)] private float maxSpeed;

        protected ArcadeVehicleController arcadeVehicleController;
        protected ArcadeVehicleNetwork arcadeVehicleNetwork;

        protected Vector2 lastInput;

        private float smoothAmount;
        private float maxRotation;


        protected void Awake()
        {
            smoothAmount = Random.Range(smoothAmountRange.x, smoothAmountRange.y);
            maxRotation = Random.Range(maxRotationRange.x, maxRotationRange.y);

            arcadeVehicleNetwork = GetComponent<ArcadeVehicleNetwork>();
            arcadeVehicleController = arcadeVehicleNetwork.vehicleController;
        }

        private void FixedUpdate()
        {
            ProcessAI();
        }

        private void ProcessAI()
        {
            Vector3 targetMarker = GetMarker();
            Vector3 targetMarkerRelative = (targetMarker - transform.position).normalized;

            Vector3 velocity = transform.forward;

            velocity.y = 0f;
            targetMarkerRelative.y = 0f;

            float amount = transform.InverseTransformPoint(targetMarker).x/*targetMarkerRelative.x - velocity.x*/;
            float x = Mathf.Lerp(0, 1, Mathf.Abs(amount) / maxRotation);

            float dot = Vector3.Dot(targetMarkerRelative, velocity);
            float y = Mathf.Clamp(Mathf.Abs(dot), minSpeed, maxSpeed);


            if (amount < 0)
                x = -x;
            if (dot < 0)
                y = -y;

            gizmosTargetMarkerDirection = targetMarkerRelative;
            gizmosVelocityDirection = velocity;
            gizmosMarker = targetMarker;

            Vector2 input = new(x, y);

            if (smoothInput)
            {
                lastInput = Vector2.Lerp(lastInput, input, (1f - smoothAmount) * Time.fixedDeltaTime * 100f);
                input = lastInput;
            }

            arcadeVehicleController.SetInput(input, false);
        } 

        private Vector3 GetMarker()
        {
            int currentMarker = GameManager.Instance.RoadManager.GetPlayers()[arcadeVehicleNetwork.netId].marker;
            int[] markers = new int[markerSamples];

            for (int i = 0; i < markers.Length; i++)
            {
                currentMarker = GameManager.Instance.RoadManager.GetNextPointIndex(currentMarker);
                markers[i] = currentMarker;
            }

            Vector3 resultVector = GameManager.Instance.RoadManager.GetPoint(markers[0]);
            foreach (var vector in markers.Select(m => GameManager.Instance.RoadManager.GetPoint(m)))
            {
                resultVector = Vector3.Lerp(vector, resultVector, markerBlend.Evaluate(Vector3.Distance(transform.position, vector) / maxMarkerDistance));
            }

            return resultVector;
        }


        protected Vector3 gizmosTargetMarkerDirection;
        protected Vector3 gizmosVelocityDirection;
        protected Vector3 gizmosMarker;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, gizmosVelocityDirection * 50f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, gizmosTargetMarkerDirection * 50f);

            Gizmos.color = new(1, 0, 0, 0.7f); // Red
            Gizmos.DrawSphere(gizmosMarker, GameManager.Instance.RoadManager.GetRadius());
        }
    }
}
