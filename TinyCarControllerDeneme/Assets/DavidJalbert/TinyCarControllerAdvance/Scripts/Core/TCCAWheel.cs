using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert.TinyCarControllerAdvance
{
    public class TCCAWheel : MonoBehaviour
    {
        [Header("Physics")]
        [Tooltip("Radius of the wheel collider. This should be equal to the size of the wheel model.")]
        public float wheelRadius = 1f;
        [Tooltip("Mass of the wheel rigidbody.")]
        public float wheelMass = 1f;
        [Tooltip("Material of the wheel rigidbody.")]
        public PhysicMaterial wheelMaterial;
        [Tooltip("Whether to use interpolation for the wheel rigidbody.")]
        public RigidbodyInterpolation rigidbodyInterpolation;
        [Tooltip("Which collision detection mode to use for the wheel collider.")]
        public CollisionDetectionMode collisionDetectionMode;
        [Header("Parameters", order = 1)]
        [Header("Suspension", order = 2)]
        [Tooltip("Force applied to the suspension. Higher values make the suspension stiffer.")]
        public float suspensionSpring = 1000;
        [Tooltip("Damper applied to the suspension. Higher values make the suspension settle faster.")]
        public float suspensionDamper = 20;
        [Tooltip("Shifts the position of the wheel relative to its initial position. Useful to mimic hydraulics.")]
        public Vector3 suspensionOffset = Vector3.zero;
        [Header("Steering")]
        [Tooltip("Whether to allow the wheel to turn left or right.")]
        public bool steeringEnabled = true;
        [Tooltip("Maximum angle at which the wheel can turn.")]
        public float steeringMaxAngle = 30;
        [Tooltip("How much steering, between 0 (none) and 1 (max), to apply relative to the vehicle's speed, between 0 (stationary) and 1 (max speed).")]
        public AnimationCurve steeringOverSpeed = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        [Tooltip("The amount of force to apply to the steering axle.")]
        public float steeringSpring = 1000;
        [Tooltip("The amount of friction to apply to the steering axle.")]
        public float steeringDamper = 10;
        [Tooltip("Whether to invert steering. Useful for the rear wheels if you want to have a four wheel steering.")]
        public bool invertSteering = false;
        [Header("Motor")]
        [Tooltip("Whether to allow the wheel to accelerate.")]
        public bool motorEnabled = true;
        [Tooltip("Maximum speed to which the wheel can keep accelerating when using the motor.")]
        public float motorMaxSpeed = 50f;
        [Tooltip("Maximum acceleration to apply to the wheel when using the motor.")]
        public float motorAcceleration = 500f;
        [Tooltip("How much acceleration, between 0 (none) and 1 (max), to apply relative to the vehicle's speed, between 0 (stationary) and 1 (max speed).")]
        public AnimationCurve motorAccelerationOverSpeed = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        [Tooltip("Speed at which the wheel will decelerate when not accelerating.")]
        public float motorDrag = 100f;
        [Tooltip("Whether to allow the wheel to use the handbrake.")]
        public bool handbrakeEnabled = true;

        private float inputSteering;
        private float inputMotor;

        private GameObject wheelRootObject;
        private Rigidbody carBody;
        private TCCACollider internalCollider;
        private GameObject springObject;
        private Rigidbody springBody;
        private ConfigurableJoint springJoint;
        private GameObject axleObject;
        private Rigidbody axleBody;
        private SphereCollider axleCollider;
        private ConfigurableJoint axleJoint;
        private GameObject steeringObject;
        private Rigidbody steeringBody;
        private HingeJoint steeringHinge;
        private SoftJointLimit springLimit;
        private SoftJointLimitSpring springLimitSpring;
        private Vector3 initialAnchor;
        private bool usingHandbrake = false;
        private bool touchingGroundLastFrame = false;
        private float motorMaxSpeedMultiplier = 1;
        private float motorAccelerationMultiplier = 1f;
        private TCCAPlayer parentPlayer;
        private bool wasInitialized = false;

        public virtual void onCollisionStay(Collision collision) { }
        public virtual void onCollisionEnter(Collision collision) { }
        public virtual void onCollisionExit(Collision collision) { }
        public virtual void onTriggerStay(Collider other) { }
        public virtual void onTriggerEnter(Collider other) { }
        public virtual void onTriggerExit(Collider other) { }

        public void initialize(TCCAPlayer parent)
        {
            parentPlayer = parent;

            carBody = parentPlayer.getRigidbody();
            initialAnchor = transform.localPosition;

            wheelRootObject = new GameObject(name + " wheel collider");
            wheelRootObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            wheelRootObject.transform.SetParent(parentPlayer.transform);

            // spring objects

            springObject = new GameObject(name + " spring");
            springObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            springObject.transform.SetParent(wheelRootObject.transform);
            springObject.transform.position = transform.position;
            springObject.transform.rotation = transform.rotation;

            springBody = springObject.AddComponent<Rigidbody>();
            springBody.mass = wheelMass;
            springBody.drag = 0;
            springBody.angularDrag = 0;
            springBody.useGravity = true;
            springBody.isKinematic = false;
            springBody.interpolation = rigidbodyInterpolation;
            springBody.collisionDetectionMode = collisionDetectionMode;

            springJoint = springObject.AddComponent<ConfigurableJoint>();
            springJoint.connectedBody = carBody;
            springJoint.anchor = Vector3.zero;
            springJoint.axis = Vector3.right;
            springJoint.secondaryAxis = Vector3.up;
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = initialAnchor + suspensionOffset;
            springJoint.enableCollision = false;
            springJoint.enablePreprocessing = true;
            springJoint.xMotion = ConfigurableJointMotion.Locked;
            springJoint.yMotion = ConfigurableJointMotion.Limited;
            springJoint.zMotion = ConfigurableJointMotion.Locked;
            springJoint.angularXMotion = ConfigurableJointMotion.Locked;
            springJoint.angularYMotion = ConfigurableJointMotion.Locked;
            springJoint.angularZMotion = ConfigurableJointMotion.Locked;
            springLimitSpring = new SoftJointLimitSpring();
            springLimitSpring.spring = suspensionSpring;
            springLimitSpring.damper = suspensionDamper;
            springJoint.linearLimitSpring = springLimitSpring;
            springLimit = new SoftJointLimit();
            springLimit.limit = 0.00001f;
            springLimit.bounciness = 0;
            springLimit.contactDistance = 1000;
            springJoint.linearLimit = springLimit;

            // steering objects

            steeringObject = new GameObject(name + " steering");
            steeringObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            steeringObject.transform.SetParent(wheelRootObject.transform);
            steeringObject.transform.position = transform.position;
            steeringObject.transform.rotation = transform.rotation;

            steeringBody = steeringObject.AddComponent<Rigidbody>();
            steeringBody.mass = wheelMass;
            steeringBody.drag = 0;
            steeringBody.angularDrag = 0;
            steeringBody.useGravity = true;
            steeringBody.isKinematic = false;
            steeringBody.interpolation = rigidbodyInterpolation;
            steeringBody.collisionDetectionMode = collisionDetectionMode;

            steeringHinge = steeringObject.AddComponent<HingeJoint>();
            steeringHinge.connectedBody = springBody;
            steeringHinge.anchor = Vector3.zero;
            steeringHinge.axis = Vector3.up;
            steeringHinge.autoConfigureConnectedAnchor = false;
            steeringHinge.connectedAnchor = new Vector3(0, 0, 0);
            steeringHinge.useLimits = false;
            steeringHinge.useMotor = false;
            steeringHinge.useSpring = false;
            steeringHinge.enableCollision = false;
            steeringHinge.enablePreprocessing = true;

            // axle objects

            axleObject = new GameObject(name + " axle");
            axleObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            axleObject.transform.SetParent(wheelRootObject.transform);
            axleObject.transform.position = transform.position;
            axleObject.transform.rotation = transform.rotation;

            axleBody = axleObject.AddComponent<Rigidbody>();
            axleBody.maxAngularVelocity = float.MaxValue;
            axleBody.mass = wheelMass;
            axleBody.drag = 0;
            axleBody.angularDrag = 0;
            axleBody.useGravity = true;
            axleBody.isKinematic = false;
            axleBody.interpolation = rigidbodyInterpolation;
            axleBody.collisionDetectionMode = collisionDetectionMode;
            
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }

            axleCollider = axleObject.AddComponent<SphereCollider>();
            axleCollider.isTrigger = false;
            axleCollider.material = wheelMaterial;
            axleCollider.center = Vector3.zero;
            axleCollider.radius = wheelRadius;

            foreach (Collider c in carBody.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(axleCollider, c);
            }

            axleJoint = axleObject.AddComponent<ConfigurableJoint>();
            axleJoint.connectedBody = steeringBody;
            axleJoint.anchor = Vector3.zero;
            axleJoint.axis = Vector3.right;
            axleJoint.secondaryAxis = Vector3.up;
            axleJoint.autoConfigureConnectedAnchor = false;
            axleJoint.connectedAnchor = new Vector3(0, 0, 0);
            axleJoint.enableCollision = false;
            axleJoint.enablePreprocessing = true;
            axleJoint.xMotion = ConfigurableJointMotion.Locked;
            axleJoint.yMotion = ConfigurableJointMotion.Locked;
            axleJoint.zMotion = ConfigurableJointMotion.Locked;
            axleJoint.angularXMotion = ConfigurableJointMotion.Free;
            axleJoint.angularYMotion = ConfigurableJointMotion.Locked;
            axleJoint.angularZMotion = ConfigurableJointMotion.Locked;

            internalCollider = axleObject.AddComponent<TCCACollider>();
            internalCollider.initialize(this);

            wasInitialized = true;
        }

        public void refresh()
        {
            if (!wasInitialized) return;

            springBody.mass = wheelMass;
            springBody.interpolation = rigidbodyInterpolation;
            springBody.collisionDetectionMode = collisionDetectionMode;
            springLimitSpring.spring = suspensionSpring;
            springLimitSpring.damper = suspensionDamper;
            springJoint.linearLimitSpring = springLimitSpring;
            springJoint.connectedAnchor = initialAnchor + suspensionOffset;
            steeringBody.mass = wheelMass;
            steeringBody.interpolation = rigidbodyInterpolation;
            steeringBody.collisionDetectionMode = collisionDetectionMode;
            axleBody.mass = wheelMass;
            axleBody.interpolation = rigidbodyInterpolation;
            axleBody.collisionDetectionMode = collisionDetectionMode;
            axleCollider.material = wheelMaterial;
            axleCollider.radius = wheelRadius;
            axleBody.angularDrag = 0;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, wheelRadius);
        }

        void Update()
        {
            if (!wasInitialized) return;

            transform.position = axleObject.transform.position;
            transform.rotation = axleObject.transform.rotation;
        }

        void FixedUpdate()
        {
            if (!wasInitialized) return;

            refresh();

            touchingGroundLastFrame = internalCollider.isTouchingGround();

            float accelerationDelta = motorAccelerationOverSpeed.Evaluate(getParentPlayer().getForwardVelocityDelta());
            float adjustedInputSteering = inputSteering * (invertSteering ? -1 : 1);
            float motor = inputMotor;
            float axleVelocity = getForwardSpinVelocity();
            float adjustedMotorMaxSpeed = motorMaxSpeed * motorMaxSpeedMultiplier;

            // axle (engine, moving forward and back)

            if (handbrakeEnabled && usingHandbrake)
            {
                axleBody.constraints = RigidbodyConstraints.FreezeRotationX;
            }
            else
            {
                axleBody.constraints = RigidbodyConstraints.None;

                if (motorEnabled)
                {
                    float motorCurrentAcceleration = Mathf.Lerp(motorDrag, motorAcceleration * motorAccelerationMultiplier * accelerationDelta, Mathf.Abs(motor));
                    axleVelocity = Mathf.MoveTowards(axleVelocity, motor * adjustedMotorMaxSpeed, motorCurrentAcceleration * Time.fixedDeltaTime);
                    setForwardSpinVelocity(axleVelocity);
                }
            }

            // steering (moving left and right)

            float steeringDelta = steeringOverSpeed.Evaluate(getParentPlayer().getForwardVelocityDelta());

            steeringHinge.useLimits = false;
            steeringHinge.useMotor = false;
            steeringHinge.useSpring = true;
            JointSpring js = steeringHinge.spring;
            float targetSteeringAngle = 0;
            if (steeringEnabled)
            {
                targetSteeringAngle = adjustedInputSteering * steeringMaxAngle * steeringDelta;
            }
            js.spring = steeringSpring;
            js.damper = steeringDamper;
            js.targetPosition = targetSteeringAngle;
            steeringHinge.spring = js;

            //

            internalCollider.resetCollision();
        }

        public TCCAPlayer getParentPlayer()
        {
            return parentPlayer;
        }

        public void setSpeedMultiplier(float m)
        {
            motorMaxSpeedMultiplier = m;
        }

        public void setAccelerationMultiplier(float m)
        {
            motorAccelerationMultiplier = m;
        }

        public SphereCollider getCollider()
        {
            return axleCollider;
        }

        public bool isTouchingGround()
        {
            return touchingGroundLastFrame;
        }

        public void setSteering(float value)
        {
            inputSteering = value;
        }

        public void setMotor(float value)
        {
            inputMotor = value;
        }

        public void setHandbrake(bool e)
        {
            usingHandbrake = e;
        }

        public float getSteering()
        {
            return inputSteering;
        }

        public float getMotor()
        {
            return inputMotor;
        }

        public Vector3 getForwardAngularVelocity()
        {
            return steeringBody.transform.InverseTransformDirection(axleBody.angularVelocity);
        }

        public void setForwardAngularVelocity(Vector3 v)
        {
            axleBody.angularVelocity = steeringBody.transform.TransformDirection(v);
        }

        public float getForwardSpinVelocity()
        {
            float angularVelocity = getForwardAngularVelocity().x;
            return axleCollider.radius * angularVelocity;
        }

        public void setForwardSpinVelocity(float v)
        {
            Vector3 angularVelocity = getForwardAngularVelocity();
            angularVelocity.x = v / axleCollider.radius;
            setForwardAngularVelocity(angularVelocity);
        }

        public float getForwardVelocity()
        {
            return getRelativeVelocity().z;
        }

        public float getSideVelocity()
        {
            return getRelativeVelocity().x;
        }

        public Vector3 getRelativeVelocity()
        {
            return steeringBody.transform.InverseTransformDirection(steeringBody.velocity);
        }

        public void immobilize()
        {
            axleBody.velocity = Vector3.zero;
            axleBody.angularVelocity = Vector3.zero;
            springBody.velocity = Vector3.zero;
            springBody.angularVelocity = Vector3.zero;
            steeringBody.velocity = Vector3.zero;
            steeringBody.angularVelocity = Vector3.zero;
        }

        public Vector3 getPosition()
        {
            return axleBody.position;
        }

        public Quaternion getRotation()
        {
            return axleBody.rotation;
        }

        public void translate(Vector3 offset)
        {
            axleBody.position += offset;
            springBody.position += offset;
            steeringBody.position += offset;
        }

        public void setParent(Transform parent)
        {
            wheelRootObject.transform.SetParent(parent, true);
        }
    }
}