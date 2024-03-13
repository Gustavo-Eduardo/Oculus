using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Oculus.Interaction;
// using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityGLTF;

public class ObjImporter : MonoBehaviour
{
    // public TextMeshProUGUI inputField;
    // public Camera mainCamera;

    // [SerializeField] private GameInput gameInput;

    // public void Start()
    // {
    //     gameInput.OnPlayerGenerate += ObjImporter_Generate;
    // }

    public void ImportObject(Vector3 spawnPosition)
    {
        StartCoroutine(LoadObjectCoroutine("test", spawnPosition));
    }

    private IEnumerator LoadObjectCoroutine(string word, Vector3 spawnPosition)
    {
        // Step 2: Make API Request
        string apiUrl =
            $"https://projectmw.s3.us-east-2.amazonaws.com/godmode/models/tmp6jt9ud19.glb"; // Adjust this according to your API
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API request failed: " + www.error);
            yield break;
        }

        // Step 3: Handle Response
        string glbFilePath = Path.Combine(
            Application.persistentDataPath,
            "DownloadedObjects",
            $"${word}.glb"
        );
        Directory.CreateDirectory(Path.GetDirectoryName(glbFilePath));
        File.WriteAllBytes(glbFilePath, www.downloadHandler.data);

        // Step 4: Import the .glb File
        // Use the UnityGLTF loader to load the GLB file
        var glbObject = ImportGLB(glbFilePath);
        glbObject.transform.position = spawnPosition;
    }

    private GameObject ImportGLB(string glbFilePath)
    {
        // Instantiate the GLTFComponent on an empty GameObject
        GameObject gltfObject = new GameObject("GLTF Object");
        var gltfComponent = gltfObject.AddComponent<GLTFComponent>();
        // Set the GLTFComponent's uri to the path of the downloaded file
        gltfComponent.GLTFUri = glbFilePath;
        // Start loading the GLB file. The GLTFComponent will handle the instantiation.
        gltfComponent.Load();

        gltfObject.AddComponent<BoxCollider>();
        Rigidbody rigidbody = gltfObject.AddComponent<Rigidbody>();
        Grabbable grabbable = gltfObject.AddComponent<Grabbable>();
        
        GrabInteractable grabInteractable = gltfObject.AddComponent<GrabInteractable>();
        grabInteractable.InjectOptionalPointableElement(grabbable);
        grabInteractable.InjectRigidbody(rigidbody);

        ImportedObjectController objectController =
            gltfObject.AddComponent<ImportedObjectController>();

        // XRGrabInteractable xRGrab = obj.AddComponent<XRGrabInteractable>();
        // xRGrab.useDynamicAttach = true;
        gltfObject.transform.localScale = gltfObject.transform.localScale * 0.5f;
        return gltfObject;
    }
}
