using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnLastMesh : MonoBehaviour
{
    [SerializeField] ObjImporter objImporter;
    [SerializeField] InputManager inputManager;
    private GameObject currentInstantiatedMesh;

    private void Start()
    {
        inputManager.inputActions.Main.RespawnObject.performed += RespawnObject;
        objImporter.OnModelGenerationFinished += OnModelGenerationFinished;
    }

    private void OnModelGenerationFinished(GameObject renderedMesh)
    {
        if (renderedMesh != null)
        {
            currentInstantiatedMesh = renderedMesh;
        }
    }

    private void RespawnObject(InputAction.CallbackContext context)
    {
        if (currentInstantiatedMesh == null) return;
        ImportedObjectController controller =
                currentInstantiatedMesh.GetComponent<ImportedObjectController>();
        if (!controller.IsSelected())
        {
            controller.RespawnObject();
        }

    }

}
