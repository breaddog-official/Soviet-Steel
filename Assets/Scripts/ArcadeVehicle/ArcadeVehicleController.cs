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
        public enum MovementMode { Velocity, AngularVelocity }


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



        [Space]
        public Rigidbody rb, carBody;

        public bool Grounded { get; private set; }

        [HideInInspector]
        public RaycastHit hit;
        public AnimationCurve frictionCurve;
        public AnimationCurve turnCurve;
        public PhysicsMaterial frictionMaterial;
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


        [HideInInspector]
        public float skidWidth;


        private SphereCollider sphereCollider;

        private bool handbrake;

        private float radius;
        private Vector3 origin;

        [SyncVar]
        private Vector2 moveInput;
        private bool brakeInput;

        private float AccelerationInput => moveInput.y;
        private float SteeringInput => moveInput.x;
        private bool BrakeInput => handbrake || brakeInput;

        private bool KartLikeBrake => kartLike && brakeInput;


        private void Start()
        {
            sphereCollider = rb.GetComponent<SphereCollider>();

            radius = sphereCollider.radius;

            rb.mass = mass;
            carBody.mass = mass;

            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
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
                if (movementMode == MovementMode.AngularVelocity)
                {
                    if (Mathf.Abs(AccelerationInput) > 0.1f && !BrakeInput && !kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, AccelerationInput * maxSpeed * carBody.transform.right / radius, accelaration * ApplicationInfo.FixedDeltaTime);
                    }
                    else if (Mathf.Abs(AccelerationInput) > 0.1f && kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, AccelerationInput * maxSpeed * carBody.transform.right / radius, accelaration * ApplicationInfo.FixedDeltaTime);
                    }
                }
                else if (movementMode == MovementMode.Velocity)
                {
                    if (Mathf.Abs(AccelerationInput) > 0.1f && !BrakeInput && !kartLike)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, AccelerationInput * maxSpeed * carBody.transform.forward, accelaration / 10 * ApplicationInfo.FixedDeltaTime);
                    }
                    else if (Mathf.Abs(AccelerationInput) > 0.1f && kartLike)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, AccelerationInput * maxSpeed * carBody.transform.forward, accelaration / 10 * ApplicationInfo.FixedDeltaTime);
                    }
                }
            }
        }

        public void TurnLogic()
        {
            float turnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / maxSpeed) * (KartLikeBrake || IsDrift() ? driftMultiplier : 1f);
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
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
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
            engineSound.pitch = Mathf.Lerp(MinPitch, MaxPitch, carVelocity.magnitude / maxSpeed);
            //print(carVelocity.magnitude);
            if (IsDrift() && Grounded)
            {
                skidSound.mute = false;
            }
            else
            {
                skidSound.mute = true;
            }

            engineSound.spatialBlend = isOwned ? spatialBlend : 1f;
            skidSound.spatialBlend = isOwned ? spatialBlend : 1f;
        }

        public void CameraManager()
        {
            float t = Mathf.Abs(carVelocity.z) / maxSpeed;

            float fov = Mathf.Lerp(MinFov, MaxFov, fovCurve.Evaluate(t));
            float amplitude = Mathf.Lerp(MinNoise, MaxNoise, amplitudeCurve.Evaluate(t));

            cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, fov, (1f - smoothFov) * Time.deltaTime);

            if (noise == null && !cinemachineCamera.TryGetComponent<CinemachineBasicMultiChannelPerlin>(out noise))
                return;

            noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, amplitude, (1f - smoothNoise) * Time.deltaTime);
            noise.FrequencyGain = Mathf.Lerp(noise.AmplitudeGain, amplitude, (1f - smoothNoise) * Time.deltaTime);
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
