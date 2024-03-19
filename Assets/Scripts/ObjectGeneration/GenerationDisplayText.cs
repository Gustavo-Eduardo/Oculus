using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerationDisplayText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textObject;
    [SerializeField] private ModelGenerationFromInput modelGeneration;

    private void Start()
    {
        modelGeneration.OnTextChange += ChangeTextDisplay;
    }

    private void ChangeTextDisplay(string newText)
    {
        textObject.text = newText;
    }
}
