using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GLTFast;
using Newtonsoft.Json;
using Oculus.Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityGLTF;

public class ObjImporter : MonoBehaviour
{
    public void ImportObject(string text, Vector3 spawnPosition)
    {
        StartCoroutine(LoadObjectCoroutine(text, spawnPosition));
    }

    private IEnumerator LoadObjectCoroutine(string word, Vector3 spawnPosition)
    {
        // Step 2: Make API Request
        // string apiUrl = $"https://projectmw.s3.us-east-2.amazonaws.com/godmode/models/tmppj9el8it.obj"; // Adjust this according to your API
        
        // Step 3: Handle Response
        string objUrl = "https://projectmw.s3.us-east-2.amazonaws.com/godmode/models/tmppj9el8it.obj";
        // string objUrl = "https://s3.filebin.net/filebin/9d3133bcdcf418a5dcd82e0400c14c92866a1003a9c0acec6e696e9f8f8ad038/e1310fee224e6431abdba7ce3c982bb32c2f0478364b1bfae77043cd2ad7bfb0?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=7pMj6hGeoKewqmMQILjm%2F20240310%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20240310T201624Z&X-Amz-Expires=300&X-Amz-SignedHeaders=host&response-cache-control=max-age%3D300&response-content-disposition=filename%3D%22avatar.obj%22&response-content-type=text%2Fplain%3B%20charset%3Dutf-8&X-Amz-Signature=dd869518d9eb964bbb0a72fa81f59dd06d0e413cdb39504c3c8944d7d0e3563d";
        // Step 4: Download the .obj File
        UnityWebRequest objRequest = UnityWebRequest.Get(objUrl);
        yield return objRequest.SendWebRequest();
        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download .obj file: " + objRequest.error);
            yield break;
        }
        Debug.Log($"Download Complete");
        // Save .obj file to a folder outside the game directory
        // Step 5: Import the .obj File
        Debug.Log($"Instantiating");
        // Step 6: Instantiate GameObject
        InstantiateGameObjectFromOBJ(objRequest.downloadHandler.text, spawnPosition);

        // ImportGLB(www.downloadHandler.data, apiUrl, spawnPosition);
    }

    private void InstantiateGameObjectFromOBJ(string response, Vector3 spawnPosition)
    {
        // Read the .obj file
        string[] lines = response.Split("\n");
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
        meshRenderer.material = new Material(Shader.Find("Standard"));
        obj.AddComponent<BoxCollider>();
        Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
        Grabbable grabbable = obj.AddComponent<Grabbable>();

        GrabInteractable grabInteractable = obj.AddComponent<GrabInteractable>();
        grabInteractable.InjectOptionalPointableElement(grabbable);
        grabInteractable.InjectRigidbody(rigidbody);

        ImportedObjectController objectController = obj.AddComponent<ImportedObjectController>();

        // XRGrabInteractable xRGrab = obj.AddComponent<XRGrabInteractable>();
        // xRGrab.useDynamicAttach = true;
        obj.transform.localScale = obj.transform.localScale * 0.5f;
        obj.transform.position = spawnPosition;
    }

    private async void ImportGLB(byte[] data, string url, Vector3 spawnPosition)
    {
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(url)
        );
        if (success)
        {
            GameObject gltfObject = new GameObject("GLTF Object");
            await gltf.InstantiateMainSceneAsync(gltfObject.transform);
            // var gltfComponent = gltfObject.AddComponent<GLTFast.GltfAsset>();

            // Set the GLTFComponent's uri to the path of the downloaded file
            // gltfComponent.Url = glbFilePath;

            // Start loading the GLB file. The GLTFComponent will handle the instantiation.
            // gltfComponent.Load();

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
            gltfObject.transform.position = spawnPosition;
        }
        // Instantiate the GLTFComponent on an empty GameObject
    }

    [System.Serializable]
    public class ConversionData
    {
        public string identifier;
        public string output_file;
        public string source_file;
        public string started_at;
        public string finished_at;
        public string duration;
    }
}

// Helper class for handling JSON array parsing in Unity
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T[]>(json);
    }
}
