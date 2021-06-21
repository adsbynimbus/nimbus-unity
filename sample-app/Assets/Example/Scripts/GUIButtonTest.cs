using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using TMPro;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for various different ad types within the demo app context
	/// </summary>
	public class GUIButtonTest : MonoBehaviour, IAdEventsExtended {
		public TextMeshProUGUI bannerButtonText;
		private NimbusAdUnit _bannerAdUnit;

		private string _currentBannerButtonText;
		private bool _shouldDestroyBanner;

		private void Awake() {
			Screen.orientation = ScreenOrientation.Portrait;
		}

		private void Start() {
			if (bannerButtonText == null)
				foreach (var txt in FindObjectsOfType<TextMeshProUGUI>()) {
					if (txt.name != "BannerTextTMP") continue;
					bannerButtonText = txt;
					break;
				}

			_currentBannerButtonText = bannerButtonText.text;
		}

		public void OnAdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.ResponseMetaData.AuctionID} was rendered");
		}

		public void OnAdImpression(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.ResponseMetaData.AuctionID} fired it's impression pixel");
		}

		public void OnAdDestroyed(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.ResponseMetaData.AuctionID} ad was destroyed");
		}

		public void OnAdClicked(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.ResponseMetaData.AuctionID} was clicked");
		}

		public void OnAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.ResponseMetaData.AuctionID} was completed");
		}

		public void OnAdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} could not be rendered {nimbusAdUnit.ErrorMessage()}");
		}

		public void LoadBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				bannerButtonText.text = "Destroy Banner";
				_bannerAdUnit = NimbusManager.Instance.LoadAndShowBannerAd("unity_demo_banner_position", 0.0f);
				return;
			}

			_bannerAdUnit?.Destroy();
			_bannerAdUnit = null;
			_shouldDestroyBanner = false;
			bannerButtonText.text = _currentBannerButtonText;
		}

		public void LoadInterstitial() {
			_ = NimbusManager.Instance.LoadAndShowFullScreenAd("unity_demo_interstitial_position", 0.0f, 0.0f);
		}

		public void LoadRewardedVideoAd() {
			_ = NimbusManager.Instance.LoadAndShowRewardedVideoAd("unity_demo_video_position", 0.0f);
		}
	}
}