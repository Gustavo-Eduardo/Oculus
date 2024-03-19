using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Networking;

public class SendTextToGPT : MonoBehaviour
{
    public event Action<string> OnTextChange;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private VoiceRecognitionManager sendTextVoiceRecognition;
    [SerializeField] private ConfirmationManager confirmationManager;

    private void Start()
    {
        inputManager.Input_OnPressLeftGrab += OnHoldGrab;
        inputManager.Input_OnReleaseLeftGrab += OnReleaseGrab;
        sendTextVoiceRecognition.OnRequestDone += OnRequestDone;
    }

    private void OnHoldGrab()
    {
        sendTextVoiceRecognition.TriggerStartRecording();
    }
    private void OnReleaseGrab()
    {
        sendTextVoiceRecognition.TriggerStopRecording();
    }

    private void OnRequestDone(string text)
    {
        Action ConfirmSendText = delegate
        {
            StartCoroutine(HandleRequest(text));
        };
        confirmationManager.OnConfirmation += ConfirmSendText;
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

        OnTextChange?.Invoke(request.downloadHandler.text);
    }

}
