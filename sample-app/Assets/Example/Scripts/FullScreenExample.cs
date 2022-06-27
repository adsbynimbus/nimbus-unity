using System.Collections;
using Example.Scripts.NotAdRelated;
using Nimbus.Internal;
using Nimbus.Runtime.Scripts;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for a full screen ad and also how to manually subscribe to Nimbus Ad events
	/// </summary>
	public class FullScreenExample : MonoBehaviour {
		public GameObject cloud;
		private NimbusAdUnit _ad;
		private bool _alreadyTriggered;

		private void Awake() {
			UnityThread.InitUnityThread();
		}

		private void Start() {
			NimbusManager.Instance.NimbusEvents.OnAdCompleted += RewardUser;
			NimbusManager.Instance.NimbusEvents.OnAdError += LogError;
		}

		// called as for extra safety. Ensures all resources are released
		private void OnDestroy() {
			NimbusManager.Instance.NimbusEvents.OnAdCompleted -= RewardUser;
			NimbusManager.Instance.NimbusEvents.OnAdError -= LogError;
			_ad?.Destroy();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			_ad = NimbusManager.Instance.RequestHybridFullScreenAndLoad("unity_demo_rewarded_fullscreen_position");
			_alreadyTriggered = true;
		}

		private IEnumerator MakeItRain() {
			cloud.SetActive(true);
			yield return new WaitForSeconds(4);
			ScoreUI.Instance.UpdateScore(100);
		}

		private void RewardUser(NimbusAdUnit ad, bool skipped) {
			if (_ad?.InstanceID != ad.InstanceID) return;
			if (skipped) return;
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad was rendered for ad instance {ad.InstanceID}, " +
				$"bid value: {ad.BidResponse.BidRaw}, " +
				$"bid value in cents: {ad.BidResponse.BidInCents}, " +
				$"network: {ad.BidResponse.Network}, " +
				$"placement_id: {ad.BidResponse.PlacementId}, " +
				$"auction_id: {ad.BidResponse.AuctionId}");
			// ensures that this coroutine starts on the Unity Main thread since this is called within an event callback
			UnityThread.ExecuteInUpdate(() => StartCoroutine(MakeItRain()));
		}

		private void LogError(NimbusAdUnit ad) {
			if (_ad?.InstanceID != ad.InstanceID) return;
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad failed to load {ad.InstanceID}, " +
				$"Error Message Ad failed to load {ad.ErrResponse.Message}, " +
				$"auction_id: {ad.BidResponse.AuctionId}");
		}
	}
}