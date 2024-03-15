using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Start()
    {
        inputManager.Input_OnPressA += ActivateColor;
        inputManager.Input_OnReleaseA += DeactivateColor;
    }

    private void ActivateColor()
    {
        Debug.Log("Activation color");
        meshRenderer.material.color = Color.red;
    }

    private void DeactivateColor()
    {
        Debug.Log("Deactivation color");
        meshRenderer.material.color = Color.white;
    }
}
