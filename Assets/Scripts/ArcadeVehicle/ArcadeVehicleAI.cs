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
        [Space, Range(0, 100)]
        [SerializeField] private int markerRandomization = 40;
        [Range(0f, 1f)]
        [SerializeField] private float markerSmoothing = 0.6f;
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

        private Vector3[] randomedMarkersOffset;


        protected void Awake()
        {
            smoothAmount = Random.Range(smoothAmountRange.x, smoothAmountRange.y);
            maxRotation = Random.Range(maxRotationRange.x, maxRotationRange.y);

            arcadeVehicleNetwork = GetComponent<ArcadeVehicleNetwork>();
            arcadeVehicleController = arcadeVehicleNetwork.vehicleController;

            var markers = GameManager.Instance.RoadManager.GetMarkers();
            var randomizationFactor = GameManager.Instance.RoadManager.GetRadius() / 100f * markerRandomization;

            randomedMarkersOffset = new Vector3[markers.Count];

            for (int i = 1; i < randomedMarkersOffset.Length; i++)
            {
                randomedMarkersOffset[i] = Vector3.Lerp(randomedMarkersOffset[i - 1], Random.insideUnitSphere * randomizationFactor, 1f - markerSmoothing);
            }
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
            int currentMarker = GameManager.Instance.RoadManager.GetPlayer(arcadeVehicleNetwork.netId).marker;
            int[] markers = new int[markerSamples];

            for (int i = 0; i < markers.Length; i++)
            {
                currentMarker = GameManager.Instance.RoadManager.GetNextPointIndex(currentMarker);
                markers[i] = currentMarker;
            }

            Vector3 resultVector = GetMarkerPointWithRandomization(markers[0]);
            foreach (var vector in markers.Select(m => GetMarkerPointWithRandomization(m)))
            {
                resultVector = Vector3.Lerp(vector, resultVector, markerBlend.Evaluate(Vector3.Distance(transform.position, vector) / maxMarkerDistance));
            }

            return resultVector;
        }

        private Vector3 GetMarkerPointWithRandomization(int index) => GameManager.Instance.RoadManager.GetPoint(index) + randomedMarkersOffset[index];


        protected Vector3 gizmosTargetMarkerDirection;
        protected Vector3 gizmosVelocityDirection;
        protected Vector3 gizmosMarker;

        private void OnDrawGizmosSelected()
        {
            if (GameManager.Instance == null || GameManager.Instance.RoadManager == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, gizmosVelocityDirection * 50f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, gizmosTargetMarkerDirection * 50f);

            Gizmos.color = new(1, 0, 0, 0.7f); // Red
            Gizmos.DrawSphere(gizmosMarker, GameManager.Instance.RoadManager.GetRadius());
            
            Gizmos.color = new(1, 0.92f, 0.016f, 0.4f); // Yellow
            Gizmos.DrawSphere(gizmosMarker, GameManager.Instance.RoadManager.GetRadius() / 100f * markerRandomization);
        }
    }
}
