using UnityEngine;
using UnityEngine.Events;

namespace Example.Scripts {
	public class BannerDestroyEvent : MonoBehaviour {
		public UnityEvent onDestroy;

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null) return;
			onDestroy?.Invoke();
		}
	}
}