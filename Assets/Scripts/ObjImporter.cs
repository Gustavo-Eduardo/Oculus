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
using UnityGLTF;

public class ObjImporter : MonoBehaviour
{
    [SerializeField]
    private GameObject plane;

    [SerializeField]
    private GameObject selectorPrefab;

    [SerializeField]
    private TextMeshPro statusText;
    public event Action<Sprite> OnSourceImageGenerated;
    public event Action OnModelGenerationFinished;
    private GameObject currentInstantiatedMesh;
    private GameObject currentSpawnCircle;

    private void Awake()
    {
        OnSourceImageGenerated += (Sprite sprite) =>
        {
            plane.GetComponent<SpriteRenderer>().sprite = sprite;
            BoxCollider collider = plane.GetComponent<BoxCollider>();
            collider.size = new Vector3(sprite.texture.width, sprite.texture.height, 1);
        };
    }

    private void updateCurrentInstantiatedMesh(GameObject instantiateMesh)
    {
        currentInstantiatedMesh = instantiateMesh;
    }

    public void TriggerImageGeneratedEvent(Sprite sprite) {
        OnSourceImageGenerated?.Invoke(sprite);
     }

    private void Update()
    {
        if (currentInstantiatedMesh != null)
        {
            bool pressedB = OVRInput.GetUp(OVRInput.Button.Two);
            ImportedObjectController controller =
                currentInstantiatedMesh.GetComponent<ImportedObjectController>();
            if (!controller.IsSelected() && pressedB)
            {
                controller.RespawnObject();
            }
        }
    }

    public void ImportObject(string text, Vector3 spawnPosition)
    {
        if (currentSpawnCircle != null)
        {
            Destroy(currentSpawnCircle);
        }
        GameObject selector = Instantiate(selectorPrefab);
        currentSpawnCircle = selector;
        selector.transform.position = spawnPosition - Vector3.up + Vector3.up * 0.01f;
        OnModelGenerationFinished += delegate
        {
            if (selector != null)
            {
                Destroy(selector);
            }
        };
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
            yield break;
        }

        statusText.text = "First request failed";
        ServerResponse response = JsonConvert.DeserializeObject(firstRequest.downloadHandler.text) as ServerResponse;
        // Step 2: Request will return an id in string type
        string objectId = response.id;
        string sound = response.sound;

        Debug.Log($"Received ID, attempting polling: {objectId}");

        // Step 3: Polling for object conversion
        string pollingUrl = "http://52.53.194.213:8000/api/conversions/";
        bool conversionCompleted = false;
        string sourceFileUrl = "";
        string outputFileUrl = "";

        statusText.text = "Generating image";

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
                statusText.text = "Generation failed";
                Debug.LogError("Step 3 API request failed: " + pollingRequest.error);
                OnModelGenerationFinished?.Invoke();
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
                        statusText.text = "Generating mesh";
                    }
                }
                if (data.identifier == objectId && !string.IsNullOrEmpty(data.output_file))
                {
                    Debug.Log($"converstion completed");
                    statusText.text = "Generation completed";
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
            OnModelGenerationFinished?.Invoke();
            yield break;
        }

        // string glbFilePath = Path.Combine(Application.persistentDataPath, "DownloadedObjects", $"{word}.glb");
        // Directory.CreateDirectory(Path.GetDirectoryName(glbFilePath));
        // File.WriteAllBytes(glbFilePath, www.downloadHandler.data);

        ImportGLB(www.downloadHandler.data, apiUrl, spawnPosition);
    }

    private IEnumerator LoadObjectCoroutineBU(string word, Vector3 spawnPosition)
    {
        // Step 1: Make API Request to http://localhost:3001/godmode/object-image?word=banana
        // string word = "banana";
        string firstApiUrl =
            $"https://projectmw.s3.us-east-2.amazonaws.com/godmode/models/tmp6jt9ud19.glb";
        UnityWebRequest firstRequest = UnityWebRequest.Get(firstApiUrl);
        Debug.Log("Sending request");
        yield return firstRequest.SendWebRequest();

        if (firstRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Step 1 API request failed: " + firstRequest.error);
            yield break;
        }

        Debug.Log("Request done");
        // Step 2: Request will return an id in string type
        ImportGLB(firstRequest.downloadHandler.data, firstApiUrl, spawnPosition);
    }

    public async void ImportGLB(byte[] data, string url, Vector3 spawnPosition)
    {
        Debug.Log("Importing object");
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(url)
        );
        if (success)
        {
            Debug.Log("Object loaded");

            GameObject gltfObject = new GameObject("GLTF Object");
            Debug.Log("Adding components");
            // var gltfComponent = gltfObject.AddComponent<GLTFast.GltfAsset>();

            // Set the GLTFComponent's uri to the path of the downloaded file
            // gltfComponent.Url = glbFilePath;

            // Start loading the GLB file. The GLTFComponent will handle the instantiation.
            // gltfComponent.Load();

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

            // XRGrabInteractable xRGrab = obj.AddComponent<XRGrabInteractable>();
            // xRGrab.useDynamicAttach = true;
            gltfObject.transform.localScale = gltfObject.transform.localScale * 0.5f;
            gltfObject.transform.position = spawnPosition;

            Debug.Log("Instantiating Model");
            await gltf.InstantiateMainSceneAsync(gltfObject.transform);
            OnModelGenerationFinished?.Invoke();
            currentInstantiatedMesh = gltfObject;
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
    
    [System.Serializable]
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
