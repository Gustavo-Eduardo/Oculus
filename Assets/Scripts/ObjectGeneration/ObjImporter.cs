using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DreamXR.Interaction;
using GLTFast;
using Newtonsoft.Json;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ObjImporter : MonoBehaviour
{
    public enum Status
    {
        ErrorFirstRequest,
        ErrorGeneration,
        GeneratingImage,
        GeneratingMesh,
        GenerationComplete
    }

    public event Action<string, Vector3> OnModelGenerationStarted;
    public event Action<Status> OnModelGenerationStatusChange;
    public event Action OnModelRequestSuccess;
    public event Action<GameObject> OnModelGenerationFinished;
    public event Action<Sprite> OnSourceImageGenerated;

    public void TriggerImageGeneratedEvent(Sprite sprite)
    {
        OnSourceImageGenerated?.Invoke(sprite);
    }

    public void ImportObject(string text, Vector3 spawnPosition)
    {
        OnModelGenerationStarted?.Invoke(text, spawnPosition);
        StartCoroutine(LoadObjectCoroutine(text, spawnPosition));
    }

    private IEnumerator LoadObjectCoroutine(string word, Vector3 spawnPosition)
    {
        // Step 1: Make API Request to http://localhost:3001/godmode/object-image?word=banana
        // string word = "banana";
        string firstApiUrl =
            $"https://memorywarserver.herokuapp.com/godmode/object-image?word={word}";
        UnityWebRequest firstRequest = UnityWebRequest.Get(firstApiUrl);
        yield return firstRequest.SendWebRequest();

        if (firstRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Step 1 API request failed: " + firstRequest.error);
            OnModelGenerationStatusChange?.Invoke(Status.ErrorFirstRequest);
            yield break;
        }

        Debug.Log($"Response {firstRequest.downloadHandler.text}");
        ServerResponse response = JsonConvert.DeserializeObject<ServerResponse>(firstRequest.downloadHandler.text);
        Debug.Log($"Formatted response {response}");
        // Step 2: Request will return an id in string type
        string objectId = response.id;
        string sound = response.sound;
        Debug.Log($"Object {objectId}");
        Debug.Log($"Sound {sound}");

        Debug.Log($"Received ID, attempting polling: {objectId}");

        // Step 3: Polling for object conversion
        string pollingUrl = "http://52.53.194.213:8000/api/conversions/";
        bool conversionCompleted = false;
        string sourceFileUrl = "";
        string outputFileUrl = "";

        OnModelGenerationStatusChange?.Invoke(Status.GeneratingImage);

        while (!conversionCompleted)
        {
            Debug.Log($"Polling");

            UnityWebRequest pollingRequest = UnityWebRequest.Get(pollingUrl);

            string bearerToken = "8606e97a3516fb6f5ecefdf1db91bc286333ce14";
            pollingRequest.SetRequestHeader("Authorization", "Token " + bearerToken);

            yield return pollingRequest.SendWebRequest();

            Debug.Log($"result: {pollingRequest.result}");
            if (pollingRequest.result != UnityWebRequest.Result.Success)
            {
                OnModelGenerationStatusChange?.Invoke(Status.ErrorGeneration);
                Debug.LogError("Step 3 API request failed: " + pollingRequest.error);
                OnModelGenerationFinished?.Invoke(null);
                yield break;
            }

            // Parse JSON response
            Debug.Log($"text: {pollingRequest.downloadHandler.text}");
            string jsonText = pollingRequest.downloadHandler.text;
            ConversionData[] conversionDataArray = JsonHelper.FromJson<ConversionData>(jsonText);

            // Step 4: Check if identified object has "output_file" field which is a URL
            foreach (var data in conversionDataArray)
            {
                if (
                    data.identifier == objectId
                    && !string.IsNullOrEmpty(data.source_file)
                    && string.IsNullOrEmpty(sourceFileUrl)
                )
                {
                    sourceFileUrl = data.source_file;
                    Debug.Log($"source file: {data.source_file}");
                    Debug.Log($"url {sourceFileUrl}");
                    UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(sourceFileUrl);
                    yield return imageRequest.SendWebRequest();
                    if (imageRequest.result == UnityWebRequest.Result.Success)
                    {
                        DownloadHandlerTexture downloadHandlerTexture =
                            imageRequest.downloadHandler as DownloadHandlerTexture;
                        Texture2D texture = downloadHandlerTexture.texture;
                        Sprite sprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            Vector2.one * 0.5f
                        );
                        OnSourceImageGenerated?.Invoke(sprite);
                        OnModelGenerationStatusChange?.Invoke(Status.GeneratingMesh);
                    }
                }
                if (data.identifier == objectId && !string.IsNullOrEmpty(data.output_file))
                {
                    Debug.Log($"converstion completed");
                    OnModelGenerationStatusChange?.Invoke(Status.GenerationComplete);
                    conversionCompleted = true;
                    outputFileUrl = data.output_file;
                    Debug.Log($"output file: {data.output_file}");
                    Debug.Log($"url {outputFileUrl}");
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
            OnModelGenerationFinished?.Invoke(null);
            yield break;
        }

        OnModelRequestSuccess?.Invoke();
        // string glbFilePath = Path.Combine(Application.persistentDataPath, "DownloadedObjects", $"{word}.glb");
        // Directory.CreateDirectory(Path.GetDirectoryName(glbFilePath));
        // File.WriteAllBytes(glbFilePath, www.downloadHandler.data);

        ImportGLB(www.downloadHandler.data, apiUrl, spawnPosition);
    }

    public async void ImportGLB(byte[] data, string url, Vector3 spawnPosition)
    {
        Debug.Log("Importing object");
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            new Uri(url)
        );
        if (success)
        {
            Debug.Log("Object loaded");

            GameObject gltfObject = new GameObject("GLTF Object");
            Debug.Log("Adding components");

            gltfObject.AddComponent<BoxCollider>();
            Rigidbody rigidbody = gltfObject.AddComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Grabbable grabbable = gltfObject.AddComponent<Grabbable>();

            PhysicsGrabbable physicsGrabbable = gltfObject.AddComponent<PhysicsGrabbable>();
            physicsGrabbable.InjectPointable(grabbable);
            physicsGrabbable.InjectRigidbody(rigidbody);

            GrabInteractable grabInteractable = gltfObject.AddComponent<GrabInteractable>();
            grabInteractable.InjectOptionalPointableElement(grabbable);
            grabInteractable.InjectRigidbody(rigidbody);

            DistanceGrabInteractable distanceGrabInteractable =
                gltfObject.AddComponent<DistanceGrabInteractable>();
            distanceGrabInteractable.InjectOptionalPointableElement(grabbable);
            distanceGrabInteractable.InjectRigidbody(rigidbody);

            HandGrabInteractable handGrabInteractable =
                gltfObject.AddComponent<HandGrabInteractable>();
            handGrabInteractable.InjectOptionalPointableElement(grabbable);
            handGrabInteractable.InjectRigidbody(rigidbody);

            gltfObject.AddComponent<HitObject>();

            ImportedObjectController objectController =
                gltfObject.AddComponent<ImportedObjectController>();
            objectController.SetInitialPosition(spawnPosition);

            gltfObject.transform.localScale = gltfObject.transform.localScale * 0.5f;
            gltfObject.transform.position = spawnPosition;

            Debug.Log("Instantiating Model");
            await gltf.InstantiateMainSceneAsync(gltfObject.transform);
            OnModelGenerationFinished?.Invoke(gltfObject);
        }
        // Instantiate the GLTFComponent on an empty GameObject
    }

    [Serializable]
    public class ConversionData
    {
        public string identifier;
        public string output_file;
        public string source_file;
        public string started_at;
        public string finished_at;
        public string duration;
    }

    [Serializable]
    public class ServerResponse
    {
        public string id;
        public string sound;
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
