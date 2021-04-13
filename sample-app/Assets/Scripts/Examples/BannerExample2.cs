using Nimbus;
using UnityEngine;
using System.Collections;

namespace Examples {
	public class BannerExample2: MonoBehaviour {
		private bool _alreadyTriggered;
		private IEnumerator _bannerRoutine;

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_bannerRoutine = NimbusManager.Instance.LoadAndShowBannerAdWithRefresh("unity_demo_banner_position2");
			StartCoroutine(_bannerRoutine);
			_alreadyTriggered = true;
		}
		
		public void StopBannerRefresh() {
			StopCoroutine(_bannerRoutine);
		}
	}
}