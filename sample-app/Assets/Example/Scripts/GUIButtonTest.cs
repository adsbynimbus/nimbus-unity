using System;
using System.Collections;
using System.Collections.Generic;
using AdsByNimbus;
using AdsByNimbus.RTB;
using AdsByNimbus.RTB.Request;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using App = AdsByNimbus.RTB.App;
using User = AdsByNimbus.RTB.User;

namespace Example.Scripts {
	/// <summary>
	///     This demonstrates how to call for various different ad types within the demo app context
	/// </summary>
	public class GUIButtonTest : MonoBehaviour, IAdEventsExtended {
		[SerializeField] private TextMeshProUGUI _loadedBannerButtonText;
		[SerializeField] private TextMeshProUGUI _loadedDynamicUnitButtonText;
		[SerializeField] private TextMeshProUGUI _errorText;
		[SerializeField] private List<AdController> _interactableButtons;

		private InlineAd _loadAndShowBannerAdUnit;
		private InlineAd _loadAndShowDynamicUnitAdUnit;
		private bool _shouldDestroyDynamicUnit;
		private bool _shouldDestroyBanner;

		private void Start()
		{
			Nimbus.configuration.sessionId = ("session_test");
			Nimbus.configuration.coppa = true;
			var app = new App("com.test.nimbusUnity", Array.Empty<string>(), "nimbus.co", "testapp", Array.Empty<string>(),
				true, true, new Publisher(Array.Empty<string>(), "nimbus.co", "nimbus"), 
				Array.Empty<string>(), "www.nimbus.co", "3.0.0");
			Nimbus.configuration.app = app;
			Nimbus.configuration.user = new User(30, "custom", Gender.male, "keywords");;
			Nimbus.configuration.blockedAdvertisingDomains = new[] { "www.yahoo.com", "www.reddit.com"};
			Nimbus.configuration.interceptorTimeout = 1000;
		}

		private void Awake() {
			Screen.orientation = ScreenOrientation.Portrait;
			_errorText.text = "";
		}

		public void OnAdLoaded(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was loaded");
		}

		public void OnAdWasRendered(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was rendered");
		}

		public void OnAdImpression(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} fired it's impression pixel");
		}

		public void OnAdDestroyed(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} ad was destroyed");
		}

		public void OnAdClicked(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was clicked");
		}

		public void OnAdCompleted(Ad nimbusAdUnit, bool skipped) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was completed");
		}

		public void OnAdError(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} could not be rendered.");
		}
		
		public void OnAdRewardEarned(Ad nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} has given a reward.");
		}

		public void LoadAndShowBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				_loadedBannerButtonText.text = "Destroy Banner";
				_loadAndShowBannerAdUnit = 
					Nimbus.bannerAd("unity_demo_banner_position", 
							bidFloor: 0.05f, screenPosition: AdScreenPosition.BOTTOM_CENTER, components: new()
				{
					new app(new [] { "pagecat1","pagecat2" }, 
						new [] { "sectioncat1","sectioncat2" }),
					new location(0.0, 0.0, LocationType.gps, 20),
					new user("gaming,puzzle")
				});
				_loadAndShowBannerAdUnit.Show();
				return;
			}
			_loadAndShowBannerAdUnit?.Destroy();
			_loadAndShowBannerAdUnit = null;
			_shouldDestroyBanner = false;
			_loadedBannerButtonText.text = "Load And Show Banner";
		}
		
		public void LoadAndShowDynamicUnit() {
			if (!_shouldDestroyDynamicUnit) {
				_shouldDestroyDynamicUnit = true;
				_loadedDynamicUnitButtonText.text = "Destroy Dynamic Unit";
				_loadAndShowDynamicUnitAdUnit = 
					Nimbus.dynamicUnit("unity_demo_dynamicunit_position", screenPosition: AdScreenPosition.BOTTOM_CENTER);
				_loadAndShowDynamicUnitAdUnit.Show();
				return;
			}

			_loadAndShowDynamicUnitAdUnit?.Destroy();
			_loadAndShowDynamicUnitAdUnit = null;
			_shouldDestroyDynamicUnit = false;
			_loadedDynamicUnitButtonText.text = "Load And Show Dynamic Unit";
		}

		public void LoadAndShowInterstitial() {
			Nimbus.interstitialAd("unity_demo_interstitial_position", new Format[] { Format.halfScreen }, 
				AdOrientation.portrait, 0.10f).Show();
		}

		public void LoadAndShowRewardedVideoAd() {
			Nimbus.rewardedAd("unity_demo_video_position", AdOrientation.portrait, 0.05f).Show();
		}

		public void LoadAdController(int index) {
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (_interactableButtons[index].state) {
				case AdState.NotLoaded:
					RequestForAd(index);
					break;
				case AdState.Loaded:
					var currentAd = _interactableButtons[index].CurrentAd;
					currentAd.Show();
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
				AdType.Inline => Nimbus.bannerAd("unity_demo_banner_position", screenPosition: AdScreenPosition.BOTTOM_CENTER),
				AdType.Fullscreen => Nimbus.interstitialAd(
					"unity_demo_interstitial_position"),
				AdType.Rewarded => Nimbus.rewardedAd("unity_demo_video_position"),
				_ => _interactableButtons[index].CurrentAd
			};
		}

		private IEnumerator SetErrorText(string text, AdController controller) {
			_errorText.text = text;
			yield return new WaitForSeconds(2);
			_errorText.text = "";
			controller.ResetState();
		}
		
		private static IEnumerator ResetState(AdController controller, Ad adUnit) {
			if (adUnit.AdType != AdType.Fullscreen && adUnit.AdType != AdType.Rewarded) yield break;
			while (adUnit.CurrentAdState != AdEvent.COMPLETED ||
			       adUnit.CurrentAdState != AdEvent.DESTROYED) {
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
		public AdType adUnitType;
		public Ad CurrentAd;

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