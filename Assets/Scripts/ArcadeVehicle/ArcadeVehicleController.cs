using UnityEngine;
using Unity.Burst;
using Mirror;
using Unity.Cinemachine;
using NaughtyAttributes;
using Scripts.Extensions;

namespace ArcadeVP
{
    [BurstCompile]
    public class ArcadeVehicleController : NetworkBehaviour
    {
        public enum GroundCheckType { None, RayCast, SphereCaste }
        public enum MovementMode { Velocity, AngularVelocity, Force }


        public MovementMode movementMode;
        public GroundCheckType groundCheck;
        public LayerMask drivableSurface;

        public float maxSpeed = 80f;
        public float accelaration = 12f;
        public float turn = 12f;
        public float drift = 10f;
        [Space]
        public float gravity = 7f;
        public float downforce = 5f;
        [Space]
        public float mass = 1000f;

        [Space]
        [Tooltip("if true : can turn vehicle in air")]
        public bool AirControl = false;
        [Tooltip("if true : vehicle will drift instead of brake while holding space")]
        public bool kartLike = false;
        [Tooltip("turn more while drifting (while holding space) only if kart Like is true")]
        public float driftMultiplier = 1.5f;
        public float driftAngle = 25f;


        public bool Grounded { get; private set; }

        [Header("Physics")]
        public Rigidbody rb;
        public Rigidbody carBody;
        
        [Space]
        public float minFriction = 1f;
        public float maxFriction = 5f;
        [HideInInspector] public PhysicsMaterial frictionMaterial;
        [HideInInspector] public RaycastHit hit;

        [Header("Curves")]
        [CurveRange(0, 0, 1, 1, EColor.Red)] public AnimationCurve accelerationCurve;
        [CurveRange(0, 0, 1, 1, EColor.Yellow)] public AnimationCurve frictionCurve;
        [CurveRange(0, 0, 1, 1, EColor.Green)] public AnimationCurve turnCurve;
        [Header("Visuals")]
        public Transform BodyMesh;
        public Transform[] FrontWheels = new Transform[2];
        public Transform[] RearWheels = new Transform[2];
        [HideInInspector, SyncVar]
        public Vector3 carVelocity;

        [Range(0, 10)]
        public float BodyTilt;
        [Header("Audio settings")]
        public AudioSource engineSound;
        public AudioSource skidSound;
        [Range(0f, 1f)]
        public float spatialBlend = 0.6f;

        [MinMaxSlider(0f, 5f)] public Vector2 pitchRange = new(0.5f, 2.5f);

        public float MinPitch => pitchRange.x;
        public float MaxPitch => pitchRange.y;


        [Header("Camera Fov")]
        public CinemachineCamera cinemachineCamera;
        [MinMaxSlider(20f, 180f)] public Vector2 fovRange = new(60, 75);
        [Range(0f, 1f)] public float smoothFov = 0.7f;
        [CurveRange(EColor.Green)] public AnimationCurve fovCurve;

        public float MinFov => fovRange.x;
        public float MaxFov => fovRange.y;


        [Header("Camera Noise")]
        [MinMaxSlider(0f, 5f)] public Vector2 noiseRange = new(0, 5);
        [Range(0f, 1f)] public float smoothNoise = 0.7f;
        [CurveRange(EColor.Green)] public AnimationCurve amplitudeCurve;

        public float MinNoise => noiseRange.x;
        public float MaxNoise => noiseRange.y;

        private CinemachineBasicMultiChannelPerlin noise;


        [Header("Camera Rotation")]
        [Min(0f)] public float cameraRotationMaxVelocity = 15;
        [Range(0f, 90f)] public float cameraRotationAngle = 15;
        [Range(0f, 1f)] public float smoothCameraRotation = 0.7f;
        [CurveRange(EColor.Green)] public AnimationCurve cameraRotationCurve;

        private CinemachineLookAtOffset lookAt;

        [Header("Camera X Move")]
        [Min(0f)] public float cameraXMoveMaxVelocity = 15;
        [Range(0f, 5f)] public float cameraXMove = 1;
        [Range(0f, 1f)] public float smoothCameraXMove = 0.7f;
        [CurveRange(EColor.Green)] public AnimationCurve cameraXMoveCurve;

        private CinemachineFollow follow;



        [HideInInspector]
        public float skidWidth;


        private SphereCollider sphereCollider;

        private bool handbrake;

        private float radius;
        private Vector3 origin;

        [SyncVar]
        private Vector2 moveInput;
        private bool brakeInput;

        private float AccelerationInput => moveInput.y * accelerationCurve.Evaluate(Speed / maxSpeed) * AccelerationMultiplier;
        private float SteeringInput => moveInput.x;
        private bool BrakeInput => handbrake || brakeInput;

