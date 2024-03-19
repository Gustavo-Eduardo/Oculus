using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GenerationStatusText : MonoBehaviour
{
    [SerializeField] private ObjImporter objImporter;
    private TextMeshPro statusText;

    private void Awake()
    {
        statusText = GetComponent<TextMeshPro>();
    }

    private Dictionary<ObjImporter.Status, string> statusToText = new() {
        {ObjImporter.Status.ErrorFirstRequest, "First request failed"},
        {ObjImporter.Status.ErrorGeneration, "Generation failed"},
        {ObjImporter.Status.GeneratingImage, "Generating image"},
        {ObjImporter.Status.GeneratingMesh, "Generating mesh"},
        {ObjImporter.Status.GenerationComplete, "Generation completed"},
    };

    private void Start()
    {
        objImporter.OnModelGenerationStatusChange += OnModelGenerationStatusChange;
    }

    private void OnModelGenerationStatusChange(ObjImporter.Status status)
    {
        statusText.text = statusToText.GetValueOrDefault(status, "Unknown status");
    }
}
