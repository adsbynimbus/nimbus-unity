using System.Threading;
using Example.Scripts.NotAdRelated;
using Internal.AdObjects;
using Nimbus.Runtime.Scripts;
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
			_adUnit = NimbusManager.Instance.BannerAd("unity_demo_banner_position2");
			_alreadyTriggered = true;
		}

		public void StopBannerRefresh() {
			_adUnit.Destroy();
		}
	}
}