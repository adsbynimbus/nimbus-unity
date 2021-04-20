using System.Collections.Generic;
using UnityEngine;

namespace Example.Scripts {
	public class Activate:MonoBehaviour {
		public List<GameObject> toActivate;
		
		private bool _alreadyTriggered;
		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_alreadyTriggered = true;

			foreach (var a in toActivate) {
				a.SetActive(true);
			}
		}
	}
}