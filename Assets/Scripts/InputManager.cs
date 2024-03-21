using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public InputActions inputActions;
    // TODO: Find if it's possible to make this dynamic
    public enum ActionMap
    {
        Main,
        Confirmation,
        GenerationRecording,
        ChatRecording,
        HoldingObject
    }

    private Dictionary<ActionMap, string> enumToString = new() {
        {ActionMap.Main, "Main"},
        {ActionMap.Confirmation, "Confirmation"},
        {ActionMap.GenerationRecording, "GenerationRecording"},
        {ActionMap.ChatRecording, "ChatRecording"},
        {ActionMap.HoldingObject, "HoldingObject"},
    };

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Main.Enable();
        inputActions.GenerationRecording.Enable();
        inputActions.ChatRecording.Enable();
    }

    public void SetActionMapAvailability(ActionMap actionMap, bool available)
    {
        string actionMapName = enumToString.GetValueOrDefault(actionMap, "Main");
        if (available)
        {            
            inputActions.asset.FindActionMap(actionMapName).Enable();
        }
        else
        {
            inputActions.asset.FindActionMap(actionMapName).Disable();
        }
    }
    public void SetActionMapAvailability(string actionMapName, bool available)
    {
        if (available)
        {            
            inputActions.asset.FindActionMap(actionMapName).Enable();
        }
        else
        {
            inputActions.asset.FindActionMap(actionMapName).Disable();
        }
    }
}
