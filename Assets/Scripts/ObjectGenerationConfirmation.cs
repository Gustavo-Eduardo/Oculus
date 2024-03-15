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

    private void Awake()
    {
        PointableElement confirmPointable = confirmText.GetComponent<PointableElement>();
        PointableElement cancelPointable = cancelText.GetComponent<PointableElement>();
        confirmPointable.WhenPointerEventRaised += OnConfirm;
        cancelPointable.WhenPointerEventRaised += OnCancel;
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
        Debug.Log("Activating text cancel");
        confirmText.SetActive(false);
        cancelText.SetActive(false);
        mainText.text = "";
    }
}
