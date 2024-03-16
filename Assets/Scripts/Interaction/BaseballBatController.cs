using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Rendering;

namespace DreamXR.Interaction {
    public class BaseballBatController : MonoBehaviour {
        [Tooltip("The left hand grab anchor. This is the capsule collider in hand called 'GrabVolumeBig'")]
        public GameObject leftHandGrabAnchor;
        [Tooltip("The right hand grab anchor. This is the capsule collider in hand called 'GrabVolumeBig'")]
        public GameObject rightHandGrabAnchor;

        private const int STORE_THROWING_FRAMES_COUNT = 10;
        private const float LINEAR_VELOCITY_MULTIPLIER = 1.5f;
        private const float ANGULAR_VELOCITY_MULTIPLIER = 0.5f;
        // Anything hit above this magnitude threshold with the bat will be instantly destroyed.
        private const float LINEAR_VELOCITY_MAGNITUDE_THRESHOLD_FOR_DESTRUCTION = 50f;
        // The index of the last velocity frame stored.
        int currentVelocityFrameIndex = 0;
        // These dictionaries keep last STORE_THROWING_FRAMES_COUNT of throwing frames data to better calculate
        // throwing objects by player.
        Dictionary<OVRInput.Controller, ThrowingFrame[]> handToThrowingFrames;
        // Center of mass for both controllers are kept cached to compute realistic throwing.
        private Vector3 rightHandCenterOfMass;
        private Vector3 leftHandCenterOfMass;

        void Awake() {
            handToThrowingFrames = new Dictionary<OVRInput.Controller, ThrowingFrame[]>();
            handToThrowingFrames.Add(OVRInput.Controller.RTouch, new ThrowingFrame[STORE_THROWING_FRAMES_COUNT]);
            handToThrowingFrames.Add(OVRInput.Controller.LTouch, new ThrowingFrame[STORE_THROWING_FRAMES_COUNT]);
        }

        void Start() {
            /*
            rightHandCenterOfMass = rightHandGrabAnchor.GetComponent<Rigidbody>().centerOfMass;
            leftHandCenterOfMass = leftHandGrabAnchor.GetComponent<Rigidbody>().centerOfMass;
            */
            rightHandCenterOfMass = rightHandGrabAnchor.transform.position;
            leftHandCenterOfMass = leftHandGrabAnchor.transform.position;
        }

        // Update is called once per frame
        void Update() {
            // TODO(elhacker): it currently only works when the bat is on the right hand.
            ManageThrowing(OVRInput.Controller.RTouch);
            //ManageThrowing(OVRInput.Controller.LTouch);
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.TryGetComponent<Rigidbody>(out var otherRigidBody)) {
                Vector3 totalLinearVelocity = _throwAngularVelocity * LINEAR_VELOCITY_MULTIPLIER;
                if (totalLinearVelocity.magnitude >= LINEAR_VELOCITY_MAGNITUDE_THRESHOLD_FOR_DESTRUCTION) {
                    // If hit is too hard, the hitted object is instantly destroyed.
                    Destroy(other.gameObject);
                } else {
                    otherRigidBody.velocity = totalLinearVelocity;
                    otherRigidBody.angularVelocity = -_throwAngularVelocity * ANGULAR_VELOCITY_MULTIPLIER;
                    ParticleManager.PlayAndFollow("SpikyFireTrail", other.transform);
                }
            }
        }

        void FixedUpdate() {
            FixedThrowVelocityUpdate();
        }

        private void FixedThrowVelocityUpdate() {
            if (handToThrowingFrames != null) {
                currentVelocityFrameIndex++;
                currentVelocityFrameIndex = currentVelocityFrameIndex % STORE_THROWING_FRAMES_COUNT;
                StoreThrowingFramesForHand(OVRInput.Controller.RTouch, currentVelocityFrameIndex);
                StoreThrowingFramesForHand(OVRInput.Controller.LTouch, currentVelocityFrameIndex);
            }
        }

        private void StoreThrowingFramesForHand(OVRInput.Controller hand, int frameIndex) {
            OVRPose trackingSpace = transform.ToOVRPose();
            GameObject handGrabAnchor = hand == OVRInput.Controller.LTouch ? leftHandGrabAnchor : rightHandGrabAnchor;
            ThrowingFrame throwingFrame = new ThrowingFrame();
            throwingFrame.Position = handGrabAnchor.transform.position;
            throwingFrame.LinearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(hand);
            throwingFrame.AngularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(hand);
            throwingFrame.CapturedTime = Time.time;
            handToThrowingFrames[hand][frameIndex] = throwingFrame;
        }

