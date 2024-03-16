using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace DreamXR.Interaction {
    public class BaseballBatController : MonoBehaviour {
        [SerializeField] private GameObject visual;
        [SerializeField] private GameObject colliderGO;

        void Start() {
            /**
            Rigidbody rigidbody = colliderGO.GetComponent<Rigidbody>();
            Grabbable grabbable = gameObject.AddComponent<Grabbable>();

            GrabInteractable grabInteractable = gameObject.AddComponent<GrabInteractable>();
            grabInteractable.InjectOptionalPointableElement(grabbable);
            grabInteractable.InjectRigidbody(rigidbody);

            HandGrabInteractable handGrabInteractable = gameObject.AddComponent<HandGrabInteractable>();
            handGrabInteractable.InjectOptionalPointableElement(grabbable);
            handGrabInteractable.InjectRigidbody(rigidbody);
            */
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
