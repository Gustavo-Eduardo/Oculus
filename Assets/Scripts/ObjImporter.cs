using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Oculus.Interaction;
// using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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

    // public void ObjImporter_Generate(object sender, EventArgs e)
    // {
    //     string word = inputField.text;

    //     if (inputField.text != null && inputField.text != "")
    //     {
    //         Debug.Log($"Attempting generation of: {inputField.text}");
    //         // StartCoroutine(LoadObjectCoroutine(word));
    //     }
    //     else
    //     {
    //         Debug.LogError($"Word cannot be null or empty!");
    //     }
    // }

    private IEnumerator LoadObjectCoroutine(string word, Vector3 spawnPosition)
    {
        // Step 2: Make API Request
        // string apiUrl = $"https://filebin.net/nn43qsqw7t5xi8ih/{word}.obj"; // Adjust this according to your API
        // UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        // yield return www.SendWebRequest();

        // if (www.result != UnityWebRequest.Result.Success)
        // {
        //     Debug.LogError("API request failed: " + www.error);
        //     yield break;
        // }

        // // Step 3: Handle Response
        // string objUrl = www.downloadHandler.text.Trim();
        string objUrl =
            "https://projectmw.s3.us-east-2.amazonaws.com/godmode/models/tmpy4vjpn9y.obj";

        // Step 4: Download the .obj File
        UnityWebRequest objRequest = UnityWebRequest.Get(objUrl);
        Debug.Log("Initialize download");
        yield return objRequest.SendWebRequest();

        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download .obj file: " + objRequest.error);
            yield break;
        }
        Debug.Log($"Download Complete");

        // Save .obj file to a folder outside the game directory
        string filePath = Path.Combine(
            Application.persistentDataPath,
            "DownloadedObjects",
            $"{word}.obj"
        );
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllBytes(filePath, objRequest.downloadHandler.data);

        // Step 5: Import the .obj File

        Debug.Log($"Instantiating");
        // Step 6: Instantiate GameObject
        GameObject obj = InstantiateGameObjectFromOBJ(filePath);
        if (obj != null)
        {
            Debug.Log("Object created");
            // Position the object 5 units in front of the camera
            // Vector3 spawnPosition =
            //     mainCamera.transform.position + mainCamera.transform.forward * 5f;
            obj.transform.position = spawnPosition;
        }
    }

    // You'll need to implement a method to instantiate the GameObject from the .obj file
    private GameObject InstantiateGameObjectFromOBJ(string filePath)
    {
        // Read the .obj file        
        string[] lines = File.ReadAllLines(filePath);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (string line in lines)
        {
            string[] parts = line.Trim().Split(' ');

            if (parts[0] == "v") // Vertex
            {
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                float z = float.Parse(parts[3]);
                vertices.Add(new Vector3(x, y, z));
            }
            else if (parts[0] == "f") // Face
            {
                for (int i = 1; i < parts.Length; i++)
                {
                    string[] indices = parts[i].Split('/');
                    int vertexIndex = int.Parse(indices[0]) - 1; // OBJ indices start from 1
                    triangles.Add(vertexIndex);
                }
            }
        }

        // Create GameObject
        GameObject obj = new GameObject("OBJObject");

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
        // MeshCollider meshCollider =  obj.AddComponent<MeshCollider>();
        // meshCollider.sharedMesh = mesh;
        // meshCollider.convex = true;

        Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
        Grabbable grabbable = obj.AddComponent<Grabbable>();
        GrabInteractable grabInteractable = obj.AddComponent<GrabInteractable>();
        grabInteractable.InjectOptionalPointableElement(grabbable);
        grabInteractable.InjectRigidbody(rigidbody);

        // XRGrabInteractable xRGrab = obj.AddComponent<XRGrabInteractable>();
        // xRGrab.useDynamicAttach = true;
        meshRenderer.material = new Material(Shader.Find("Standard"));
        obj.transform.localScale = obj.transform.localScale * 0.5f;
        // meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        return obj;
    }
}