         private bool _leftHandThrowing;
        private bool _rightHandThrowing;
        private Vector3 _throwLinearVelocity;
        private Vector3 _throwAngularVelocity;
        private bool _primaryIndexTriggerLeft;
        private bool _primaryIndexTriggerRight;
        private void ManageThrowing(OVRInput.Controller controller) {
            bool isThrowing = true;
            if (controller == OVRInput.Controller.LTouch) {
                _leftHandThrowing |= isThrowing;
            }
            if (controller == OVRInput.Controller.RTouch) {
                _rightHandThrowing |= isThrowing;
            }
            if (isThrowing) {
                // With realistic throw.
                Vector3 controllerCenterOfMass = controller == OVRInput.Controller.LTouch ? leftHandCenterOfMass : rightHandCenterOfMass;

                // Set the linear and angular velocity on item throw.
                OVRPose trackingSpace = transform.ToOVRPose();
                Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(controller);
                Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(controller);

                // TODO(elhacker): Might need to use collision.position instead of the transform.localPosition here.
                // More realistic throwing behaviour
                Vector3 directionToGrabbedObject = transform.localPosition - controllerCenterOfMass;
                Vector3 controlVelocityCross = Vector3.Cross(angularVelocity, directionToGrabbedObject);
                Vector3 fullThrowLinearVelocity = linearVelocity + controlVelocityCross;

                // Apply exponential smoothing strategy for throwing.
                fullThrowLinearVelocity += GetExponentialSmoothingVelocityOnHand(controller);
                angularVelocity += GetAngularVelocityAverageOnHand(controller, 5);

                _throwLinearVelocity = fullThrowLinearVelocity;
                _throwAngularVelocity = -angularVelocity;
            }
        }

        // Implements Exponential Smoothing algorithm to reduce signal noise
        private Vector3 GetExponentialSmoothingVelocityOnHand(OVRInput.Controller hand) {
            float smoothingFactor = 0.8f;
            int smoothOverFrames = 5;
            Vector3 linearVelocity = Vector3.zero;
            ThrowingFrame[] frames = handToThrowingFrames[hand];
            int startIdx = GetPastCircularIndex(frames.Length, currentVelocityFrameIndex, smoothOverFrames);
            for (int i = startIdx, j = 0; j < smoothOverFrames; j++, i = (i + 1) % frames.Length) {
                linearVelocity = linearVelocity + smoothingFactor * (frames[i].LinearVelocity - linearVelocity);
            }
            return linearVelocity;
        }

        // Returns the start of the range requested in the past
        // in a circular array. For example if arrayLength = 10,
        // currentIdx = 1 and pastCount = 3. It will return 8.
        private int GetPastCircularIndex(int arrayLength, int currentIdx, int pastCount) {
            if (arrayLength < 0) return -1;
            if (arrayLength == 0) return 0;
            if (currentIdx >= arrayLength) return -1;
            if (pastCount > arrayLength) return -1;
            int diff = currentIdx - pastCount;
            if (diff >= 0) {
                return diff;
            } else {
                return arrayLength + diff;
            }
        }

         private Vector3 GetAngularVelocityAverageOnHand(OVRInput.Controller controller, int frameCount) {
            if (frameCount > STORE_THROWING_FRAMES_COUNT) frameCount = STORE_THROWING_FRAMES_COUNT;
            ThrowingFrame[] frames = handToThrowingFrames[controller];
            Vector3[] angularVelocity = new Vector3[frameCount];
            int startIdx = GetPastCircularIndex(frames.Length, currentVelocityFrameIndex, frameCount);
            for (int i = startIdx, j = 0; j < frameCount; j++, i = (i + 1) % frames.Length) {
                angularVelocity[j] = frames[i].AngularVelocity;
            }
            return GetVectorAverage(angularVelocity);
        }

        private Vector3 GetVectorAverage(Vector3[] vectors) {
            Vector3 average = Vector3.zero;
            foreach (Vector3 vector in vectors) {
                average = average + vector;
            }
            average = average / ((float)vectors.Length);
            return average;
        }

        // Stores the data required to compute multiple throwing strategies like averaging, bracketing highest speed, and linear regression.
        private struct ThrowingFrame {
            public Vector3 Position;
            public Vector3 LinearVelocity;
            public Vector3 AngularVelocity;
            public float CapturedTime; // When was this frame captured from controller input. FPS independent.

            public ThrowingFrame(Vector3 position, Vector3 linearVelocity, Vector3 angularVelocity, float capturedTime) {
                Position = position;
                LinearVelocity = linearVelocity;
                AngularVelocity = angularVelocity;
                CapturedTime = capturedTime;
            }
        }
    }

}
