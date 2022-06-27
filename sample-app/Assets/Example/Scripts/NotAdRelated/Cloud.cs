using System;
using Unity.Mathematics;
using UnityEngine;

namespace Example.Scripts.NotAdRelated {
	public class Cloud : MonoBehaviour {
		[SerializeField] private float _floatDistance = 4;
		[SerializeField] private float _growthSpeed = 1;
		[SerializeField] private ParticleSystem _rain;
		[SerializeField] private GameObject _rainSpawnLocation;
		
		private const float Tolerance = 0.05f;
		private const float MaxScale = 1;
		
		private bool _completeTransition;
		private float _newYPosition;
		private Transform _transform;

		private void Start() {
			_transform = transform;
			_newYPosition = _transform.position.y + _floatDistance;
		}


		private void Update() {
			var delta = Time.deltaTime;
			if (!_completeTransition) {
				Grow(delta);
				FloatUp(delta);
			} else {
				try {
					Instantiate(_rain, _rainSpawnLocation.transform.position, Quaternion.identity);
				}
				catch (NullReferenceException e) {
					Console.WriteLine(e);
				}
				enabled = false;
			}

			if (!(Math.Abs(_transform.localScale.sqrMagnitude - 3.0f) <= Tolerance)) return;
			if (Math.Abs(_transform.position.y - _newYPosition) <= Tolerance) _completeTransition = true;
		}

		private void Grow(float deltaTime) {
			var localScale = _transform.localScale;
			var scale = new Vector3(localScale.x, localScale.y, localScale.z);
			scale.x = math.lerp(scale.x, MaxScale, _growthSpeed * deltaTime);
			scale.y = math.lerp(scale.y, MaxScale, _growthSpeed * deltaTime);
			scale.z = math.lerp(scale.z, MaxScale, _growthSpeed * deltaTime);
			_transform.localScale = scale;
		}

		private void FloatUp(float deltaTime) {
			var position = _transform.position;
			position.y = math.lerp(position.y, _newYPosition, deltaTime);
			_transform.position = position;
		}
	}
}