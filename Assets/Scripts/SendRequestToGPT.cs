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
    }

    
}
