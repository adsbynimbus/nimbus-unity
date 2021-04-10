using System;
using Unity.Mathematics;
using UnityEngine;

namespace Examples {
    public class Cloud : MonoBehaviour {
        public float floatDistance;
        public float growthSpeed = 1;
        public ParticleSystem rain;
        public GameObject rainSpawnLocation;

        private float _newYPosition;
        private bool _completeTransition;
        private const float MAXScale = 1;
        private const float Tolerance = 0.05f;
        private void Start() {
            _newYPosition = transform.position.y + floatDistance;
        }


        private void Update() {
            var delta = Time.deltaTime;
            if (!_completeTransition) {
                Grow(delta);
                FloatUp(delta);
            }
            else {
                Instantiate(rain, rainSpawnLocation.transform.position, Quaternion.identity);
                GetComponent<Cloud>().enabled = false;
            }
            
            var transform1 = transform;
            var localScale = transform1.localScale;
            var position = transform1.position;
            if (!(Math.Abs(localScale.sqrMagnitude - 3.0f) <= Tolerance)) return;
            if (Math.Abs(position.y - _newYPosition) <= Tolerance) {
                _completeTransition = true;
            }
        }

        private void Grow(float deltaTime) {
            var localScale = transform.localScale;
            var scale = new Vector3(localScale.x, localScale.y, localScale.z);
            scale.x = math.lerp(scale.x, MAXScale, growthSpeed * deltaTime);
            scale.y = math.lerp(scale.y, MAXScale, growthSpeed * deltaTime);
            scale.z = math.lerp(scale.z, MAXScale, growthSpeed * deltaTime);
            transform.localScale = scale;
        }

        private void FloatUp(float deltaTime) {
            var position = transform.position;
            position.y = math.lerp(position.y, _newYPosition, deltaTime);
            transform.position = position;
        }


    }
}
