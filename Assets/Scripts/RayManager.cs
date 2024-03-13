using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;

public class RayManager : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private ObjImporter objectImporter;
    private RayInteractor rayInteractor;

    private void Awake()
    {
        rayInteractor = GetComponent<RayInteractor>();
    }

    private void Start()
    {
        inputManager.Input_OnReleaseButton += OnReleaseButton;
    }

    private void OnReleaseButton()
    {
        Debug.Log("Release Button");
        if (rayInteractor.CollisionInfo.HasValue)
        {
            SurfaceHit collision = rayInteractor.CollisionInfo.Value;

            // Vector3 hitPosition = collision.Point + Vector3.up * 3;
            Vector3 hitPosition = collision.Point + Vector3.up;
            objectImporter.ImportObject(hitPosition);
            // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // cube.AddComponent<Rigidbody>();
            // cube.AddComponent<XRGrabInteractable>();
            // cube.transform.position = hitPosition + Vector3.up * 3;
            // cube.transform.localScale = cube.transform.localScale * 0.2f;
        }
    }
}