        private bool KartLikeBrake => kartLike && brakeInput;
        public float Speed => new Vector3(carVelocity.x, 0f, carVelocity.z).magnitude;

        public float AccelerationMultiplier = 1f;

        private ArcadeVehicleNetwork network;


        private void Start()
        {
            frictionMaterial = new PhysicsMaterial();
            frictionMaterial.staticFriction = 1f;
            frictionMaterial.bounciness = 0f;
            frictionMaterial.frictionCombine = PhysicsMaterialCombine.Maximum;

            sphereCollider = rb.GetComponent<SphereCollider>();
            sphereCollider.sharedMaterial = frictionMaterial;

            radius = sphereCollider.radius;

            rb.mass = mass;
            carBody.mass = mass;

            network = GetComponent<ArcadeVehicleNetwork>();

            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
        }

        private void OnDestroy()
        {
            DestroyImmediate(frictionMaterial, true);
        }

        private void Update()
        {
            Visuals();
            AudioManager();
            CameraManager();
        }

        private void FixedUpdate()
        {
            if (isOwned)
            {
                carVelocity = carBody.transform.InverseTransformDirection(carBody.linearVelocity);

                if (!NetworkServer.active)
                    SendVelocity(carVelocity);
            }
            

            GroundCheck();
            BreakLogic();

            if (isOwned)
            {
                TurnLogic();
                GravityLogic();
                AccelerationLogic();
            }
        }

        public void SetInput(Vector2 _moveInput, bool _brakeInput)
        {
            moveInput = _moveInput;
            brakeInput = _brakeInput;
        }

        public void SetHandbrake(bool state)
        {
            handbrake = state;
        }

        [Command]
        public void SendVelocity(Vector3 velocity)
        {
            carVelocity = velocity;
        }


        #region Logic

