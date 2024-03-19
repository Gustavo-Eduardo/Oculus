using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnLastMesh : MonoBehaviour
{
    [SerializeField] ObjImporter objImporter;
    [SerializeField] InputManager inputManager;
    private GameObject currentInstantiatedMesh;

    private void Start()
    {
        objImporter.OnModelGenerationFinished += OnModelGenerationFinished;
        inputManager.Input_OnPressB += OnPressedB;
    }

    private void OnModelGenerationFinished(GameObject renderedMesh)
    {
        if (renderedMesh != null)
        {
            currentInstantiatedMesh = renderedMesh;
        }
    }

    private void OnPressedB()
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
