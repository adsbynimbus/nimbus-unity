using System.Collections;
using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	public class RefreshingBannerExample : MonoBehaviour {
		private bool _alreadyTriggered;
		private IEnumerator _bannerRoutine;
		
		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_bannerRoutine =
				NimbusManager.Instance.LoadAndShowBannerAdWithRefresh("unity_demo_banner_position2", 0.0f);
			StartCoroutine(_bannerRoutine);
			_alreadyTriggered = true;
		}
		
		public void StopBannerRefresh() {
			// cleanup make sure that the ad is destroyed and removed after stopping the coroutine
			NimbusManager.Instance.StopRefreshBannerAd(_bannerRoutine);
		}
	}
}