using UnityEngine;

namespace Example.Scripts {
	public class Flower : MonoBehaviour {
		private static readonly int Wiggle = Animator.StringToHash("Wiggle");
		private Animator _anim;

		private void Start() {
			_anim = GetComponent<Animator>();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null) return;
			_anim.SetTrigger(Wiggle);
		}
	}
}