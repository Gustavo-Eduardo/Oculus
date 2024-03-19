using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SendRequestToGPT : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private TextMeshPro textMesh;

    [SerializeField]
    private TextRequestConfirmation textRequestConfirmation;
    [SerializeField] private VoiceRecognitionManager holdRecognitionManager;


    private void Start()
    {
        inputManager.Input_OnPressLeftGrab += StartRecording;
        inputManager.Input_OnReleaseLeftGrab += StopRecording;
        holdRecognitionManager.OnRequestDone += RegisterRequestCallback;
    }

    private void StartRecording()
    {
        Debug.Log("Is recording");
        Debug.Log("Is recording");
        holdRecognitionManager.TriggerStartRecording();
    }

    private void StopRecording()
    {
        Debug.Log("Is stop recording");
        holdRecognitionManager.TriggerStopRecording();
    }

    private void RegisterRequestCallback(string text)
    {
        textMesh.text = text;
        Action SendRequest = delegate
        {
            StartCoroutine(HandleRequest(text));
        };

        textRequestConfirmation.OnConfirmTextRequest += SendRequest;
    }

    private IEnumerator HandleRequest(string text)
    {
        string apiUrl = $"localhost:3001/godmode/simulate-conversation";
        Dictionary<string, string> formData = new();
        formData["message"] = text;
        UnityWebRequest request = UnityWebRequest.Post(apiUrl, formData);
        Debug.Log("Sending request");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API request failed: " + request.error);
            yield break;
        }

        textMesh.text = request.downloadHandler.text;
    }
}
