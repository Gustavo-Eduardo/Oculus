using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsTest : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartRecordingManual);
        stopButton.onClick.AddListener(StopRecordingManual);
    }

    private void StartRecordingManual()
    {
        VoiceRecognitionManager.Instance.TriggerStartRecording();
    }
    private void StopRecordingManual()
    {
        VoiceRecognitionManager.Instance.TriggerStopRecording();
    }
}