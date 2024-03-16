using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreamXR.Interaction {
    // Responsible for managing your scene's global particle system requirements.
    // Useful for when you have a GameObject that's parent to Particle System,
    // which needs to be played when parent GameObject is destroyed.
    public class ParticleManager : MonoBehaviour {
        [SerializeField]
        private List<NameToParticleSystem> namesAndParticleSystems;

        // TODO(elhacker): Maybe encapsulate this dictionary in an internal class to avoid external modifications.
        private Dictionary<string, ParticleSystem> namesToParticleSystems;

        // Singleton
        public static ParticleManager Instance { get; private set;}

        private void Awake() {
            // If there's already an instance of this class destroy myself.
            if (Instance != null && Instance != this) {
                Destroy(this);
            } else {
                Instance = this;
            }

            namesToParticleSystems = new Dictionary<string, ParticleSystem>();
            foreach (NameToParticleSystem entry in namesAndParticleSystems) {
                namesToParticleSystems.Add(entry.Name, entry.Particle);
            }
        }

        // Public methods

        // TODO(elhacker): Add a method that allows looping and specifying some duration in seconds.
        public static GameObject Play(string particleName, Vector3 position, Quaternion rotation) {
            if (Instance.namesToParticleSystems.ContainsKey(particleName)) {
                return Play(Instance.namesToParticleSystems[particleName], position, rotation);
            }
            return null;
        }

        public static GameObject Play(ParticleSystem particleSystem, Vector3 position, Quaternion rotation) {
            GameObject temporalParticleGO = Instantiate(particleSystem.gameObject, position, rotation);
            ParticleSystem ps = temporalParticleGO.GetComponent<ParticleSystem>();
            ps.Play();
            Destroy(temporalParticleGO, ps.main.duration);
            return temporalParticleGO;
        }

        public static GameObject PlayAndFollow(string particleName, Transform target) {
            GameObject particleSystemGO = Play(particleName, target.position, target.rotation);
            if (particleSystemGO != null) {
                FollowTarget follow = particleSystemGO.AddComponent<FollowTarget>();
                follow.target = target;
            }
            return particleSystemGO;
        }
    }

    [Serializable]
    public struct NameToParticleSystem {
        public string Name;
        public ParticleSystem Particle;
    }

}
