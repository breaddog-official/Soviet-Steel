using UnityEngine;
using Unity.Burst;
using Mirror;

namespace ArcadeVP
{
    [BurstCompile]
    public class ArcadeVehicleController : NetworkBehaviour
    {
        public enum GroundCheck { None, RayCast, SphereCaste }
        public enum MovementMode { Velocity, AngularVelocity }


        public MovementMode movementMode;
        public GroundCheck groundCheck;
        public LayerMask drivableSurface;

        public float MaxSpeed, accelaration, turn, gravity = 7f, downforce = 5f;
        [Tooltip("if true : can turn vehicle in air")]
        public bool AirControl = false;
        [Tooltip("if true : vehicle will drift instead of brake while holding space")]
        public bool kartLike = false;
        [Tooltip("turn more while drifting (while holding space) only if kart Like is true")]
        public float driftMultiplier = 1.5f;

        [Space]
        public bool smoothInput = true;
        public float smoothTime = 1;
        public float smoothDifference = 0.7f;

        private float currentSmoothTime = 0;

        [Space]
        public Rigidbody rb, carBody;

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
        [Range(0, 1)]
        public float minPitch;
        [Range(1, 3)]
        public float MaxPitch;
        public AudioSource SkidSound;

        [HideInInspector]
        public float skidWidth;


        private SphereCollider sphereCollider;

        private bool handbrake;

        private float radius;
        private Vector3 origin;

        [SyncVar]
        private Vector2 smoothedMoveInput;
        private Vector2 moveInput;
        private bool brakeInput;

        private float AccelerationInput => smoothedMoveInput.y;
        private float SteeringInput => smoothedMoveInput.x;
        private bool BrakeInput => handbrake || brakeInput;


        private void Start()
        {
            sphereCollider = rb.GetComponent<SphereCollider>();

            radius = sphereCollider.radius;

            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
        }

        private void Update()
        {
            Visuals();
            AudioManager();
            SmoothInput();
        }

        public void ProvideInputs(Vector2 _moveInput, bool _brakeInput)
        {
            if (moveInput != _moveInput && (moveInput - _moveInput).magnitude > smoothDifference)
                currentSmoothTime = 0f;

            moveInput = _moveInput;
            brakeInput = _brakeInput;
        }

        public void SmoothInput()
        {
            if (NetworkServer.active && !isOwned)
                return;

            smoothedMoveInput = smoothInput ? Vector2.Lerp(smoothedMoveInput, moveInput, currentSmoothTime / smoothTime) : moveInput;
            currentSmoothTime += Time.deltaTime;
        }

        public void AudioManager()
        {
            engineSound.pitch = Mathf.Lerp(minPitch, MaxPitch, Mathf.Abs(carVelocity.z) / MaxSpeed);
            if (Mathf.Abs(carVelocity.x) > 10 && grounded())
            {
                SkidSound.mute = false;
            }
            else
            {
                SkidSound.mute = true;
            }
        }

        public void Handbrake(bool state)
        {
            handbrake = state;
        }





