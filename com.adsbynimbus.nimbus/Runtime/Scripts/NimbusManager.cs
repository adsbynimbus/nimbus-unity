using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nimbus.Internal;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Moloco;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
using Nimbus.Internal.LiveRamp;
using Nimbus.Internal.Network;
using Nimbus.Internal.RequestBuilder;
using Nimbus.Internal.Session;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gender = OpenRTB.Request.Gender;

namespace Nimbus.Runtime.Scripts {
	[DisallowMultipleComponent]
	public class NimbusManager : MonoBehaviour {
		[field: SerializeField] private NimbusSDKConfiguration _configuration;
		
		private bool _isTheApplicationBackgrounded;
		private NimbusClient _nimbusClient;
		private NimbusAPI _nimbusPlatformAPI;
		private Regs _regulations;
		private List<Segment> _userData = new ();
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
				_nimbusPlatformAPI.InitializeSDK(_configuration);
				var privacyRegs = NimbusPrivacyHelpers.getPrivacyRegulations();
				if (privacyRegs != null)
				{
					_regulations = privacyRegs;
				}
				_ctx = new CancellationTokenSource();
				_nimbusClient = new NimbusClient(_ctx, _configuration, _nimbusPlatformAPI.GetVersion());
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this) {
				Destroy(gameObject);
			}
		}
		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			AutoUnsubscribe();
			AutoSubscribe();
			SceneManager.sceneLoaded -= OnSceneLoaded;

			// SceneLoaded gets called BEFORE Start on app/game start
			SceneManager.sceneLoaded += OnSceneLoaded;
			yield return null;
		}
		
		// Listener for sceneLoaded
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			AutoUnsubscribe();
			AutoSubscribe();
		}

		private void OnDisable() {
			_ctx?.Cancel();
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
		///     If this inventory is subject to CCPA regulations use this function to pass in RTB CCPA information for all Nimbus requests
		/// </summary>
		/// <param name="usPrivacyString">
		///		The user generated CCPA consent string
		/// </param>
		public void SetUsPrivacyString(string usPrivacyString) {
			_regulations.Ext ??= new RegExt();
			_regulations.Ext.UsPrivacy = usPrivacyString;
		}
		
		/// <summary>
		///     If this inventory is subject to COPPA restrictions use this function to pass in RTB COPPA information for all Nimbus requests
		/// </summary>
		/// <param name="isCoppa">
		///		Signals that the inventory is under that age of 13
		/// </param>
		public void SetCoppa(bool isCoppa) {
			_regulations.Coppa = isCoppa ? 1 : 0;
			_nimbusPlatformAPI.SetCoppaFlag(isCoppa);
		}
		
		/// <summary>
		///     Sets the Gender of the User
		/// </summary>
		/// <param name="gender">
		///		enum representing the user's gender (F, M, O)
		/// </param>
		public void SetUserGender(Gender gender)
		{
			var index = _userData.FindIndex(seg => seg.Name == "gender");
			if (index >= 0) 
			{
				_userData[index].Value = gender.ToString();
			} 
			else {
				var genderObj = new Segment();
				genderObj.Name = "gender";
				genderObj.Value = gender.ToString();
				_userData.Add(genderObj);
			}	
		}
		
		/// <summary>
		///     Sets the Age of the User
		/// </summary>
		/// <param name="age">
		///		integer greater than 0 representing user's age
		/// </param>
		public void SetUserAge(int age) {
			var index = _userData.FindIndex(seg => seg.Name == "age");
			if (index >= 0) 
			{
				_userData[index].Value = age.ToString();
			} 
			else {
				var ageObj = new Segment();
				ageObj.Name = "age";
				ageObj.Value = age.ToString();
				_userData.Add(ageObj);
			}				
		}

		internal BidRequest ApplyUserData(BidRequest bidRequest)
		{
			if (_userData.Count > 0)
			{
				bidRequest.User ??= new User();
				var data = new Data();
				data.Name = "nimbus";
				data.Segment = _userData.ToArray();
				bidRequest.User.Data = (bidRequest.User.Data == null ? 
					new[] { data } : new List<Data>(bidRequest.User.Data) { data }.ToArray());
			}
			return bidRequest;
		}
		
		#if NIMBUS_ENABLE_LIVERAMP
		/// <summary>
		///     This method will initialize the LiveRamp Identity SDK
		/// </summary>
		/// <param name="configId">
		///		Config ID provided by LiveRamp
		/// </param>
		/// <param name="hasConsentForNoLegislation">
		///		Set to true if the user is not governed by consent laws (i.e CCPA/GDPR)
		///		Refer to https://developers.liveramp.com/authenticatedtraffic-api/docs/init-best-practices#consent-requirements
		/// </param>
		/// <param name="email">
		///		Email is the preferred method for identifying a user, if null will attempt to use phone number
		/// </param>
		///  <param name="phoneNumber">
		///		Optional phone if email isn't known, only US is supported
		/// </param>
		/// <param name="testMode">
		///		Optional parameter if debugging / testing
		/// </param>
			public static void initializeLiveRamp(String configId,
				Boolean hasConsentForNoLegislation, String email, 
				String phoneNumber = "", Boolean testMode = false)
			{
					// if Nimbus SDK hasn't been initialized yet, wait for SDK initialization
					NimbusLiveRampHelpers.initializeLiveRamp(configId, hasConsentForNoLegislation, testMode,
							email, phoneNumber);
			}
		#endif
		
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
			bidRequest.Regs = _regulations;
		}

		private async Task<BidRequest> ApplyInterceptors(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			if (_nimbusPlatformAPI.Interceptors() == null) {
				return bidRequest;
			}

			var interceptorTasks = new List<Task<BidRequestDelta>>();
			foreach (var interceptor in _nimbusPlatformAPI.Interceptors())
			{
				interceptorTasks.Add(interceptor.GetBidRequestDeltaAsync(adUnitType, isFullScreen, bidRequest).TimeoutWithResult(2000));
			}
			try
			{
				var results = await Task.WhenAll(interceptorTasks);
				bidRequest = BidRequestDeltaManager.ApplyDeltas(results, bidRequest);
			}
			catch (Exception e)
			{
				Debug.unityLogger.Log($"NIMBUS INTERCEPTOR ERROR: {e.Message}  {e.StackTrace}");
			}

			return bidRequest;
		}

