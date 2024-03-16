using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Meta.WitAi;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ImportedObjectController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Grabbable _grabbable;
    private bool isSelected = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _grabbable = GetComponent<Grabbable>();
        _grabbable.WhenPointerEventRaised += HandleEvent;
    }

    private void Update()
    {
        bool pressedB = OVRInput.GetUp(OVRInput.Button.Two);
        if (isSelected && pressedB)
        {
            Destroy(gameObject);
        }
    }

    private void HandleEvent(PointerEvent ev)
    {
        if (ev.Type == PointerEventType.Select)
        {
            OnSelectedImportedObject();
        }
        else if (ev.Type == PointerEventType.Unselect)
        {
            OnUnselectedImportedObject();
        }
    }

    public void OnSelectedImportedObject()
    {
        _rigidbody.useGravity = false;
        isSelected = true;
    }

    public void OnUnselectedImportedObject()
    {
        _rigidbody.useGravity = true;
        isSelected = false;
    }
}
