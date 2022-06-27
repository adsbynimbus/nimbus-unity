using UnityEngine;

namespace Example.Scripts.NotAdRelated {
	public class Flower : MonoBehaviour {
		private static readonly int Wiggle = Animator.StringToHash("Wiggle");
		private Animator _anim;

		private void Start() {
			_anim = GetComponent<Animator>();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			if (!TryGetComponent(out NimbusPlayerController player)) return;
			_anim.SetTrigger(Wiggle);
		}
	}
}