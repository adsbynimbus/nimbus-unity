using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal;
using Nimbus.Internal.Network;
using Nimbus.Internal.RequestBuilder;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Runtime.Scripts {
	[DisallowMultipleComponent]
	public class NimbusManager : MonoBehaviour {
		[field: SerializeField] private NimbusSDKConfiguration _configuration;
		
		private bool _isTheApplicationBackgrounded;
		private NimbusClient _nimbusClient;
		private NimbusAPI _nimbusPlatformAPI;
		private GlobalRtbRegulation _regulations;
		private CancellationTokenSource _ctx;

		public AdEvents NimbusEvents;
		public static NimbusManager Instance;

		private void Awake() {
			if (_configuration == null) throw new Exception("The configuration object cannot be null");

			if (Instance == null) {
				_nimbusPlatformAPI = _nimbusPlatformAPI ?? new
#if UNITY_EDITOR
					Editor
#elif UNITY_ANDROID
                Android
#else
                IOS
#endif
					();

				Debug.unityLogger.logEnabled = _configuration.enableUnityLogs;
				NimbusEvents = new AdEvents();
				_regulations = new GlobalRtbRegulation();
				_nimbusPlatformAPI.InitializeSDK(_configuration);
				_ctx = new CancellationTokenSource();
				_nimbusClient = new NimbusClient(_ctx, _configuration, _nimbusPlatformAPI.GetVersion());
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this) {
				Destroy(gameObject);
			}
		}

		private IEnumerator Start() {
			yield return new WaitForEndOfFrame();
			AutoUnsubscribe();
			AutoSubscribe();
			yield return null;
		}

		private void OnDisable() {
			_ctx.Cancel();
			AutoUnsubscribe();
		}

		private void OnApplicationPause(bool isPaused) {
			_isTheApplicationBackgrounded = isPaused;
		}

		[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
		[SuppressMessage("ReSharper", "InvertIf")]
		private static void AutoSubscribe() {
			if (Instance == null) return;
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance.NimbusEvents.OnAdLoaded += iAdEvent.OnAdLoaded;
				Instance.NimbusEvents.OnAdRendered += iAdEvent.OnAdWasRendered;
				Instance.NimbusEvents.OnAdError += iAdEvent.OnAdError;
				Instance.NimbusEvents.OnAdClicked += iAdEvent.OnAdClicked;
				Instance.NimbusEvents.OnAdCompleted += iAdEvent.OnAdCompleted;

				if (iAdEvent is IAdEventsExtended iAdEventExt) {
					Instance.NimbusEvents.OnAdImpression += iAdEventExt.OnAdImpression;
					Instance.NimbusEvents.OnAdDestroyed += iAdEventExt.OnAdDestroyed;
				}

				if (iAdEvent is IAdEventsVideoExtended iAdEventVideoExt) {
					Instance.NimbusEvents.OnVideoAdPaused += iAdEventVideoExt.OnVideoAdPaused;
					Instance.NimbusEvents.OnVideoAdResume += iAdEventVideoExt.OnVideoAdResume;
				}
			}
		}

		[SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
		[SuppressMessage("ReSharper", "InvertIf")]
		private static void AutoUnsubscribe() {
			if (Instance == null) return;
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance.NimbusEvents.OnAdLoaded -= iAdEvent.OnAdLoaded;
				Instance.NimbusEvents.OnAdRendered -= iAdEvent.OnAdWasRendered;
				Instance.NimbusEvents.OnAdError -= iAdEvent.OnAdError;
				Instance.NimbusEvents.OnAdClicked -= iAdEvent.OnAdClicked;
				Instance.NimbusEvents.OnAdCompleted -= iAdEvent.OnAdCompleted;

				if (iAdEvent is IAdEventsExtended iAdEventExt) {
					Instance.NimbusEvents.OnAdImpression -= iAdEventExt.OnAdImpression;
					Instance.NimbusEvents.OnAdDestroyed -= iAdEventExt.OnAdDestroyed;
				}

				if (iAdEvent is IAdEventsVideoExtended iAdEventVideoExt) {
					Instance.NimbusEvents.OnVideoAdPaused -= iAdEventVideoExt.OnVideoAdPaused;
					Instance.NimbusEvents.OnVideoAdResume -= iAdEventVideoExt.OnVideoAdResume;
				}
			}
		}
		
		
		/// <summary>
		///     RequestAdAndLoad communicates the RTB object data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners and attempts the render the returned ad immediately.
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bidRequest">
		///     A manually constructed RTB data object. While it allows for more publisher flexibility to communicate to Nimbus
		///		it does require more RTB knowledge. Please reference https://github.com/adsbynimbus/nimbus-openrtb/wiki/Nimbus-S2S-Documentation
		///		for more insight on how to construct an RTB request. You may also use the static helper class NimbusRtbBidRequestHelper
		///		to auto fill some data.
		/// </param>
		public NimbusAdUnit RequestAdAndLoad(string nimbusReportingPosition, BidRequest bidRequest) {
			var adUnit = RequestAd(nimbusReportingPosition, bidRequest);
			ShowLoadedAd(adUnit);
			return adUnit;
		}
		

		/// <summary>
		///     RequestBannerAdAndLoad communicates the RTB object data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners and attempts the render the returned ad immediately.
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bannerFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		public NimbusAdUnit RequestBannerAdAndLoad(string nimbusReportingPosition, float bannerFloor = 0f) {
			var adUnit = RequestBannerAd(nimbusReportingPosition, bannerFloor);
			ShowLoadedAd(adUnit);
			return adUnit;
		}

		
		/// <summary>
		///     RequestHybridFullScreenAndLoad pre constructs a Nimbus hybrid auction RTB object and communicates
		///		data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners. Note, though RTB Banner and Video objects are
		///		being sent, creative types if of either type will only be returned if matching placements have been
		///		set up operationally by the Demand partner and Nimbus team. Attempts the render the returned ad immediately.
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bannerFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		/// <param name="videoFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for VAST video creatives
		/// </param>
		public NimbusAdUnit RequestHybridFullScreenAndLoad(string nimbusReportingPosition, float bannerFloor = 0f,
			float videoFloor = 0f) {
			var adUnit = RequestHybridFullScreenAd(nimbusReportingPosition, bannerFloor, videoFloor);
			ShowLoadedAd(adUnit);
			return adUnit;
		}

		
		/// <summary>
		///     RequestRewardVideoAd pre constructs a Nimbus Video auction RTB object and communicates
		///		data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners. Reward in RTB is not defined as a creative
		///		type, but rather a rendering behavior.  Attempts the render the returned ad immediately.
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="videoFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		public NimbusAdUnit RequestRewardVideoAdAndLoad(string nimbusReportingPosition, float videoFloor = 0f) {
			var adUnit = RequestRewardVideoAd(nimbusReportingPosition, videoFloor);
			ShowLoadedAd(adUnit);
			return adUnit;
		}
		
		
		/// <summary>
		///     RequestRefreshingBannerAdAndLoad pre constructs a RTB Banner and sends requests to Nimbus periodically to retrieve ads.
		///		This uses async rather than Unity Coroutines
		/// </summary>
		/// <param name="source">
		///		Passes a context to the Coroutine to signal if and when the coroutine should stop running in the background
		/// </param>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bannerFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		/// <param name="refreshIntervalInSeconds">
		///		Defines the rate at which Banner ads are refreshed with a new ad.
		///		Defaults to the IAB recommended 30 seconds. Nimbus does not allow anything lower than 20 seconds
		/// </param>
		public async void RequestRefreshingBannerAdAndLoad(CancellationTokenSource source,
			string nimbusReportingPosition, float bannerFloor = 0f,
			int refreshIntervalInSeconds = 30) {

			NimbusAdUnit nextAdUnit = null; 
			var delay = (refreshIntervalInSeconds <= 20 ? 30: refreshIntervalInSeconds) * 1000;
			var currentAdUnit = RequestBannerAdAndLoad(nimbusReportingPosition, bannerFloor);
			while (!source.IsCancellationRequested) {
				try {
					await Task.Delay(delay, source.Token);
					
					// stop making updates if the application is backgrounded
					while (!source.IsCancellationRequested && _isTheApplicationBackgrounded) {
						await Task.Yield();
					}
					
					// make sure to reset to avoid any leaks
					nextAdUnit?.Destroy();
					nextAdUnit = RequestBannerAd(nimbusReportingPosition, bannerFloor);
					
					// while the ad was not fully returned and we did not receive an error from Nimbus
					// we give the response some time to load into the ad unit object
					while (!nextAdUnit.WasAnAdReturned() && nextAdUnit.ErrResponse.Message.IsNullOrEmpty()) {
						await Task.Yield();
					}
					
					// confirm that the ad was returned, it will be false if there was an error
					if (!nextAdUnit.WasAnAdReturned()) {
						nextAdUnit?.Destroy();
						continue;
					}
	
					// nextAdUnit was fully returned, we can swap the current unit out with the new returned banner
					currentAdUnit?.Destroy();
					currentAdUnit = nextAdUnit;
					ShowLoadedAd(currentAdUnit);
				}
				catch (TaskCanceledException) {
					break;
				}
			}
			// requesting behavior killed
			currentAdUnit?.Destroy();
			nextAdUnit?.Destroy();
		}
		

		/// <summary>
		///     ShowLoadedAd a returned ad from Nimbus servers and attempts to render it on screen
		/// </summary>
		/// <param name="adUnit">
		///     A reference to a ad unit returned from RequestAd, RequestHybridFullScreenAd, RequestBannerAd, or
		///     RequestRewardVideoAd
		/// </param>
		public void ShowLoadedAd(NimbusAdUnit adUnit) {
			if (adUnit == null) {
				Debug.unityLogger.LogError("Nimbus",
					"there was no ad to render, likely there was no fill meaning that demand did not want to spend");
				return;
			}
			if (adUnit.WasAdRendered()) {
				Debug.unityLogger.LogError(adUnit.InstanceID.ToString(),
					"the was already rendered, you cannot render the same ad twice");
				return;
			}
			StartCoroutine(LoadAd(adUnit));
		}


		private IEnumerator LoadAd(NimbusAdUnit adUnit) {
			while (adUnit.ErrResponse.Message.IsNullOrEmpty()) {
				if (adUnit.WasAnAdReturned()) {
					_nimbusPlatformAPI.ShowAd(adUnit);
					yield break;
				}
				yield return null;
			}
			
			if (!adUnit.ErrResponse.Message.IsNullOrEmpty()) {
				Debug.unityLogger.LogError(adUnit?.InstanceID.ToString(),
					$"error retrieving the ad message: {adUnit?.ErrResponse.Message}");
			}
		}
		

		/// <summary>
		///     RequestAd communicates the RTB object data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners 
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bidRequest">
		///     A manually constructed RTB data object. While it allows for more publisher flexibility to communicate to Nimbus
		///		it does require more RTB knowledge. Please reference https://github.com/adsbynimbus/nimbus-openrtb/wiki/Nimbus-S2S-Documentation
		///		for more insight on how to construct an RTB request. You may also use the static helper class NimbusRtbBidRequestHelper
		///		to auto fill some data.
		/// </param>
		public NimbusAdUnit RequestAd(string nimbusReportingPosition, BidRequest bidRequest) {
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition);
			var adUnitType = AdUnitHelper.BidRequestToAdType(bidRequest);
			return RequestForNimbusAdUnit(bidRequest, adUnitType);
		}
		
		
		/// <summary>
		///     RequestHybridFullScreenAd pre constructs a Nimbus hybrid auction RTB object and communicates
		///		data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners. Note, though RTB Banner and Video objects are
		///		being sent, creative types if of either type will only be returned if matching placements have been
		///		set up operationally by the Demand partner and Nimbus team. 
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bannerFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		/// <param name="videoFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for VAST video creatives
		/// </param>
		public NimbusAdUnit RequestHybridFullScreenAd(string nimbusReportingPosition, float bannerFloor = 0f,
			float videoFloor = 0f) {
			
			const AdUnitType adUnitType = AdUnitType.Interstitial;
			var bidRequest = NimbusRtbBidRequestHelper.ForHybridInterstitialAd(nimbusReportingPosition);
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition).
				SetBannerFloor(bannerFloor).
				SetVideoFloor(videoFloor);
			
			return RequestForNimbusAdUnit(bidRequest, adUnitType);
		}

		
		/// <summary>
		///     RequestBannerAd pre constructs a Nimbus Banner auction RTB object and communicates
		///		data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners.
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="bannerFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		public NimbusAdUnit RequestBannerAd(string nimbusReportingPosition, float bannerFloor = 0f) {
			const AdUnitType adUnitType = AdUnitType.Banner;
			
			var bidRequest = NimbusRtbBidRequestHelper.ForBannerAd(nimbusReportingPosition);
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition).
				SetBannerFloor(bannerFloor);
			
			return RequestForNimbusAdUnit(bidRequest, adUnitType);
		}

		/// <summary>
		///     RequestRewardVideoAd pre constructs a Nimbus Video auction RTB object and communicates
		///		data to Nimbus servers to invoke a server side auction to potentially return a
		///		bid from one of the publishers integrated demand partners. Reward in RTB is not defined as a creative
		///		type, but rather a rendering behavior.
		/// </summary>
		/// <param name="nimbusReportingPosition">
		///     Allows you to see ad revenue attributed to the string value in the Nimbus UI. Useful for publishers
		///		to create custom reporting breakouts
		/// </param>
		/// <param name="videoFloor">
		///		Allows the publisher to optionally set the RTB minimum bid value for HTML/Static creatives
		/// </param>
		public NimbusAdUnit RequestRewardVideoAd(string nimbusReportingPosition, float videoFloor = 0f) {
			const AdUnitType adUnitType = AdUnitType.Rewarded;
			
			var bidRequest = NimbusRtbBidRequestHelper.ForVideoInterstitialAd(nimbusReportingPosition);
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition).
				AttemptToShowVideoEndCard().
				SetVideoFloor(videoFloor).
				SetRewardedVideoFlag();

			return RequestForNimbusAdUnit(bidRequest, adUnitType);
		}
		
		/// <summary>
		///     If this inventory is subject to GDPR regulations use this function to pass in RTB GDPR information for all Nimbus requests
		/// </summary>
		/// <param name="gdprConsentString">
		///		If the user is subject to GDPR pass in the CMP generated consent string
		/// </param>
		public void SetGdprConsent(string gdprConsentString) {
			_regulations.GdprConsentString = gdprConsentString;
		}
	
		/// <summary>
		///     If this inventory is subject to CCPA regulations use this function to pass in RTB CCPA information for all Nimbus requests
		/// </summary>
		/// <param name="usPrivacyString">
		///		The user generated CCPA consent string
		/// </param>
		public void SetUsPrivacyString(string usPrivacyString) {
			_regulations.UsPrivacyString = usPrivacyString;
		}
		
		/// <summary>
		///     If this inventory is subject to COPPA restrictions use this function to pass in RTB COPPA information for all Nimbus requests
		/// </summary>
		/// <param name="isCoppa">
		///		Signals that the inventory is under that age of 13
		/// </param>
		public void SetCoppa(bool isCoppa) {
			_regulations.Coppa = isCoppa;
			_nimbusPlatformAPI.SetCoppaFlag(isCoppa);
		}
		
		public void SetNimbusSDKConfiguration(NimbusSDKConfiguration configuration) {
			_configuration = configuration;
		}
		
		public NimbusSDKConfiguration GetNimbusConfiguration() {
			return _configuration;
		}

		#region Private helpers
		
		// with test = 1 Nimbus servers check that the app object is present
		// in production app data properly construction on Nimbus from database information assuming the account is not a 1:many account
		// if the account is 1:many the publisher needs submit proper App object
		private void SetTestData(BidRequest bidRequest) {
			if (_configuration.enableSDKInTestMode) bidRequest.SetAppName(Application.productName);
		}

		private void SetRegulations(BidRequest bidRequest) {
			bidRequest.SetCoppa(_regulations.Coppa);
			bidRequest.SetGdprConsentString(_regulations.GdprConsentString);
			bidRequest.SetUsPrivacy(_regulations.UsPrivacyString);
		}

		private BidRequest ApplyInterceptors(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			if (_nimbusPlatformAPI.Interceptors() == null) {
				return bidRequest;
			}
			
			foreach (var interceptor in _nimbusPlatformAPI.Interceptors()) {
				var data = interceptor.GetProviderRtbDataFromNativeSDK(adUnitType, isFullScreen);
				bidRequest = interceptor.ModifyRequest(bidRequest, data);
			}
			return bidRequest;
		}

