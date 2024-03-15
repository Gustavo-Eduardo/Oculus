using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        VoiceRecognitionManager.Instance.OnStartRecording += ActivateColor;
        VoiceRecognitionManager.Instance.OnStopRecording += DeactivateColor;
    }

    private void ActivateColor(object sender, EventArgs ev)
    {
        meshRenderer.material.SetColor("testColor", Color.red);
    }

    private void DeactivateColor(object sender, EventArgs ev)
    {
        meshRenderer.material.SetColor("testColor", Color.white);
    }
}
