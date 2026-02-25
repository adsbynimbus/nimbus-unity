using System;
using System.Collections;
using Example.Scripts.NotAdRelated;
using Nimbus.Internal;
using Nimbus.Runtime.Scripts;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for a rewarded ad by implementing the IAdEvents interface which the NimbusManager
	///     auto subscribes to
	/// </summary>
	public class RewardedVideoExample : MonoBehaviour, IAdEventsExtended {
		public GameObject cloud;

		// keep a reference of the returned ad so that it can be safely cleaned up
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
			_ad = NimbusManager.Instance.RequestRewardVideoAdAndLoad("unity_demo_rewarded_video_position");
			_alreadyTriggered = true;
		}

		public void OnAdLoaded(NimbusAdUnit nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad was returned and loaded into memory ad instance {nimbusAdUnit.InstanceID}, " +
				$"bid value: {nimbusAdUnit.BidResponse.BidRaw}, " +
				$"bid value in cents: {nimbusAdUnit.BidResponse.BidInCents}, " +
				$"network: {nimbusAdUnit.BidResponse.Network}, " +
				$"placement_id: {nimbusAdUnit.BidResponse.PlacementId}, " +
				$"auction_id: {nimbusAdUnit.BidResponse.AuctionId}");
		}

		public void OnAdWasRendered(NimbusAdUnit nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad was rendered for ad instance {nimbusAdUnit.InstanceID}, " +
				$"bid value: {nimbusAdUnit.BidResponse.BidRaw}, " +
				$"bid value in cents: {nimbusAdUnit.BidResponse.BidInCents}, " +
				$"network: {nimbusAdUnit.BidResponse.Network}, " +
				$"placement_id: {nimbusAdUnit.BidResponse.PlacementId}, " +
				$"auction_id: {nimbusAdUnit.BidResponse.AuctionId}");
		}

		public void OnAdImpression(NimbusAdUnit nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad impression was fired {nimbusAdUnit.InstanceID}, " +
				$"network: {nimbusAdUnit.BidResponse.Network}, " +
				$"placement_id: {nimbusAdUnit.BidResponse.PlacementId}, " +
				$"auction_id: {nimbusAdUnit.BidResponse.AuctionId}");
		}

		public void OnAdClicked(NimbusAdUnit nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad was clicked {nimbusAdUnit.InstanceID}, " +
				$"network: {nimbusAdUnit.BidResponse.Network}, " +
				$"placement_id: {nimbusAdUnit.BidResponse.PlacementId}, " +
				$"auction_id: {nimbusAdUnit.BidResponse.AuctionId}");
		}

		public void OnAdDestroyed(NimbusAdUnit nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad was destroyed/removed from the scene {nimbusAdUnit.InstanceID}, " +
				$"network: {nimbusAdUnit.BidResponse.Network}, " +
				$"placement_id: {nimbusAdUnit.BidResponse.PlacementId}, " +
				$"auction_id: {nimbusAdUnit.BidResponse.AuctionId}");
		}

		public void OnAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			if (skipped) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad was completed and the use can be rewarded {nimbusAdUnit.InstanceID}, " +
				$"network: {nimbusAdUnit.BidResponse.Network}, " +
				$"placement_id: {nimbusAdUnit.BidResponse.PlacementId}, " +
				$"auction_id: {nimbusAdUnit.BidResponse.AuctionId}");
			UnityThread.ExecuteInUpdate(RewardUser);
		}

		public void OnAdError(NimbusAdUnit nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log($"RewardedVideoExample Err {nimbusAdUnit.ErrResponse.Message}");
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