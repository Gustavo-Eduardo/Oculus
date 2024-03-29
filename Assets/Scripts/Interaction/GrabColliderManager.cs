using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabColliderManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log($"Colliding with: {collider.gameObject.name}");
    }

}
