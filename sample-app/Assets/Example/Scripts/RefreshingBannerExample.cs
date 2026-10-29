using System.Threading;
using Example.Scripts.NotAdRelated;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for a refreshing banner using coroutines
	/// </summary>
	public class RefreshingBannerExample : MonoBehaviour {
		private bool _alreadyTriggered;
		private InlineAd _adUnit;

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_adUnit = Nimbus.bannerAd("unity_demo_banner_position2",  screenPosition: AdScreenPosition.TOP_CENTER);
			_alreadyTriggered = true;
		}

		public void StopBannerRefresh() {
			_adUnit.destroy();
		}
	}
}