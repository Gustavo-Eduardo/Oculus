using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event Action Input_OnReleaseButton;

    private float rJoystickYInput;
    private bool pressingTrigger;
    private bool pressingButton;

    private void Awake() { }

    private void Update()
    {
        // HandleTriggerButton();
        // Vector2 inputMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * Vector2.up;
        // rJoystickYInput = inputMovement.y;
        HandleAButton();
    }
    private void HandleAButton()
    {
        bool isPressed = OVRInput.GetDown(OVRInput.Button.One);
        bool isReleased = OVRInput.GetUp(OVRInput.Button.One);
        if (isPressed) {
            Debug.Log("Start recording");
            VoiceRecognitionManager.Instance.TriggerStartRecording();
        }
        if (isReleased) {
            Debug.Log("Stop recording");
            VoiceRecognitionManager.Instance.TriggerStopRecording();
        }
    }

    private void HandleTriggerButton()
    {
        float primaryIndexTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        if (primaryIndexTriggerValue > 0)
        {
            if (!pressingTrigger)
            {
                pressingTrigger = true;
                OnHoldButton();
            }
        }
        else
        {
            if (pressingTrigger)
            {
                pressingTrigger = false;
                OnReleaseButton();
            }
        }
    }

    private void OnHoldButton()
    {
        // VoiceRecognitionManager.Instance.TriggerStartRecording();
    }

    private void OnReleaseButton()
    {
        // VoiceRecognitionManager.Instance.TriggerStopRecording();
        Input_OnReleaseButton?.Invoke();
    }

    public float GetRightJoystickYAxisInput()
    {
        return rJoystickYInput;
    }
}
