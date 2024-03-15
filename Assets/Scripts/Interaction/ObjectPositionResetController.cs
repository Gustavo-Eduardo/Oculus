using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPositionResetController : MonoBehaviour {

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start() {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void Update() {
        if (OVRInput.GetDown(OVRInput.Button.One)) {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            if (TryGetComponent<Rigidbody>(out var rigidbody)) {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
