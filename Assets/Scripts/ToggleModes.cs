using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleModes : MonoBehaviour
{
    [SerializeField] GameObject zenModeImage;
    [SerializeField] GameObject destructionModeImage;
    [SerializeField] InputManager inputManager;
    private bool isZenMode = true;

    private void Start() {
        inputManager.Input_OnPressX += ToggleMode;
    }

    public void SetZenMode(bool zenMode) {
        isZenMode = zenMode;        
    }

    private void ToggleMode() {
        isZenMode = !isZenMode;
    }

    private void Update() {
        destructionModeImage.SetActive(!isZenMode);
        zenModeImage.SetActive(isZenMode);
    }

}
