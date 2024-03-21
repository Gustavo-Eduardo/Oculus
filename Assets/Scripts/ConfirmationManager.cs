using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConfirmationManager : MonoBehaviour
{
    [SerializeField] private GameObject confirmText;
    [SerializeField] private GameObject cancelText;
    [SerializeField] private Transform confirmationSpawnPoint;
    [SerializeField] private InputManager inputManager;
    [SerializeField] public event Action OnConfirmation;

    private void Start()
    {
        inputManager.SetActionMapAvailability(InputManager.ActionMap.Confirmation, true);
        transform.SetParent(confirmationSpawnPoint, false);
        inputManager.inputActions.Confirmation.Confirm.performed += OnConfirm;
        inputManager.inputActions.Confirmation.Cancel.performed += OnCancel;
        DeactivateText();
    }

    public void ActivateText()
    {
        confirmText.SetActive(true);
        cancelText.SetActive(true);
        inputManager.SetActionMapAvailability(InputManager.ActionMap.Confirmation, true);
    }

    public void DeactivateText()
    {
        confirmText.SetActive(false);
        cancelText.SetActive(false);
        inputManager.SetActionMapAvailability(InputManager.ActionMap.Confirmation, false);
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        OnConfirmation?.Invoke();
        DeactivateText();
    }
    private void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("Canceled");
        DeactivateText();
    }

}
