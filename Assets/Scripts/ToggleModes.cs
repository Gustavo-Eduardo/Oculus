using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleModes : MonoBehaviour
{
    [SerializeField] GameObject zenModeImage;
    [SerializeField] GameObject destructionModeImage;
    [SerializeField] InputManager inputManager;
    [SerializeField] List<GameObject> zenGameObjects;
    [SerializeField] List<GameObject> destructionGameObjects;
    private bool isZenMode = true;

    public void SetZenMode(bool zenMode) {
        ToggleMode(zenMode);
    }

    private void ToggleMode(bool zenMode) {
        isZenMode = zenMode;
        foreach (GameObject go in zenGameObjects) {
            go.SetActive(isZenMode);
        }
        foreach (GameObject go in destructionGameObjects) {
            go.SetActive(!isZenMode);
        }
    }

    private void Update() {
        if (OVRInput.GetUp(OVRInput.Button.SecondaryThumbstick)) {
            ToggleMode(!isZenMode);
        }
        // destructionModeImage.SetActive(!isZenMode);
        zenModeImage.SetActive(isZenMode);
    }

}
