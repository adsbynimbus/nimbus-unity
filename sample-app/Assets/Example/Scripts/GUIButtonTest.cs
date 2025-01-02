using System;
using System.Collections;
using System.Collections.Generic;
using Nimbus.Internal;
using Nimbus.Runtime.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for various different ad types within the demo app context
	/// </summary>
	public class GUIButtonTest : MonoBehaviour, IAdEventsExtended {
		[SerializeField] private TextMeshProUGUI _loadedBannerButtonText;
		[SerializeField] private TextMeshProUGUI _errorText;
		[SerializeField] private List<AdController> _interactableButtons;

		private NimbusAdUnit _loadAndShowBannerAdUnit;
		private bool _shouldDestroyBanner;

		private void Awake() {
			Screen.orientation = ScreenOrientation.Portrait;
			_errorText.text = "";
		}

		public void OnAdLoaded(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.BidResponse.AuctionId} was loaded");
		}

		public void OnAdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.BidResponse.AuctionId} was rendered");
		}

		public void OnAdImpression(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.BidResponse.AuctionId} fired it's impression pixel");
		}

		public void OnAdDestroyed(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.BidResponse.AuctionId} ad was destroyed");
		}

		public void OnAdClicked(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.BidResponse.AuctionId} was clicked");
		}

		public void OnAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.BidResponse.AuctionId} was completed");
		}

		public void OnAdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} could not be rendered {nimbusAdUnit.ErrResponse.Message}");
		}

		public void LoadAndShowBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				_loadedBannerButtonText.text = "Destroy Banner";
				_loadAndShowBannerAdUnit = NimbusManager.Instance.RequestBannerAdAndLoad("unity_demo_banner_position");
				return;
			}

			_loadAndShowBannerAdUnit?.Destroy();
			_loadAndShowBannerAdUnit = null;
			_shouldDestroyBanner = false;
			_loadedBannerButtonText.text = "Load And Show Banner";
		}

		public void LoadAndShowInterstitial() {
			_ = NimbusManager.Instance.RequestHybridFullScreenAndLoad("unity_demo_interstitial_position");
		}

		public void LoadAndShowRewardedVideoAd() {
			_ = NimbusManager.Instance.RequestRewardVideoAdAndLoad("unity_demo_video_position");
		}

		public void LoadAdController(int index) {
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (_interactableButtons[index].state) {
				case AdState.NotLoaded:
					RequestForAd(index);
					break;
				case AdState.Loaded:
					// check to see if there was an error retrieving the ad such as no bid, which in itself is not an error
					// it simply means demand partners did not want to bid on this inventory
					if (_interactableButtons[index].CurrentAd.ErrResponse.Message != null) {
						var message = _interactableButtons[index].CurrentAd.ErrResponse.Message;
						Debug.unityLogger.LogWarning("AdError", message);
						StartCoroutine(SetErrorText(message, _interactableButtons[index]));
						break;
					}
					var currentAd = _interactableButtons[index].CurrentAd;
					NimbusManager.Instance.ShowLoadedAd(currentAd);
					StartCoroutine(ResetState(_interactableButtons[index], currentAd));
					break;
				case AdState.Displayed:
					_interactableButtons[index].DestroyAd();
					break;
			}

			_interactableButtons[index].NextState();
		}

		private void RequestForAd(int index) {
			var adType = _interactableButtons[index].adUnitType;
			_interactableButtons[index].CurrentAd = adType switch {
				AdUnitType.Banner => NimbusManager.Instance.RequestBannerAd("unity_demo_banner_position"),
				AdUnitType.Interstitial => NimbusManager.Instance.RequestHybridFullScreenAd(
					"unity_demo_interstitial_position"),
				AdUnitType.Rewarded => NimbusManager.Instance.RequestRewardVideoAd("unity_demo_video_position"),
				_ => _interactableButtons[index].CurrentAd
			};
		}

		private IEnumerator SetErrorText(string text, AdController controller) {
			_errorText.text = text;
			yield return new WaitForSeconds(2);
			_errorText.text = "";
			controller.ResetState();
		}
		
		private static IEnumerator ResetState(AdController controller, NimbusAdUnit adUnit) {
			if (adUnit.AdType != AdUnitType.Interstitial && adUnit.AdType != AdUnitType.Rewarded) yield break;
			while (adUnit.CurrentAdState != AdEventTypes.COMPLETED ||
			       adUnit.CurrentAdState != AdEventTypes.DESTROYED) {
				yield return null;
			}
			controller.ResetState();
		}

		public void LoadGame()
		{
			SceneManager.LoadScene("Example/Scenes/NimbusAdShowCase");
		}
	}


	[Serializable]
	public class AdController {
		[HideInInspector] public AdState state;
		public TextMeshProUGUI button;
		public AdUnitType adUnitType;
		public NimbusAdUnit CurrentAd;

		public void NextState() {
			state = (AdState)(((int)state + 1) % 3);
			button.text = state switch {
				AdState.NotLoaded => $"Load {adUnitType}",
				AdState.Loaded => $"{adUnitType} Loaded, Display?",
				AdState.Displayed => $"{adUnitType} Displayed, Destroy?",
				_ => button.text
			};
		}

		public void ResetState() {
			state = AdState.NotLoaded;
			button.text = $"Load {adUnitType}";
		}

		public void DestroyAd() {
			if (state != AdState.Displayed) return;
			CurrentAd?.Destroy();
			CurrentAd = null;
		}
	}


	[Serializable]
	public enum AdState : uint {
		NotLoaded,
		Loaded,
		Displayed
	}
}