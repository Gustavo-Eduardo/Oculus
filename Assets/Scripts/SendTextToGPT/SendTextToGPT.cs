using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class SendTextToGPT : MonoBehaviour
{
    public event Action<string> OnTextChange;
    public event Action<string> OnChatRequestDone;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private VoiceRecognitionManager sendTextVoiceRecognition;
    [SerializeField] private ConfirmationManager confirmationManager;

    private void Start()
    {
        InputAction RecordAction = inputManager.inputActions.ChatRecording.Record;
        RecordAction.started += StartRecording;
        RecordAction.performed += StopRecording;
        sendTextVoiceRecognition.OnRequestDone += OnRequestDone;
    }

    private void StartRecording(InputAction.CallbackContext context)
    {
        sendTextVoiceRecognition.TriggerStartRecording();
    }
    private void StopRecording(InputAction.CallbackContext context)
    {
        sendTextVoiceRecognition.TriggerStopRecording();
    }

    private void OnRequestDone(string text)
    {
        confirmationManager.ActivateText();
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
        OnChatRequestDone?.Invoke(request.downloadHandler.text);
    }

}
