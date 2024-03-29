using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandGunController : MonoBehaviour
{
    private const string HANDGUN_NAME = "HandGun";

    [SerializeField] private GrabInteractor lHandGrabInteractor;
    [SerializeField] private GrabInteractor rHandGrabInteractor;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private InputManager inputManager;


    private void Start()
    {
        inputManager.inputActions.Main.FireGunLeft.performed += FireLeft;
        inputManager.inputActions.Main.FireGunRight.performed += FireRight;
    }

    private void FireLeft(InputAction.CallbackContext context)
    {
        if (isGrabbingHandgunLeft())
        {
            audioSource.Play();
        }
    }
    private void FireRight(InputAction.CallbackContext context)
    {
        if (isGrabbingHandgunRight())
        {
            audioSource.Play();
        }
    }

    private bool isGrabbingHandgunLeft()
    {
        if (!lHandGrabInteractor.HasSelectedInteractable) return false;

        if (!lHandGrabInteractor.SelectedInteractable.gameObject.name.Equals(HANDGUN_NAME)) return false;

        return true;
    }

    private bool isGrabbingHandgunRight()
    {
        if (!rHandGrabInteractor.HasSelectedInteractable) return false;

        if (!rHandGrabInteractor.SelectedInteractable.gameObject.name.Equals(HANDGUN_NAME)) return false;

        return true;
    }
}
