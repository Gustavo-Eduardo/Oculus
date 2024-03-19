using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatDisplayText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textObject;
    [SerializeField] private SendTextToGPT sendTextToGPT;
    private void Start() {
        sendTextToGPT.OnTextChange += ChangeTextDisplay;
    }
    private void ChangeTextDisplay(string newText) {
        textObject.text = newText;
    }
}
