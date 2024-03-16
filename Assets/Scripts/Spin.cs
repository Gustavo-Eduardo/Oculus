using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField]
    Vector3 rotateAmount;

    void Update() { 
        transform.Rotate(rotateAmount * Time.deltaTime);
    }
}
