using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandGunController : MonoBehaviour
{
    private const string HANDGUN_NAME = "ISDK_HandGrabInteraction";

    [SerializeField] private GrabInteractor lGrabInteractor;
    [SerializeField] private GrabInteractor rGrabInteractor;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private InputManager inputManager;


    private void Start()
    {
        inputManager.inputActions.Main.FireGunLeft.performed += FireLeft;
        inputManager.inputActions.Main.FireGunRight.performed += FireRight;
    }

    private void FireLeft(InputAction.CallbackContext context)
    {
        Debug.Log("Attempting shoot left");
        if (isGrabbingHandgunLeft())
        {
            Debug.Log("shooting left");
            audioSource.Play();
        }
    }
    private void FireRight(InputAction.CallbackContext context)
    {
        Debug.Log("Attempting shoot right");
        if (isGrabbingHandgunRight())
        {
            Debug.Log("shooting right");
            audioSource.Play();
        }
    }

    private bool isGrabbingHandgunLeft()
    {
        if (!lGrabInteractor.HasSelectedInteractable) return false;

        if (!lGrabInteractor.SelectedInteractable.gameObject.name.Equals(HANDGUN_NAME)) return false;

        return true;
    }

    private bool isGrabbingHandgunRight()
    {
        if (!rGrabInteractor.HasSelectedInteractable) return false;

        if (!rGrabInteractor.SelectedInteractable.gameObject.name.Equals(HANDGUN_NAME)) return false;

        return true;
    }
}