#if  UNITY_IOS
		private async Task<string> MakeRequestAsyncWithInterceptor(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			return await Task.Run(async () => {
				bidRequest = await ApplyInterceptors(bidRequest, adUnitType, isFullScreen);
				return await  _nimbusClient.MakeRequestAsync(bidRequest);
			});
		}
#else
		private async Task<string> MakeRequestAsyncWithInterceptor(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			bidRequest = await ApplyInterceptors(bidRequest, adUnitType, isFullScreen);
			return await  _nimbusClient.MakeRequestAsync(bidRequest);
		}
#endif
		
		private NimbusAdUnit RequestForNimbusAdUnit(BidRequest bidRequest, AdUnitType adUnitType) {
			Task<string> responseJson;
			responseJson = MakeRequestAsyncWithInterceptor(bidRequest, adUnitType, AdUnitHelper.IsAdTypeFullScreen(adUnitType));
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
			#if NIMBUS_ENABLE_LIVERAMP
				bidRequest = NimbusLiveRampHelpers.addLiveRampToRequest(bidRequest);
			#endif
			bidRequest = NimbusSessionHelpers.addSessionToRequest(bidRequest);
			bidRequest = ApplyUserData(bidRequest);
			SetTestData(bidRequest);
			SetRegulations(bidRequest);
			return bidRequest;
		}
		
		private class GlobalRtbRegulation {
			internal bool Coppa;
			internal bool GdprApplies;
			internal string GdprConsentString;
			internal string GppConsentString;
			internal string GppSectionId;
			internal string UsPrivacyString;
		}
		
		#endregion
		
		/// <summary>
		///		This method is ONLY for the Sample App
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
		public NimbusAdUnit RequestThirdPartyBannerAdAndLoad(ThirdPartyDemand thirdParty, string nimbusReportingPosition, float bannerFloor = 0f) {
			var adUnit = RequestThirdPartyBannerAd(thirdParty, nimbusReportingPosition, bannerFloor);
			ShowLoadedAd(adUnit);
			return adUnit;
		}

		
		/// <summary>
		///		This method is ONLY for the Sample App
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
		public NimbusAdUnit RequestThirdPartyFullScreenAndLoad(ThirdPartyDemand thirdParty, string nimbusReportingPosition, float bannerFloor = 0f,
			float videoFloor = 0f) {
			var adUnit = RequestThirdPartyHybridFullScreenAd(thirdParty, nimbusReportingPosition, bannerFloor, videoFloor);
			ShowLoadedAd(adUnit);
			return adUnit;
		}

		
		/// <summary>
		///		This method is ONLY for the Sample App
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
		public NimbusAdUnit RequestThirdPartyRewardedVideoAdAndLoad(ThirdPartyDemand thirdParty, string nimbusReportingPosition, float videoFloor = 0f) {
			var adUnit = RequestThirdPartyRewardVideoAd(thirdParty, nimbusReportingPosition, videoFloor);
			ShowLoadedAd(adUnit);
			return adUnit;
		}
		
				
		
		/// <summary>
		///		This method is ONLY for the Sample App
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
		public NimbusAdUnit RequestThirdPartyHybridFullScreenAd(ThirdPartyDemand thirdParty, string nimbusReportingPosition, float bannerFloor = 0f,
			float videoFloor = 0f) {
			
			const AdUnitType adUnitType = AdUnitType.Interstitial;
			var bidRequest = NimbusRtbBidRequestHelper.ForHybridInterstitialAd(nimbusReportingPosition);
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition).
				SetBannerFloor(bannerFloor).
				SetVideoFloor(videoFloor);
			
			return RequestForThirdPartyNimbusAdUnit(thirdParty, bidRequest, adUnitType);
		}

		
		/// <summary>
		///		This method is ONLY for the Sample App
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
		public NimbusAdUnit RequestThirdPartyBannerAd(ThirdPartyDemand thirdParty, string nimbusReportingPosition, float bannerFloor = 0f) {
			const AdUnitType adUnitType = AdUnitType.Banner;
			
			var bidRequest = NimbusRtbBidRequestHelper.ForBannerAd(nimbusReportingPosition);
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition).
				SetBannerFloor(bannerFloor);
			
			return RequestForThirdPartyNimbusAdUnit(thirdParty, bidRequest, adUnitType);
		}

		/// <summary>
		///		This method is ONLY for the Sample App
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
		public NimbusAdUnit RequestThirdPartyRewardVideoAd(ThirdPartyDemand thirdParty, string nimbusReportingPosition, float videoFloor = 0f) {
			const AdUnitType adUnitType = AdUnitType.Rewarded;
			
			var bidRequest = NimbusRtbBidRequestHelper.ForVideoInterstitialAd(nimbusReportingPosition);
			bidRequest = SetUniversalRtbData(bidRequest, nimbusReportingPosition).
				AttemptToShowVideoEndCard().
				SetVideoFloor(videoFloor).
				SetRewardedVideoFlag();

			return RequestForThirdPartyNimbusAdUnit(thirdParty, bidRequest, adUnitType);
		}
		
