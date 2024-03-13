using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ImportedObjectController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Grabbable _grabbable;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _grabbable = GetComponent<Grabbable>();
        _grabbable.WhenPointerEventRaised += HandleEvent;
    }

    private void HandleEvent(PointerEvent ev) {
        if (ev.Type == PointerEventType.Select) {
            OnSelectedImportedObject();
        } else if (ev.Type == PointerEventType.Unselect) {
            OnUnselectedImportedObject();
        }
    }

    public void OnSelectedImportedObject()
    {
        _rigidbody.useGravity = false;
    }

    public void OnUnselectedImportedObject()
    {
        _rigidbody.useGravity = true;
    }
}
