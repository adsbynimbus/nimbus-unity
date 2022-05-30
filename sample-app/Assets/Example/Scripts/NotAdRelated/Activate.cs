using System.Collections.Generic;
using UnityEngine;

namespace Example.Scripts.NotAdRelated {
	public class Activate : MonoBehaviour {
		[SerializeField]private List<GameObject> _toActivate;

		private bool _alreadyTriggered;

		private void OnTriggerEnter2D(Collider2D other) {
			if (_alreadyTriggered) return;
			if (!TryGetComponent(out NimbusPlayerController player)) return;
			
			_alreadyTriggered = true;
			foreach (var a in _toActivate) a.SetActive(true);
		}
	}
}