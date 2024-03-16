using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamXR.Interaction {
    public class HitObject : MonoBehaviour {
        private const float MAX_SECONDS_TO_AUTO_DESTROY = 5f;

        Coroutine autoDestroyCoroutine;
        void OnCollisionEnter(Collision other) {
            if (other.gameObject.name == "Baseball_Bat") {
                if (autoDestroyCoroutine == null) {
                    autoDestroyCoroutine = StartCoroutine(AutoDestroyCoroutine());
                }
            } else {
                if (autoDestroyCoroutine != null) {
                    // If we've hitted something else after bat hit.
                    Destroy(gameObject);
                }
            }
        }

        private IEnumerator AutoDestroyCoroutine() {
            yield return new WaitForSeconds(MAX_SECONDS_TO_AUTO_DESTROY);
            Destroy(gameObject);
        }

        private void OnDestroy() {
            ParticleManager.Play("ExplosionFireballFire", transform.position, transform.rotation);
        }
    }
}
