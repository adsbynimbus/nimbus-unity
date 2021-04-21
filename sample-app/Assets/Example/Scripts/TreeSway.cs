using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Example.Scripts {
    public class TreeSway : MonoBehaviour {
        public float speed = 5f;
        public float maxRotation = 10f;
        public float trunkMaxRotation = 1f;
        public List<string> bonesToExclude;

        private Transform _trunk;
        private float _trunkZ;
        private SpriteSkin _skin;
        private readonly List<Transform> _leafNodes = new List<Transform>();
        
        private void Start() {
            _skin = GetComponent<SpriteSkin>();
            foreach (var b in _skin.boneTransforms) {
                if (b.name == "bone_2") {
                    _trunk = b;
                    _trunkZ = _trunk.rotation.z;
                }
                if (b.childCount == 0 && !bonesToExclude.Contains(b.name)) {
                    _leafNodes.Add(b);
                }
            }
        }
        
        private void Update() {
            var delta = Time.deltaTime;
            foreach (var n in _leafNodes) {
                LeafFlutter(n, delta);
            }
            TrunkFlutter(_trunk, delta);
        }

        private void LeafFlutter(Transform t, float delta) {
            var z = Mathf.Lerp(t.localRotation.z, maxRotation * Mathf.Sin(Time.time), speed * delta);
            t.localRotation = Quaternion.Euler(0f, 0f, z);
        }
        
        private void TrunkFlutter(Transform t, float delta) {
            var z = Mathf.Lerp(t.localRotation.z,  trunkMaxRotation * Mathf.Sin(Time.time), speed * delta);
            t.localRotation = Quaternion.Euler(0f, 0f, z);
        }
    }
}
