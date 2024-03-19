using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class ObjectGenerationConfirmation : MonoBehaviour
{
    public event Action OnConfirmGeneration;

    [SerializeField]
    private GameObject confirmText;

    [SerializeField]
    private GameObject cancelText;
    [SerializeField]
    private TextMeshPro mainText;
    [SerializeField]
    private InputManager inputManager;

    private void Awake()
    {
        // PointableElement confirmPointable = confirmText.GetComponent<PointableElement>();
        // PointableElement cancelPointable = cancelText.GetComponent<PointableElement>();
        // confirmPointable.WhenPointerEventRaised += OnConfirm;
        // cancelPointable.WhenPointerEventRaised += OnCancel;
    }
    private void Start() {
         // TODO Refactor
        inputManager.Input_OnPressX += delegate {
            Debug.Log("Selected");
            OnConfirmGeneration?.Invoke();
            if (OnConfirmGeneration == null) return;
            Delegate[] list = OnConfirmGeneration.GetInvocationList();
            foreach (Action d in list)
            {
                OnConfirmGeneration -= d;
            }
            DeactivateConfirmation();
        };
        inputManager.Input_OnPressY += delegate {
             Debug.Log("Canceled");
            DeactivateConfirmation();
        };
    }

    private void OnConfirm(PointerEvent ev)
    {
        if (ev.Type == PointerEventType.Select)
        {
            Debug.Log("Selected");
            OnConfirmGeneration?.Invoke();
            Delegate[] list = OnConfirmGeneration.GetInvocationList();
            foreach (Action d in list)
            {
                OnConfirmGeneration -= d;
            }
            DeactivateConfirmation();
        }
    }

    private void OnCancel(PointerEvent ev)
    {
        if (ev.Type == PointerEventType.Select)
        {
            Debug.Log("Canceled");
            DeactivateConfirmation();
        }
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