#if  UNITY_IOS
		// This method is ONLY for the Sample App
		private async Task<string> MakeRequestAsyncWithSpecificInterceptor(BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			return await Task.Run(async () => {
				bidRequest = await ApplyInterceptors(bidRequest, adUnitType, isFullScreen);
				return await  _nimbusClient.MakeRequestAsync(bidRequest);
			});
		}
#else
		// This method is ONLY for the Sample App
		private async Task<string> MakeRequestAsyncWithSpecificInterceptor(ThirdPartyDemand thirdParty, BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			bidRequest = await ApplySpecificInterceptors(thirdParty, bidRequest, adUnitType, isFullScreen);
			return await  _nimbusClient.MakeRequestAsync(bidRequest);
		}
#endif
		
		// This method is ONLY for the Sample App
		private NimbusAdUnit RequestForThirdPartyNimbusAdUnit(ThirdPartyDemand thirdParty, BidRequest bidRequest, AdUnitType adUnitType) {
			Task<string> responseJson;
			responseJson = MakeRequestAsyncWithSpecificInterceptor(thirdParty, bidRequest, adUnitType, AdUnitHelper.IsAdTypeFullScreen(adUnitType));
			var adUnit = new NimbusAdUnit(adUnitType, NimbusEvents);
			adUnit.LoadJsonResponseAsync(responseJson);
			return adUnit;
		}
		
		// This method is ONLY for the Sample App
		private async Task<BidRequest> ApplySpecificInterceptors(ThirdPartyDemand thirdParty, BidRequest bidRequest, AdUnitType adUnitType, bool isFullScreen) {
			if (_nimbusPlatformAPI.Interceptors() == null) {
				return bidRequest;
			}

			var interceptorTasks = new List<Task<BidRequestDelta>>();
			var checkType = typeof(AdMobAndroid);

			#if UNITY_IOS
			switch (thirdParty)
			{
				case ThirdPartyDemand.AdMob:
					checkType = typeof(AdMobIOS);
					break;
				case ThirdPartyDemand.APS:
					checkType = typeof(ApsIOS);
					break;
				case ThirdPartyDemand.InMobi:
					//checkType = typeof(InMobiIOS);
					break;
				case ThirdPartyDemand.Meta:
					checkType = typeof(MetaIOS);
					break;
				case ThirdPartyDemand.Mintegral:
					checkType = typeof(MintegralIOS);
					break;
				case ThirdPartyDemand.MobileFuse:
					checkType = typeof(MobileFuseIOS);
					break;
				case ThirdPartyDemand.Moloco:
					checkType = typeof(MolocoIOS);
					break;
				case ThirdPartyDemand.UnityAds:
					checkType = typeof(UnityAdsIOS);
					break;
				case ThirdPartyDemand.Vungle:
					checkType = typeof(VungleIOS);
					break;
			}
			#elif UNITY_ANDROID
			switch (thirdParty)
			{
				case ThirdPartyDemand.AdMob:
					checkType = typeof(AdMobAndroid);
					break;
				case ThirdPartyDemand.APS:
					checkType = typeof(ApsAndroid);
					break;
				case ThirdPartyDemand.InMobi:
					//checkType = typeof(InMobiAndroid);
					break;
				case ThirdPartyDemand.Meta:
					checkType = typeof(MetaAndroid);
					break;
				case ThirdPartyDemand.Mintegral:
					checkType = typeof(MintegralAndroid);
					break;
				case ThirdPartyDemand.MobileFuse:
					checkType = typeof(MobileFuseAndroid);
					break;
				case ThirdPartyDemand.Moloco:
					checkType = typeof(MolocoAndroid);
					break;
				case ThirdPartyDemand.UnityAds:
					checkType = typeof(UnityAdsAndroid);
					break;
				case ThirdPartyDemand.Vungle:
					checkType = typeof(VungleAndroid);
					break;
			}
			#endif
			foreach (var interceptor in _nimbusPlatformAPI.Interceptors())
			{
				if (interceptor.GetType() == checkType)
				{
					interceptorTasks.Add(interceptor.GetBidRequestDeltaAsync(adUnitType, isFullScreen, bidRequest).TimeoutWithResult(2000));
					break;
				}
			}
			try
			{
				var results = await Task.WhenAll(interceptorTasks);
				bidRequest = BidRequestDeltaManager.ApplyDeltas(results, bidRequest);
			}
			catch (Exception e)
			{
				Debug.unityLogger.Log($"NIMBUS INTERCEPTOR ERROR: {e.Message}  {e.StackTrace}");
			}

			return bidRequest;
		}
		
		public enum ThirdPartyDemand
		{
			AdMob,
			APS,
			InMobi,
			Meta,
			Mintegral,
			MobileFuse,
			Moloco,
			UnityAds,
			Vungle
		}
		
	}

}