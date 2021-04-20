using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Example.Scripts {
    public class TreeSway : MonoBehaviour {
        public float speed = 5f;
        public float maxRotation = 10f;
        
        private SpriteSkin _skin;
        private List<Transform> _leafNodes = new List<Transform>();
        [ExecuteInEditMode]
        private void Start() {
            _skin = GetComponent<SpriteSkin>();
            foreach (var b in _skin.boneTransforms) {
                if (b.childCount == 0) {
                    _leafNodes.Add(b);
                }
            }
         
        }

        
        private void Update() {
            var delta = Time.deltaTime;
            foreach (var n in _leafNodes) {
                Rotate(n, delta);
            }
        }

        private void Rotate(Transform t, float delta) {
            var z = Mathf.Lerp(t.localRotation.z, maxRotation * Mathf.Sin(Time.time), speed * delta);
            t.rotation = Quaternion.Euler(0f, 0f, z);
        }
    }
}
