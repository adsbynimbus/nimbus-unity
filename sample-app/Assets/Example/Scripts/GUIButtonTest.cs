using System;
using System.Collections;
using System.Collections.Generic;
using Nimbus.Internal;
using Nimbus.Internal.Extensions;
using Nimbus.RTB;
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
		[SerializeField] private TextMeshProUGUI _loadedLeaderboardButtonText;
		[SerializeField] private TextMeshProUGUI _errorText;
		[SerializeField] private List<AdController> _interactableButtons;
		private RequestModifiers _requestModifiers;

		private NimbusAdUnit _loadAndShowBannerAdUnit;
		private NimbusAdUnit _loadAndShowLeaderboardAdUnit;
		private bool _shouldDestroyLeaderboard;
		private bool _shouldDestroyBanner;

		private void Start()
		{
			NimbusManager.Instance.SetSessionId("session_test");
			NimbusManager.Instance.SetCoppa(true);
			var app = new App("com.test.nimbusUnity", Array.Empty<string>(), "nimbus.co", "testapp", Array.Empty<string>(),
				true, true, new Publisher(Array.Empty<string>(), "nimbus.co", "nimbus"), 
				Array.Empty<string>(), "www.nimbus.co", "3.0.0");
			NimbusManager.Instance.SetApp(app);
			var user = new User(30, "custom", Gender.male, "keywords");
			NimbusManager.Instance.SetUser(user);
			const string verificationUrl =
				"https://adsbynimbus-public.s3.amazonaws.com/dev/omid_validation_verification_script_v1.js";
			var vProvider = new VerificationProvider(response =>
				{
					return $"<script src=\"{verificationUrl}\" type=\"text/javascript\"></script>";
				}, s =>
				{
					return new VerificationProvider.VerificationScriptResource(verificationUrl, "iabtechlab-Adsbynimbus", "iabtechlab.com-omid");
				}
			);
			NimbusManager.Instance.SetBlockedAdvertisingDomains(new[] { "www.yahoo.com", "www.reddit.com"});
			NimbusManager.Instance.SetInterceptorTimeout(1000);
			NimbusManager.Instance.SetVerificationProviders(new [] { vProvider });
			var requestApp = new PerRequestApp(new [] { "pagecat1","pagecat2" }, 
				new [] { "sectioncat1","sectioncat2" });
			var bannerCreative = new BannerCreative(320, 50, new[]
			{
				Format.banner, Format.leaderboard, Format.mrec
			}, adPosition: Position.footer, bidFloor: 0.0f, 
				new [] { CreativeAttribute.hasPopup });
			var videoCreative = new VideoCreative(adPosition: Position.fullScreen, 0.0f, 0, 30,
				0, 0, VideoPlacementType.inArticle,
				new[] { PlaybackMethod.clickWithSoundOn, PlaybackMethod.mouseOverWithSoundOn });
			_requestModifiers = new RequestModifiers(app: requestApp, banner: bannerCreative, 
				location: new Location(0.0, 0.0, LocationType.gps, 20), userKeywords: "smart,gaming", 
				video:videoCreative, viewability: new Viewability("omid1", "omid2"));
		}

		private void Awake() {
			Screen.orientation = ScreenOrientation.Portrait;
			_errorText.text = "";
		}

		public void OnAdLoaded(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was loaded");
		}

		public void OnAdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was rendered");
		}

		public void OnAdImpression(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} fired it's impression pixel");
		}

		public void OnAdDestroyed(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} ad was destroyed");
		}

		public void OnAdClicked(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was clicked");
		}

		public void OnAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} was completed");
		}

		public void OnAdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} could not be rendered.");
		}
		
		public void OnAdRewardEarned(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"Ad unit of {nimbusAdUnit.InstanceID} type {nimbusAdUnit.AdType} has given a reward.");
		}

		public void LoadAndShowBanner() {
			if (!_shouldDestroyBanner) {
				_shouldDestroyBanner = true;
				_loadedBannerButtonText.text = "Destroy Banner";
				_loadAndShowBannerAdUnit = 
					NimbusManager.Instance
						.RequestBannerAdAndLoad("unity_demo_banner_position", 
							bannerFloor: 0.05f, requestModifiers: _requestModifiers);
				return;
			}
			_loadAndShowBannerAdUnit?.Destroy();
			_loadAndShowBannerAdUnit = null;
			_shouldDestroyBanner = false;
			_loadedBannerButtonText.text = "Load And Show Banner";
		}
		
		public void LoadAndShowLeaderboard() {
			if (!_shouldDestroyLeaderboard) {
				_shouldDestroyLeaderboard = true;
				_loadedLeaderboardButtonText.text = "Destroy Leaderboard";
				_loadAndShowLeaderboardAdUnit = NimbusManager.Instance.RequestBannerAdAndLoad("unity_demo_leaderboard_position", adSize: IabSupportedAdSizes.LeaderBoard);
				return;
			}

			_loadAndShowLeaderboardAdUnit?.Destroy();
			_loadAndShowLeaderboardAdUnit = null;
			_shouldDestroyLeaderboard = false;
			_loadedLeaderboardButtonText.text = "Load And Show Leaderboard";
		}

		public void LoadAndShowInterstitial() {
			NimbusManager.Instance.RequestHybridFullScreenAndLoad("unity_demo_interstitial_position", 
				bannerFloor: 0.05f, videoFloor: 0.03f, requestModifiers: _requestModifiers);
		}

		public void LoadAndShowRewardedVideoAd() {
			NimbusManager.Instance.RequestRewardVideoAdAndLoad("unity_demo_video_position", videoFloor: 0.03f,
				requestModifiers: _requestModifiers);
		}

		public void LoadAdController(int index) {
			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (_interactableButtons[index].state) {
				case AdState.NotLoaded:
					RequestForAd(index);
					break;
				case AdState.Loaded:
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
				AdType.Banner => NimbusManager.Instance.RequestBannerAd("unity_demo_banner_position"),
				AdType.Interstitial => NimbusManager.Instance.RequestHybridFullScreenAd(
					"unity_demo_interstitial_position"),
				AdType.Rewarded => NimbusManager.Instance.RequestRewardVideoAd("unity_demo_video_position"),
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
			if (adUnit.AdType != AdType.Interstitial && adUnit.AdType != AdType.Rewarded) yield break;
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
		public AdType adUnitType;
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