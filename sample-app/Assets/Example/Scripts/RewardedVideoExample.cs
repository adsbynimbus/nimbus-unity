using System;
using System.Collections;
using Nimbus.Scripts;
using Nimbus.Scripts.Internal;
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
			_ad = NimbusManager.Instance.LoadAndShowRewardedVideoAd("unity_demo_rewarded_video_position");
			_alreadyTriggered = true;
		}

		public void AdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad was rendered for ad instance {nimbusAdUnit.InstanceID}, bid value: {nimbusAdUnit.GetBidValue()}, network: {nimbusAdUnit.GetNetwork()}");
		}

		public void AdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log($"NimbusEventListenerExample Err {nimbusAdUnit.ErrorMessage()}");
		} // ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
		public void AdEvent(NimbusAdUnit nimbusAdUnit) {
			if (nimbusAdUnit.AdType != AdUnityType.Rewarded) return;
			switch (nimbusAdUnit.GetCurrentAdState()) {
				case AdEventTypes.NOT_LOADED:
					break;
				case AdEventTypes.LOADED:
					break;
				case AdEventTypes.PAUSED:
					break;
				case AdEventTypes.RESUME:
					break;
				case AdEventTypes.CLICKED:
					break;
				case AdEventTypes.FIRST_QUARTILE:
					break;
				case AdEventTypes.MIDPOINT:
					break;
				case AdEventTypes.THIRD_QUARTILE:
					break;
				case AdEventTypes.COMPLETED:
					Debug.unityLogger.Log("Rewarding the player for watching the whole video!");
					UnityThread.ExecuteInUpdate(RewardUser);
					break;
				case AdEventTypes.IMPRESSION:
					break;
				case AdEventTypes.DESTROYED:
					break;
				default:
					throw new ArgumentOutOfRangeException();
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