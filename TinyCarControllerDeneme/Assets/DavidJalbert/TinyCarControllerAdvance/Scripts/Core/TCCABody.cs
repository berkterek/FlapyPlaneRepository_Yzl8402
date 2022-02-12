using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DavidJalbert.TinyCarControllerAdvance
{
    public class TCCABody : MonoBehaviour
    {
        public enum CounterMode
        {
            Never, Always, InAir, FullyGrounded, PartiallyGrounded, PartiallyOrFullyGrounded
        }

        [Header("Body parameters")]
        [Tooltip("The mass that will be applied to the body.")]
        public float bodyMass = 10;
        [Tooltip("Whether to apply interpolation to the body.")]
        public RigidbodyInterpolation rigidbodyInterpolation;
        [Tooltip("Which collision detection mode to use on the body.")]
        public CollisionDetectionMode collisionDetectionMode;
        [Tooltip("The center of mass of the body in local space. Ideally this should be the center of the car at ground level. Change the Z value to make the car lean backward or forward when in the air.")]
        public Vector3 centerOfMass = new Vector3(0, 0, 0);

        [Header("Roll countering")]
        [Tooltip("When to apply roll countering force.")]
        public CounterMode rollCounterMode = CounterMode.Always;
        [Tooltip("The angle in degrees to which to rotate the vehicle. Set to 0 to roll perfectly upright.")]
        public float rollCounterTargetAngle = 0;
        [Tooltip("How much force to apply to rotate the vehicle upright if it rolls over.")]
        public float rollCounterForce = 1;
        [Tooltip("How fast to rotate the vehicle upright if it rolls over. Set to zero to make this instantaneous.")]
        public float rollCounterSmoothing = 5;
        [Tooltip("How much force, between 0 (none) and 1 (max), to apply relative to the vehicle's speed, between 0 (stationary) and 1 (max speed).")]
        public AnimationCurve rollCounterOverSpeed = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        [Header("Pitch countering")]
        [Tooltip("When to apply pitch countering force.")]
        public CounterMode pitchCounterMode = CounterMode.InAir;
        [Tooltip("The angle in degrees to which to rotate the vehicle. Set to 0 to level perfectly straight.")]
        public float pitchCounterTargetAngle = 0;
        [Tooltip("How much force to apply to level the vehicle.")]
        public float pitchCounterForce = 1;
        [Tooltip("How fast to level the vehicle. Set to zero to make this instantaneous.")]
        public float pitchCounterSmoothing = 5;
        [Tooltip("How much force, between 0 (none) and 1 (max), to apply relative to the vehicle's speed, between 0 (stationary) and 1 (max speed).")]
        public AnimationCurve pitchCounterOverSpeed = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        private Rigidbody carBody;
        private bool wasInitialized = false;
        private TCCAPlayer parentPlayer = null;

        void FixedUpdate()
        {
            if (!wasInitialized) return;

            refresh(Time.fixedDeltaTime);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + centerOfMass);
            Gizmos.DrawWireSphere(transform.position + centerOfMass, 0.1f);
        }

        public Rigidbody getRigidbody()
        {
            if (carBody == null) carBody = GetComponentInChildren<Rigidbody>();
            return carBody;
        }

        public void initialize(TCCAPlayer parent)
        {
            parentPlayer = parent;

            if (getRigidbody() == null)
            {
                carBody = gameObject.AddComponent<Rigidbody>();
            }

            carBody.centerOfMass = centerOfMass;
            carBody.mass = bodyMass;
            carBody.drag = 0;
            carBody.angularDrag = 0;
            carBody.useGravity = true;
            carBody.isKinematic = false;
            carBody.interpolation = rigidbodyInterpolation;
            carBody.collisionDetectionMode = collisionDetectionMode;

            wasInitialized = true;
        }

        public void refresh(float deltaTime)
        {
            if (!wasInitialized) return;

            carBody.centerOfMass = centerOfMass;
            carBody.mass = bodyMass;
            carBody.angularDrag = 0;
            carBody.interpolation = rigidbodyInterpolation;
            carBody.collisionDetectionMode = collisionDetectionMode;

            Vector3 localAngular = getForwardAngularVelocity();

            if (rollCounterForce != 0 && canCounterRotation(rollCounterMode))
            {
                float rollForceDelta = rollCounterOverSpeed.Evaluate(getParentPlayer().getForwardVelocityDelta());
                float roll = Mathf.Clamp(getRollAngle() - rollCounterTargetAngle, -90f, 90f) / 90f;
                float maxRotation = Mathf.DeltaAngle(getRollAngle() - rollCounterTargetAngle, 0);
                float counterVelocity = -Mathf.Sign(roll) * Mathf.Min(Mathf.Abs(maxRotation), rollCounterForce) * rollForceDelta;
                localAngular.z = Mathf.Lerp(localAngular.z, counterVelocity, rollCounterSmoothing == 0 ? 1 : rollCounterSmoothing * deltaTime);
            }

            if (pitchCounterForce != 0 && canCounterRotation(pitchCounterMode))
            {
                float pitchForceDelta = pitchCounterOverSpeed.Evaluate(getParentPlayer().getForwardVelocityDelta());
                float pitch = Mathf.Clamp(getPitchAngle() - pitchCounterTargetAngle, -90f, 90f) / 90f;
                float maxRotation = Mathf.DeltaAngle(getPitchAngle() - pitchCounterTargetAngle, 0);
                float counterVelocity = -Mathf.Sign(pitch) * Mathf.Min(Mathf.Abs(maxRotation), pitchCounterForce) * pitchForceDelta;
                localAngular.x = Mathf.Lerp(localAngular.x, counterVelocity, pitchCounterSmoothing == 0 ? 1 : pitchCounterSmoothing * deltaTime);
            }

            setForwardAngularVelocity(localAngular);
        }

        public float getPitchAngle()
        {
            return Mathf.DeltaAngle(0, Vector3.Angle(carBody.transform.forward, Vector3.up) - 90f);
        }

        public float getRollAngle()
        {
            return Mathf.DeltaAngle(0, Vector3.Angle(carBody.transform.right, Vector3.down) - 90f);
        }

        public bool canCounterRotation(CounterMode m)
        {
            bool canCounter = false;
            switch (m)
            {
                case CounterMode.Never: canCounter = false; break;
                case CounterMode.Always: canCounter = true; break;
                case CounterMode.InAir: canCounter = !getParentPlayer().isGrounded(); break;
                case CounterMode.FullyGrounded: canCounter = getParentPlayer().isFullyGrounded(); break;
                case CounterMode.PartiallyGrounded: canCounter = getParentPlayer().isPartiallyGrounded(); break;
                case CounterMode.PartiallyOrFullyGrounded: canCounter = getParentPlayer().isGrounded(); break;
            }
            return canCounter;
        }

        public Vector3 getForwardAngularVelocity()
        {
            return carBody.transform.InverseTransformDirection(carBody.angularVelocity);
        }

        public void setForwardAngularVelocity(Vector3 v)
        {
            carBody.angularVelocity = carBody.transform.TransformDirection(v);
        }

        public float getForwardVelocity()
        {
            return Vector3.Dot(carBody.velocity, carBody.transform.forward);
        }

        public float getLateralVelocity()
        {
            return Vector3.Dot(carBody.velocity, carBody.transform.right);
        }

        public float getVerticalVelocity()
        {
            return Vector3.Dot(carBody.velocity, carBody.transform.up);
        }

        public TCCAPlayer getParentPlayer()
        {
            return parentPlayer;
        }

        public Vector3 getPosition()
        {
            return carBody.position;
        }

        public Quaternion getRotation()
        {
            return carBody.rotation;
        }

        public void setPosition(Vector3 position)
        {
            carBody.position = position;
        }

        public void setRotation(Quaternion rotation)
        {
            carBody.rotation = rotation;
        }

        public void translate(Vector3 offset)
        {
            setPosition(getPosition() + offset);
        }

        public void immobilize()
        {
            carBody.velocity = Vector3.zero;
            carBody.angularVelocity = Vector3.zero;
        }

        public void setParent(Transform parent)
        {
            carBody.transform.SetParent(parent, true);
        }
    }
}