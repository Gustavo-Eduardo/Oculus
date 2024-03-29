using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Light pointLight;

    [SerializeField] private VoiceRecognitionManager chatRecording;
    private void Start()
    {
        // inputManager.inputActions.ChatRecording.Record.started += ActivateColor;
        // inputManager.inputActions.GenerationRecording.Record.started += ActivateColor;
        // inputManager.inputActions.ChatRecording.Record.canceled += DeactivateColor;
        // inputManager.inputActions.GenerationRecording.Record.canceled += DeactivateColor;
        chatRecording.OnStartRecording += ActivateColor;
        chatRecording.OnStopRecording += DeactivateColor;
    }

    private void ActivateColor(object sender, EventArgs args)
    {
        Debug.Log("Activation color");
        meshRenderer.material.color = Color.red;
        if (pointLight != null)
        {
            pointLight.color = Color.red;
        }
    }
    // private void ActivateColor(InputAction.CallbackContext context)
    // {
    //     Debug.Log("Activation color");
    //     meshRenderer.material.color = Color.red;
    //     if (pointLight != null)
    //     {
    //         pointLight.color = Color.red;
    //     }
    // }

    private void DeactivateColor(object sender, EventArgs args)
    {
        Debug.Log("Deactivation color");
        meshRenderer.material.color = Color.white;
        if (pointLight != null)
        {
            pointLight.color = Color.white;
        }
    }
    // private void DeactivateColor(InputAction.CallbackContext context)
    // {
    //     Debug.Log("Deactivation color");
    //     meshRenderer.material.color = Color.white;
    //     if (pointLight != null)
    //     {
    //         pointLight.color = Color.white;
    //     }
    // }
}
