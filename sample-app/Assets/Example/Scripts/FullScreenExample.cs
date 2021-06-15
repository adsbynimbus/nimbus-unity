using System.Collections;
using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for a full screen ad and also how to manually subscribe to Nimbus Ad events
	/// </summary>
	public class FullScreenExample : MonoBehaviour {
		public GameObject cloud;
		private NimbusAdUnit _ad;
		private bool _alreadyTriggered;


		private void Start() {
			NimbusManager.Instance.NimbusEvents.OnVideoAdCompleted += RewardUser;
			NimbusManager.Instance.NimbusEvents.OnAdError += LogError;
		}
		
		// called as for extra safety. Ensures all resources are released
		private void OnDestroy() {
			NimbusManager.Instance.NimbusEvents.OnVideoAdCompleted -= RewardUser;
			NimbusManager.Instance.NimbusEvents.OnAdError -= LogError;
			_ad?.Destroy();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.LoadAndShowFullScreenAd("unity_demo_rewarded_fullscreen_position", 0.0f, 0.0f);
			_alreadyTriggered = true;
		}

		private IEnumerator MakeItRain() {
			cloud.SetActive(true);
			yield return new WaitForSeconds(4);
			ScoreUI.Instance.UpdateScore(100);
		}

		private void RewardUser(NimbusAdUnit ad, bool skipped) {
			if (!skipped) {
				Debug.unityLogger.Log(
					$"NimbusEventListenerExample Ad was rendered for ad instance {ad.InstanceID}, " +
					$"bid value: {ad.ResponseMetaData.BidRaw}, " +
					$"bid value in cents: {ad.ResponseMetaData.BidInCents}, " +
					$"network: {ad.ResponseMetaData.Network}, " +
					$"placement_id: {ad.ResponseMetaData.PlacementID}, " +
					$"auction_id: {ad.ResponseMetaData.AuctionID}");
				StartCoroutine(MakeItRain());
			}
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad was rendered for ad instance, however the user skipped the ad {ad.InstanceID}, " +
				$"bid value: {ad.ResponseMetaData.BidRaw}, " +
				$"bid value in cents: {ad.ResponseMetaData.BidInCents}, " +
				$"network: {ad.ResponseMetaData.Network}, " +
				$"placement_id: {ad.ResponseMetaData.PlacementID}, " +
				$"auction_id: {ad.ResponseMetaData.AuctionID}");
		}

		private void LogError(NimbusAdUnit ad) {
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad failed to load {ad.InstanceID}, " +
				$"Error Message Ad failed to load {ad.ErrorMessage()}, " +
				$"auction_id: {ad.ResponseMetaData.AuctionID}");
		}
	}
}