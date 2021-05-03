using System.Collections;
using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	public class BannerExample2 : MonoBehaviour {
		private bool _alreadyTriggered;
		private IEnumerator _bannerRoutine;

		private NimbusAdUnit _currentAdUnit;

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_bannerRoutine =
				NimbusManager.Instance.LoadAndShowBannerAdWithRefresh("unity_demo_banner_position2", 0.0f,
					SetAdUnitFromCoroutine);
			StartCoroutine(_bannerRoutine);
			_alreadyTriggered = true;
		}

		// SetAdUnitFromCoroutine this allows you to track the current ad unit from within the coroutine as it refreshes
		// the main purpose of this is to clean up the ad unit if you decide to stop the coroutine
		private void SetAdUnitFromCoroutine(NimbusAdUnit currentAdUnit) {
			_currentAdUnit = currentAdUnit;
		}


		public void StopBannerRefresh() {
			StopCoroutine(_bannerRoutine);
			// clean up if the _currentAdUnit was send when the coroutine was stopped
			_currentAdUnit?.Destroy();
		}
	}
}