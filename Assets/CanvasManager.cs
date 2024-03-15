using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField]
    TextMeshPro word;

    [SerializeField]
    private ObjImporter objectImporter;

    [SerializeField]
    private ObjectGenerationConfirmation generationConfirmation;

    [SerializeField]
    private RayInteractor rayInteractor;

    private void Start()
    {
        VoiceRecognitionManager.Instance.OnRequestDone += CanvasManager_OnRequestDone;
    }

    private void CanvasManager_OnRequestDone(string word)
    {
        Debug.Log("Request Done");
        this.word.text = word.Replace(".", "");
        string cleanWord = CleanWord(word);
        if (rayInteractor.CollisionInfo.HasValue)
        {
            Debug.Log("Collision");
            SurfaceHit collision = rayInteractor.CollisionInfo.Value;
            Vector3 hitPosition = collision.Point + Vector3.up;

            // TODO: Handle case when there is no collision
            generationConfirmation.ActivateConfirmation();

            Action ConfirmImportObjectAction = delegate
            {
                ConfirmImportObject(cleanWord, hitPosition);
            };

            generationConfirmation.OnConfirmGeneration += ConfirmImportObjectAction;
        }
    }

    private void ConfirmImportObject(string cleanWord, Vector3 hitPosition)
    {
        objectImporter.ImportObject(cleanWord, hitPosition);        
    }

    private string CleanWord(string word)
    {
        string cleanWord = word.Trim().ToLower();
        cleanWord = cleanWord.Replace(".", "");
        cleanWord = cleanWord.Replace(" ", "_");
        return cleanWord;
    }
}