        public void AccelerationLogic()
        {
            if (Grounded)
            {
                //accelaration logic
                if (Mathf.Abs(AccelerationInput) > 0.1f && (!BrakeInput || kartLike))
                {
                    if (movementMode == MovementMode.Velocity)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, AccelerationInput * maxSpeed * carBody.transform.forward, accelaration / 10 * ApplicationInfo.FixedDeltaTime);
                    }
                    else if (movementMode == MovementMode.AngularVelocity)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, AccelerationInput * maxSpeed * carBody.transform.right / radius, accelaration * ApplicationInfo.FixedDeltaTime);
                    }
                    else if (movementMode == MovementMode.Force)
                    {
                        rb.AddForce(AccelerationInput * accelaration * 5f * ApplicationInfo.FixedDeltaTime * carBody.transform.forward, ForceMode.VelocityChange);
                    }
                }
            }
        }

        public void TurnLogic()
        {
            float turnMultiplyer = turnCurve.Evaluate(Speed / maxSpeed) * (KartLikeBrake || IsDrift() ? driftMultiplier : 1f);
            float calculatedTurn = 5000f * mass * SteeringInput * ApplicationInfo.FixedDeltaTime * turn * turnMultiplyer;

            if (Grounded)
            {
                //turnlogic
                float sign = Mathf.Sign(carVelocity.z);

                if (AccelerationInput > 0.1f || carVelocity.z > 1)
                {
                    carBody.AddTorque(calculatedTurn * sign * Vector3.up);
                }
                else if (AccelerationInput < -0.1f || carVelocity.z < -1)
                {
                    carBody.AddTorque(calculatedTurn * sign * Vector3.up);
                }
            }
            else if (AirControl)
            {
                carBody.AddTorque(calculatedTurn * Vector3.up);
            }
        }

        public void BreakLogic()
        {
            if (Mathf.Abs(carVelocity.x) > 0)
            {
                //changes friction according to sideways speed of car
                frictionMaterial.dynamicFriction = Mathf.Lerp(minFriction, maxFriction, frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100)));
            }

            if (Grounded && !kartLike)
            {
                if (BrakeInput)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotationX;
                }
                else
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
            }
        }

        public void GravityLogic()
        {
            if (Grounded)
            {
                // down force
                rb.AddForce(downforce * rb.mass * -transform.up);
            }
            else
            {
                // gravity
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity + Vector3.down * gravity, ApplicationInfo.FixedDeltaTime * gravity);
            }
        }

        #endregion

        #region Visuals

        public void Visuals()
        {
            //tires
            foreach (Transform FW in FrontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                   30 * SteeringInput, FW.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / ApplicationInfo.FixedDeltaTime);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            RearWheels[0].localRotation = rb.transform.localRotation;
            RearWheels[1].localRotation = rb.transform.localRotation;

            //Body
            if (carVelocity.z > 1)
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / maxSpeed),
                                   BodyMesh.localRotation.eulerAngles.y, BodyTilt * SteeringInput), 0.4f * Time.deltaTime / ApplicationInfo.FixedDeltaTime);
            }
            else
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.4f * Time.deltaTime / ApplicationInfo.FixedDeltaTime);
            }


            if (KartLikeBrake || IsDrift())
            {
                BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                Quaternion.Euler(0, driftAngle * SteeringInput * Mathf.Sign(carVelocity.z), 0),
                0.1f * Time.deltaTime / ApplicationInfo.FixedDeltaTime);
            }
            else
            {
                BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                Quaternion.Euler(0, 0, 0),
                0.1f * Time.deltaTime / ApplicationInfo.FixedDeltaTime);
            }

            if (Grounded)
            {
                //body tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            }
            else
            {
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
            }
        }

        public void AudioManager()
        {
            //engineSound.pitch = Mathf.Lerp(MinPitch, MaxPitch, Mathf.Abs(carVelocity.z) / maxSpeed);
            engineSound.pitch = Mathf.LerpUnclamped(MinPitch, MaxPitch, Speed / maxSpeed);
            //print(carVelocity.magnitude);
            if (IsDrift() && Grounded)
            {
                skidSound.mute = false;
            }
            else
            {
                skidSound.mute = true;
            }

            engineSound.spatialBlend = isOwned && !network.AI ? spatialBlend : 1f;
            skidSound.spatialBlend = isOwned && !network.AI ? spatialBlend : 1f;
        }

        public void CameraManager()
        {
            float speedT = Speed / maxSpeed;

            float fov = Mathf.Lerp(MinFov, MaxFov, fovCurve.Evaluate(speedT));
            float amplitude = Mathf.Lerp(MinNoise, MaxNoise, amplitudeCurve.Evaluate(speedT));

            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, fov, (1f - smoothFov) * Time.deltaTime);

            // Noise
            if (noise == null && !cinemachineCamera.TryGetComponent<CinemachineBasicMultiChannelPerlin>(out noise))
                return;

            noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, amplitude, (1f - smoothNoise) * Time.deltaTime);
            noise.FrequencyGain = Mathf.Lerp(noise.AmplitudeGain, amplitude, (1f - smoothNoise) * Time.deltaTime);

            // Rotation Z
            if (lookAt == null && !cinemachineCamera.TryGetComponent<CinemachineLookAtOffset>(out lookAt))
                return;

            float xVelocityWithDrift = Mathf.Abs(carVelocity.x) - drift;

            float xVelocityRotation = Mathf.Clamp(xVelocityWithDrift, 0f, cameraRotationAngle) / cameraRotationAngle;
            float rotationZ = Mathf.Lerp(0f, cameraRotationAngle, cameraRotationCurve.Evaluate(xVelocityRotation));

            if (carVelocity.x > 0)
                rotationZ = -rotationZ;

            lookAt.RotationOffset.z = Mathf.Lerp(lookAt.RotationOffset.z, rotationZ, (1f - smoothCameraRotation) * Time.deltaTime);

            // Move x
            if (follow == null && !cinemachineCamera.TryGetComponent<CinemachineFollow>(out follow))
                return;

            float xVelocityMove = Mathf.Clamp(xVelocityWithDrift, 0f, cameraXMoveMaxVelocity) / cameraXMoveMaxVelocity;
            float moveX = Mathf.Lerp(0f, cameraXMove, cameraXMoveCurve.Evaluate(xVelocityMove));

            if (carVelocity.x > 0)
                moveX = -moveX;

            follow.FollowOffset.x = Mathf.Lerp(follow.FollowOffset.x, moveX, (1f - smoothCameraXMove) * Time.deltaTime);
        }

        #endregion


        public void GroundCheck() //checks for if vehicle is grounded or not
        {
            origin = rb.position + sphereCollider.radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = sphereCollider.radius + 0.2f;
            
            Grounded = groundCheck switch
            {
                GroundCheckType.RayCast => Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface),
                GroundCheckType.SphereCaste => Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface),
                _ => true
            };
        }

        public bool IsDrift() => Mathf.Abs(carVelocity.x) > drift;

        private void OnDrawGizmos()
        {
            //debug gizmos
            if (sphereCollider == null && !TryGetComponent(out sphereCollider))
                return;

            radius = sphereCollider.radius;
            float width = 0.02f;
            if (!Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rb.transform.position + ((radius + width) * Vector3.down), new Vector3(2 * radius, 2 * width, 4 * radius));
                if (TryGetComponent<BoxCollider>(out var collider))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(transform.position, collider.size);
                }
            }
        }
    }
}
