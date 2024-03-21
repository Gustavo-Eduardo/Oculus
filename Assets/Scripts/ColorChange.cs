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
    private void Start()
    {
        inputManager.inputActions.ChatRecording.Record.started += ActivateColor;
        inputManager.inputActions.GenerationRecording.Record.started += ActivateColor;
        inputManager.inputActions.ChatRecording.Record.canceled += DeactivateColor;
        inputManager.inputActions.GenerationRecording.Record.canceled += DeactivateColor;
    }

    private void ActivateColor(InputAction.CallbackContext context)
    {
        Debug.Log("Activation color");
        meshRenderer.material.color = Color.red;
        if (pointLight != null)
        {
            pointLight.color = Color.red;
        }
    }

    private void DeactivateColor(InputAction.CallbackContext context)
    {
        Debug.Log("Deactivation color");
        meshRenderer.material.color = Color.white;
        if (pointLight != null)
        {
            pointLight.color = Color.white;
        }
    }
}
