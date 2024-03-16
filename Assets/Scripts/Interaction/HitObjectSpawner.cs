using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamXR.Interaction {
public class HitObjectSpawner : MonoBehaviour {
        [SerializeField] private Transform spawnLocation;
        // The prefab of the object we want to hit with the bat.
        [SerializeField] private GameObject hitObjectPrefab;
        // Start is called before the first frame update
        void Start() {
            Instantiate(hitObjectPrefab, spawnLocation.position, spawnLocation.rotation);
        }

        // Update is called once per frame
        void Update() {
            if (OVRInput.GetDown(OVRInput.Button.One)) {
                Instantiate(hitObjectPrefab, spawnLocation.position, spawnLocation.rotation);
            }
        }
    }
}
