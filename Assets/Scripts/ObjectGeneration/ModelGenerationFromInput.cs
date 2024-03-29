using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelGenerationFromInput : MonoBehaviour
{

    public event Action<string> OnTextChange;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private VoiceRecognitionManager generationVoiceRecognition;
    [SerializeField] private ConfirmationManager confirmationManager;
    [SerializeField] private RayInteractor rayInteractor;
    [SerializeField] private ObjImporter objImporter;

    private void Start()
    {
        InputAction RecordAction = inputManager.inputActions.GenerationRecording.Record;
        RecordAction.performed += StartRecording;
        RecordAction.canceled += StopRecording;
        generationVoiceRecognition.OnRequestDone += OnRequestDone;
    }

    private void OnRequestDone(string text) {
        // this.word.text = word.Replace(".", "");
        string cleanWord = CleanWord(text);
        OnTextChange?.Invoke(text);
        if (rayInteractor.CollisionInfo.HasValue)
        {
            SurfaceHit collision = rayInteractor.CollisionInfo.Value;
            Vector3 hitPosition = collision.Point + Vector3.up;
            // TODO: Handle case when there is no collision
            confirmationManager.ActivateText();
            Action ConfirmImportObjectAction = delegate
            {
                objImporter.ImportObject(cleanWord, hitPosition);
            };
            confirmationManager.OnConfirmation += ConfirmImportObjectAction;

        }
    }

    private void StartRecording(InputAction.CallbackContext context)
    {        
        generationVoiceRecognition.TriggerStartRecording();
    }
    private void StopRecording(InputAction.CallbackContext context)
    {
        generationVoiceRecognition.TriggerStopRecording();
    }

     private string CleanWord(string word)
    {
        string cleanWord = word.Trim().ToLower();
        cleanWord = cleanWord.Replace(".", "");
        cleanWord = cleanWord.Replace(" ", "_");
        return cleanWord;
    }

}
