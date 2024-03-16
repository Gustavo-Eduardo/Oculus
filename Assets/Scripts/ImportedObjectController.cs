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
    private Vector3 initialPosition;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _grabbable = GetComponent<Grabbable>();
        _grabbable.WhenPointerEventRaised += HandleEvent;
    }

    public void SetInitialPosition(Vector3 position)
    {
        initialPosition = position;
    }

    public void RespawnObject() {
        transform.position = initialPosition;
    }

    public bool IsSelected() {
        return isSelected;
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