#if  UNITY_IOS
		private async Task<string> MakeRequestAsyncWithInterceptor(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			return await Task.Run(async () => {
				bidRequest = ApplyInterceptors(bidRequest, adUnitType, isFullScreen);
				if (adUnitType == AdUnitType.Interstitial || adUnitType == AdUnitType.Rewarded) {
					_nimbusClient.AddHeader("Nimbus-Test-EnableNewRenderer", "true");
				}
				return await  _nimbusClient.MakeRequestAsync(bidRequest);
			});
		}
#else
		private async Task<string> MakeRequestAsyncWithInterceptor(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			bidRequest = ApplyInterceptors(bidRequest, adUnitType, isFullScreen);
			if (adUnitType == AdUnitType.Interstitial || adUnitType == AdUnitType.Rewarded) {
				_nimbusClient.AddHeader("Nimbus-Test-EnableNewRenderer", "true");
			}
			return await  _nimbusClient.MakeRequestAsync(bidRequest);
		}
#endif
		
		private NimbusAdUnit RequestForNimbusAdUnit(BidRequest bidRequest, AdUnitType adUnitType) {
			Task<string> responseJson;
			try {
				responseJson = MakeRequestAsyncWithInterceptor(bidRequest, adUnitType, AdUnitHelper.IsAdTypeFullScreen(adUnitType));
			} catch (Exception e) { 
				responseJson = Task.FromException<string>(e);
			}
			var adUnit = new NimbusAdUnit(adUnitType, NimbusEvents);
			adUnit.LoadJsonResponseAsync(responseJson);
			return adUnit;
		}
		
		private BidRequest SetUniversalRtbData(BidRequest bidRequest, string position) {
			bidRequest.
				SetSessionId(_nimbusPlatformAPI.GetSessionID()).
				SetDevice(_nimbusPlatformAPI.GetDevice()).
				SetTest(_configuration.enableSDKInTestMode).
				SetReportingPosition(position).
				SetOMInformation(_nimbusClient.platformSdkv);
			SetTestData(bidRequest);
			SetRegulations(bidRequest);
			return bidRequest;
		}
		
		private class GlobalRtbRegulation {
			internal bool Coppa;
			internal string GdprConsentString;
			internal string UsPrivacyString;
		}
		
		#endregion
	}

}