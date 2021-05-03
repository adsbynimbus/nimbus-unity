using System.Collections;
using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	public class FullScreenExample : MonoBehaviour {
		public GameObject cloud;
		private NimbusAdUnit _ad;
		private bool _alreadyTriggered;

		private void OnDestroy() {
			_ad?.Destroy();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.LoadAndShowFullScreenAd("unity_demo_rewarded_fullscreen_position", 0.0f, 0.0f);
			_alreadyTriggered = true;

			var loaded = false;
			while (true) {
				if (_ad.DidHaveAnError()) {
					Debug.unityLogger.Log($"No ad could be retrieved {_ad.ErrorMessage()}");
					break;
				}

				// this is an example that uses the internal state of the ad and requires 0 event notifications
				if (_ad.GetCurrentAdState() == AdEventTypes.LOADED ||
				    _ad.GetCurrentAdState() == AdEventTypes.IMPRESSION && !loaded) {
					Debug.unityLogger.Log(
						$"NimbusEventListenerExample Ad was rendered for ad instance {_ad.InstanceID}, bid value: {_ad.GetBidValue()}, network: {_ad.GetNetwork()}, auction_id: {_ad.GetAuctionID()}");
					loaded = true;
				}

				if (_ad.GetCurrentAdState() != AdEventTypes.COMPLETED &&
				    _ad.GetCurrentAdState() != AdEventTypes.DESTROYED) continue;

				RewardUser();
				break;
			}
		}

		private IEnumerator MakeItRain() {
			cloud.SetActive(true);
			yield return new WaitForSeconds(4);
			ScoreUI.Instance.UpdateScore(100);
		}

		private void RewardUser() {
			StartCoroutine(MakeItRain());
		}
	}
}