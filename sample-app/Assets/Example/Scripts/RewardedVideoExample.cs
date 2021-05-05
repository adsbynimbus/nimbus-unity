using System.Collections;
using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	public class RewardedVideoExample : MonoBehaviour, IAdEvents {
		public GameObject cloud;
		private NimbusAdUnit _ad;
		private bool _alreadyTriggered;

		private void Awake() {
			UnityThread.InitUnityThread();
		}

		private void OnDestroy() {
			_ad?.Destroy();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.LoadAndShowRewardedVideoAd("unity_demo_rewarded_video_position", 0.0f);
			_alreadyTriggered = true;
		}
		
		public void OnSDKInitialize(string gdprConsentString) {
			// TODO
		}

		public void OnAdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad was rendered for ad instance {nimbusAdUnit.InstanceID}, bid value: {nimbusAdUnit.GetBidValue()}, network: {nimbusAdUnit.GetNetwork()}, auction_id: {nimbusAdUnit.GetAuctionID()}");
		}

		public void OnAdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log($"NimbusEventListenerExample Err {nimbusAdUnit.ErrorMessage()}");
		}

		public void OnAdLoaded(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnAdImpression(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnAdClicked(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnAdDestroyed(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnVideoAdPaused(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnVideoAdResume(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnVideoAdCompleted(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log("Rewarding the player for watching the whole video!");
			UnityThread.ExecuteInUpdate(RewardUser);
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