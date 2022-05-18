using System;
using System.Collections.Generic;
using Nimbus.Runtime.Scripts;
using Nimbus.Runtime.Scripts.Internal;
using TMPro;
using UnityEngine;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for various different ad types within the demo app context
	/// </summary>
	public class GUIButtonTest : MonoBehaviour, IAdEventsExtended {
		[SerializeField] private TextMeshProUGUI _loadedBannerButtonText;
		[SerializeField] private List<AdController> _interactableButtons;

		private NimbusAdUnit _loadAndShowBannerAdUnit;
		private bool _shouldDestroyBanner;
		
		private void Awake() {
			Screen.orientation = ScreenOrientation.Portrait;
		}
		
		public void OnAdLoaded(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} for auction id {nimbusAdUnit.ResponseMetaData.AuctionID} was loaded");
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

		public void LoadAndShowBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				_loadedBannerButtonText.text = "Destroy Banner";
				_loadAndShowBannerAdUnit = NimbusManager.Instance.LoadAndShowBannerAd("unity_demo_banner_position", 0.0f);
				return;
			}

			_loadAndShowBannerAdUnit?.Destroy();
			_loadAndShowBannerAdUnit = null;
			_shouldDestroyBanner = false;
			_loadedBannerButtonText.text = "Load And Show Banner";
		}

		public void LoadAndShowInterstitial() {
			_ = NimbusManager.Instance.LoadAndShowFullScreenAd("unity_demo_interstitial_position", 0.0f, 0.0f);
		}

		public void LoadAndShowRewardedVideoAd() {
			_ = NimbusManager.Instance.LoadAndShowRewardedVideoAd("unity_demo_video_position", 0.0f);
		}
		
		public void LoadAdController(int index) {
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (_interactableButtons[index].state) {
				case AdState.NotLoaded:
					LoadAd(index);
					break;
				case AdState.Loaded:
					_interactableButtons[index].CurrentAd  = NimbusManager.Instance.ShowLoadedAd(ref _interactableButtons[index].CurrentAd);
					break;
				case AdState.Displayed:
					_interactableButtons[index].DestroyAd();
					break;
			}
			_interactableButtons[index].NextState();
		}
		
		private void LoadAd(int index) {
			var adType = _interactableButtons[index].adUnityType;
			_interactableButtons[index].CurrentAd = adType switch {
				AdUnityType.Banner => NimbusManager.Instance.LoadBannerAd("unity_demo_banner_position", 0.0f),
				AdUnityType.Interstitial => NimbusManager.Instance.LoadFullScreenAd(
					"unity_demo_interstitial_position", 0.0f, 0.0f),
				AdUnityType.Rewarded => NimbusManager.Instance.LoadRewardedVideoAd("unity_demo_video_position",
					0.0f),
				_ => _interactableButtons[index].CurrentAd
			};
		}
	}
	
	
	[Serializable]
	public class AdController {
		[HideInInspector] public AdState state;
		public TextMeshProUGUI button;
		public AdUnityType adUnityType;
		public NimbusAdUnit CurrentAd;
		
		public void NextState() {
			state = (AdState) (((int) state + 1) % 3);
			button.text = state switch {
				AdState.NotLoaded => $"Load {adUnityType}",
				AdState.Loaded => $"{adUnityType} Loaded, Display?",
				AdState.Displayed => $"{adUnityType} Displayed, Destroy?",
				_ => button.text
			};
		}

		public void DestroyAd() {
			if (state != AdState.Displayed) return;
			CurrentAd?.Destroy();
			CurrentAd = null;
		}
	}
	
	[Serializable]
	public enum AdState: uint {
		NotLoaded,
		Loaded, 
		Displayed,
	}
}