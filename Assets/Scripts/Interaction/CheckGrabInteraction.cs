using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class CheckGrabInteraction : MonoBehaviour
{
    [SerializeField] GrabInteractor grabInteractor;
    [SerializeField] DistanceGrabInteractor distanceGrabInteractor;
    public bool IsGrabbing()
    {
        return grabInteractor.HasSelectedInteractable || distanceGrabInteractor.HasSelectedInteractable;
    }
}
