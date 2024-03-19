using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationManager : MonoBehaviour
{
    [SerializeField] private GameObject confirmText;
    [SerializeField] private GameObject cancelText;
    [SerializeField] private Transform confirmationSpawnPoint;
    [SerializeField] private InputManager inputManager;
    [SerializeField] public event Action OnConfirmation;

    private void Start()
    {
        transform.SetParent(confirmationSpawnPoint);
        inputManager.Input_OnPressX += OnConfirm;
        inputManager.Input_OnPressY += OnCancel;
    }

    public void ActivateText()
    {
        confirmText.SetActive(true);
        cancelText.SetActive(true);
    }

    public void DeactivateText()
    {
        confirmText.SetActive(false);
        cancelText.SetActive(false);
    }

    private void OnConfirm()
    {
        OnConfirmation?.Invoke();
    }
    private void OnCancel()
    {
        Debug.Log("Canceled");
    }

}