        void FixedUpdate()
        {
            if (NetworkServer.active && !isOwned)
                return;

            carVelocity = carBody.transform.InverseTransformDirection(carBody.linearVelocity);

            if (Mathf.Abs(carVelocity.x) > 0)
            {
                //changes friction according to sideways speed of car
                frictionMaterial.dynamicFriction = frictionCurve.Evaluate(Mathf.Abs(carVelocity.x / 100));
            }


            if (grounded())
            {
                //turnlogic
                float sign = Mathf.Sign(carVelocity.z);
                float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);
                if (kartLike && BrakeInput) { TurnMultiplyer *= driftMultiplier; } //turn more if drifting


                if (AccelerationInput > 0.1f || carVelocity.z > 1)
                {
                    carBody.AddTorque(Vector3.up * SteeringInput * sign * turn * 100 * TurnMultiplyer);
                }
                else if (AccelerationInput < -0.1f || carVelocity.z < -1)
                {
                    carBody.AddTorque(Vector3.up * SteeringInput * sign * turn * 100 * TurnMultiplyer);
                }



                // mormal brakelogic
                if (!kartLike)
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

                //accelaration logic

                if (movementMode == MovementMode.AngularVelocity)
                {
                    if (Mathf.Abs(AccelerationInput) > 0.1f && !BrakeInput && !kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * AccelerationInput * MaxSpeed / radius, accelaration * Time.deltaTime);
                    }
                    else if (Mathf.Abs(AccelerationInput) > 0.1f && kartLike)
                    {
                        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, carBody.transform.right * AccelerationInput * MaxSpeed / radius, accelaration * Time.deltaTime);
                    }
                }
                else if (movementMode == MovementMode.Velocity)
                {
                    if (Mathf.Abs(AccelerationInput) > 0.1f && !BrakeInput && !kartLike)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, carBody.transform.forward * AccelerationInput * MaxSpeed, accelaration / 10 * Time.deltaTime);
                    }
                    else if (Mathf.Abs(AccelerationInput) > 0.1f && kartLike)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, carBody.transform.forward * AccelerationInput * MaxSpeed, accelaration / 10 * Time.deltaTime);
                    }
                }

                // down froce
                rb.AddForce(-transform.up * downforce * rb.mass);

                //body tilt
                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, hit.normal) * carBody.transform.rotation, 0.12f));
            }
            else
            {
                if (AirControl)
                {
                    //turnlogic
                    float TurnMultiplyer = turnCurve.Evaluate(carVelocity.magnitude / MaxSpeed);

                    carBody.AddTorque(Vector3.up * SteeringInput * turn * 100 * TurnMultiplyer);
                }

                carBody.MoveRotation(Quaternion.Slerp(carBody.rotation, Quaternion.FromToRotation(carBody.transform.up, Vector3.up) * carBody.transform.rotation, 0.02f));
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity + Vector3.down * gravity, Time.deltaTime * gravity);
            }

        }
        public void Visuals()
        {
            //tires
            foreach (Transform FW in FrontWheels)
            {
                FW.localRotation = Quaternion.Slerp(FW.localRotation, Quaternion.Euler(FW.localRotation.eulerAngles.x,
                                   30 * SteeringInput, FW.localRotation.eulerAngles.z), 0.7f * Time.deltaTime / Time.fixedDeltaTime);
                FW.GetChild(0).localRotation = rb.transform.localRotation;
            }
            RearWheels[0].localRotation = rb.transform.localRotation;
            RearWheels[1].localRotation = rb.transform.localRotation;

            //Body
            if (carVelocity.z > 1)
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(Mathf.Lerp(0, -5, carVelocity.z / MaxSpeed),
                                   BodyMesh.localRotation.eulerAngles.y, BodyTilt * SteeringInput), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }
            else
            {
                BodyMesh.localRotation = Quaternion.Slerp(BodyMesh.localRotation, Quaternion.Euler(0, 0, 0), 0.4f * Time.deltaTime / Time.fixedDeltaTime);
            }


            if (kartLike)
            {
                if (BrakeInput)
                {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 45 * SteeringInput * Mathf.Sign(carVelocity.z), 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }
                else
                {
                    BodyMesh.parent.localRotation = Quaternion.Slerp(BodyMesh.parent.localRotation,
                    Quaternion.Euler(0, 0, 0),
                    0.1f * Time.deltaTime / Time.fixedDeltaTime);
                }

            }

        }

        public bool grounded() //checks for if vehicle is grounded or not
        {
            origin = rb.position + sphereCollider.radius * Vector3.up;
            var direction = -transform.up;
            var maxdistance = sphereCollider.radius + 0.2f;

            return groundCheck switch
            {
                GroundCheck.RayCast => Physics.Raycast(rb.position, Vector3.down, out hit, maxdistance, drivableSurface),
                GroundCheck.SphereCaste => Physics.SphereCast(origin, radius + 0.1f, direction, out hit, maxdistance, drivableSurface),
                _ => true
            };
        }

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
