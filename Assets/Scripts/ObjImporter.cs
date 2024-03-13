using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using TMPro;
using System;
using UnityGLTF;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oculus.Interaction;

public class ObjImporter : MonoBehaviour
{

    public void ImportObject(string text, Vector3 spawnPosition)
    {
        StartCoroutine(LoadObjectCoroutine(text, spawnPosition));
    }

    private IEnumerator LoadObjectCoroutine(string word, Vector3 spawnPosition)
    {
        // Step 1: Make API Request to http://localhost:3001/godmode/object-image?word=banana
        // string word = "banana";
        string firstApiUrl = $"https://memorywarserver.herokuapp.com/godmode/object-image?word={word}";
        UnityWebRequest firstRequest = UnityWebRequest.Get(firstApiUrl);
        yield return firstRequest.SendWebRequest();

        if (firstRequest.result != UnityWebRequest.Result.Success)
        {

            Debug.LogError("Step 1 API request failed: " + firstRequest.error);
            yield break;
        }

        // Step 2: Request will return an id in string type
        string objectId = firstRequest.downloadHandler.text.Trim();

        Debug.Log($"Received ID, attempting polling: {objectId}");


        // Step 3: Polling for object conversion
        string pollingUrl = "http://52.53.194.213:8000/api/conversions/";
        bool conversionCompleted = false;
        string outputFileUrl = "";

        while (!conversionCompleted)
        {
            Debug.Log($"Polling");


            UnityWebRequest pollingRequest = UnityWebRequest.Get(pollingUrl);

            string bearerToken = "8606e97a3516fb6f5ecefdf1db91bc286333ce14";
            pollingRequest.SetRequestHeader("Authorization", "Token " + bearerToken);

            yield return pollingRequest.SendWebRequest();

            if (pollingRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Step 3 API request failed: " + pollingRequest.error);
                yield break;
            }

            // Parse JSON response
            string jsonText = pollingRequest.downloadHandler.text;
            ConversionData[] conversionDataArray = JsonHelper.FromJson<ConversionData>(jsonText);

            // Step 4: Check if identified object has "output_file" field which is a URL
            foreach (var data in conversionDataArray)
            {
                if (data.identifier == objectId && !string.IsNullOrEmpty(data.output_file))
                {
                    conversionCompleted = true;
                    outputFileUrl = data.output_file;
                    break;
                }
            }

            if (!conversionCompleted)
                yield return new WaitForSeconds(1); // Polling interval
        }


        // Step 5: Replace apiUrl Below
        string apiUrl = outputFileUrl;
        Debug.Log($"Polling complete, attempting download model");

        // string apiUrl = $"https://projectmw.s3.us-east-2.amazonaws.com/godmode/models/tmp6jt9ud19.glb"; // Adjust this according to your API
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API request failed: " + www.error);
            yield break;
        }

        string glbFilePath = Path.Combine(Application.persistentDataPath, "DownloadedObjects", $"{word}.glb");
        Directory.CreateDirectory(Path.GetDirectoryName(glbFilePath));
        File.WriteAllBytes(glbFilePath, www.downloadHandler.data);

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
        // textObject.text = "Object generated";
        return gltfObject;
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