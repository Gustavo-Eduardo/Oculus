using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class ChatWithGPT : MonoBehaviour
{
    public event Action<string> OnTextChange;
    public event Action<string> OnChatRequestDone;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private VoiceRecognitionManager sendTextVoiceRecognition;

    [SerializeField]
    private ObjImporter objImporter;

    [SerializeField]
    private Transform spawnPoint;
    [SerializeField] private CheckGrabInteraction grabInteractorCheck;

    private string threadId;

    private void Start()
    {
        InputAction RecordAction = inputManager.inputActions.ChatRecording.Record;
        RecordAction.performed += StartRecording;
        RecordAction.canceled += StopRecording;
        sendTextVoiceRecognition.OnRequestDone += OnRequestDone;
    }

    private void StartRecording(InputAction.CallbackContext context)
    {
        Debug.Log(grabInteractorCheck.IsGrabbing());
        if (grabInteractorCheck.IsGrabbing()) return;
        sendTextVoiceRecognition.TriggerStartRecording();
    }

    private void StopRecording(InputAction.CallbackContext context)
    {
        sendTextVoiceRecognition.TriggerStopRecording();
    }

    private void OnRequestDone(string text)
    {
        OnTextChange?.Invoke(text);
        StartCoroutine(HandleRequest(text));
    }

    private IEnumerator HandleRequest(string text)
    {
        if (threadId == null)
        {
            // TODO: Change to MW Server
            const string START_CONVERSATION_URL =
                "https://memorywarserver.herokuapp.com/godmode/start-conversation";
            UnityWebRequest threadRequest = UnityWebRequest.Get(START_CONVERSATION_URL);
            yield return threadRequest.SendWebRequest();

            if (threadRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API request failed: " + threadRequest.error);
                yield break;
            }

            StartConversationResponse parsedResponse =
                JsonConvert.DeserializeObject<StartConversationResponse>(
                    threadRequest.downloadHandler.text
                );
            threadId = parsedResponse.threadId;
        }

        // TODO: Change to MW Server
        const string SEND_MESSAGE_TO_CONVERSATION_URL =
            "https://memorywarserver.herokuapp.com/godmode/conversation-message";
        Dictionary<string, string> formData = new();
        formData["message"] = text;
        formData["threadId"] = threadId;
        UnityWebRequest request = UnityWebRequest.Post(SEND_MESSAGE_TO_CONVERSATION_URL, formData);
        Debug.Log($"Sending request: {text}");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API request failed: " + request.error);
            yield break;
        }

        Debug.Log("requestText: " + request.downloadHandler.text);

        SendMessageToConversationResponse conversationResponse =
            JsonConvert.DeserializeObject<SendMessageToConversationResponse>(
                request.downloadHandler.text
            );

        if (conversationResponse.functionCalls.Length > 0)
        {
            foreach (FunctionCallObject functionCall in conversationResponse.functionCalls)
            {
                Debug.Log($"Function call: {functionCall.name}");
                objImporter.ImportObject(functionCall.args["objectString"], spawnPoint.position);
            }
        }
        else
        {
            string responseMessage = conversationResponse.message;

            OnTextChange?.Invoke(responseMessage);
            OnChatRequestDone?.Invoke(responseMessage);
        }

    }

    private class StartConversationResponse
    {
        public string threadId;
    }

    [Serializable]
    private class SendMessageToConversationResponse
    {
        public string message;
        public FunctionCallObject[] functionCalls;
    }

    [Serializable]
    private class FunctionCallObject
    {
        public string name;
        public Dictionary<string, string> args;
    }
}
