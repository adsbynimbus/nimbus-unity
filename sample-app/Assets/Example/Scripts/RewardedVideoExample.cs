using System;
using System.Collections;
using AdsByNimbus;
using Example.Scripts.NotAdRelated;
using AdsByNimbus.Internal;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for a rewarded ad by implementing the IAdEvents interface which the NimbusManager
	///     auto subscribes to
	/// </summary>
	public class RewardedVideoExample : MonoBehaviour, IAdEventsExtended {
		public GameObject cloud;

		// keep a reference of the returned ad so that it can be safely cleaned up
		private RewardedAd _ad;
		private bool _alreadyTriggered;

		private void Awake() {
			UnityThread.InitUnityThread();
		}

		private void OnDestroy() {
			_ad?.destroy();
		}

		private void OnTriggerEnter2D(Collider2D other) {
			var player = other.gameObject.GetComponent<NimbusPlayerController>();
			if (player == null || _alreadyTriggered) return;
			Nimbus.rewardedAd("unity_demo_rewarded_video_position").show();
			_alreadyTriggered = true;
		}

		public void OnAdLoaded(Ad nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				$"RewardedVideoExample Ad was returned and loaded into memory");
		}

		public void OnAdWasRendered(Ad nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				"RewardedVideoExample Ad was rendered");
		}

		public void OnAdImpression(Ad nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				"RewardedVideoExample Ad impression was fired");
		}

		public void OnAdClicked(Ad nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				"RewardedVideoExample Ad was clicked");
		}

		public void OnAdDestroyed(Ad nimbusAdUnit) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				"RewardedVideoExample Ad was destroyed/removed from the scene");
		}

		public void OnAdCompleted(Ad nimbusAdUnit, bool skipped) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			if (skipped) return;
			Debug.unityLogger.Log(
				"RewardedVideoExample Ad was completed");
		}

		public void OnAdRewardEarned(Ad nimbusAdUnit)
		{
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log(
				"RewardedVideoExample Ad reward was earned and the user can be rewarded");
			UnityThread.ExecuteInUpdate(RewardUser);
		}
		
		public void OnAdError(Ad nimbusAdUnit, NimbusError nimbusError) {
			if (_ad?.InstanceID != nimbusAdUnit.InstanceID) return;
			Debug.unityLogger.Log($"RewardedVideoExample Error");
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