using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class TextRequestConfirmation : MonoBehaviour
{
    public event Action OnConfirmTextRequest;

    [SerializeField]
    private GameObject confirmText;

    [SerializeField]
    private GameObject cancelText;
    [SerializeField]
    private TextMeshPro mainText;
    [SerializeField]
    private InputManager inputManager;

    private void Start() {
         // TODO Refactor
        inputManager.Input_OnPressX += delegate {
            Debug.Log("Selected");
            OnConfirmTextRequest?.Invoke();
            Delegate[] list = OnConfirmTextRequest.GetInvocationList();
            foreach (Action d in list)
            {
                OnConfirmTextRequest -= d;
            }
            DeactivateConfirmation();
        };
        inputManager.Input_OnPressY += delegate {
             Debug.Log("Canceled");
            DeactivateConfirmation();
        };
    }

    public void ActivateConfirmation()
    {
        Debug.Log("Activating text confirm");
        confirmText.SetActive(true);
        cancelText.SetActive(true);
    }

    public void DeactivateConfirmation()
    {
        Debug.Log("Deactivating text confirm");
        confirmText.SetActive(false);
        cancelText.SetActive(false);
        mainText.text = "";
    }
}
