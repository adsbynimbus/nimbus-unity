using System;
using Nimbus;
using Nimbus.Internal;
using UnityEngine;
using System.Collections;


namespace Examples {
	public class RewardedVideoExample2: MonoBehaviour {
		public GameObject cloud;
		private NimbusAdUnit _ad;
		private bool _alreadyTriggered;

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.LoadAndShowRewardedVideoAd();
			_alreadyTriggered = true;
			
			while (true) {
				if (_ad.DidHaveAnError()) {
					Debug.unityLogger.Log($"No ad could be retrieved {_ad.ErrorMessage()}");
					break;
				}

				if (_ad.GetCurrentAdState() == AdEventTypes.COMPLETED ||
				    _ad.GetCurrentAdState() == AdEventTypes.DESTROYED) {
					RewardUser();
					break;
				}
			}
		}
		
		private void OnDestroy() {
			_ad?.Destroy();
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