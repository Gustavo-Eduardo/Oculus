using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float distance = 5f;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.3f;

    private void Update()
    {
        Vector3 targetPosition = mainCamera.transform.TransformPoint(new Vector3(0, 0, distance));

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
        var lookAtPos = new Vector3(
            mainCamera.transform.position.x,
            transform.position.y,
            mainCamera.transform.position.z
        );
        transform.LookAt(lookAtPos);
    }
}
