using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nimbus.Internal;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.LiveRamp;
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
		private NimbusAPI _nimbusPlatformAPI;
		private Regs _regulations;
		private List<Segment> _userData = new ();
		private CancellationTokenSource _ctx;
		public AdEvents NimbusEvents;
		public static NimbusManager Instance;

		private void Awake() {
			if (_configuration == null) throw new Exception("The configuration object cannot be null");

			if (Instance == null) {
				Debug.unityLogger.logEnabled = _configuration.enableUnityLogs;
				_nimbusPlatformAPI = _nimbusPlatformAPI ?? new
				#if UNITY_EDITOR
					Editor
				#elif UNITY_ANDROID
					Android
				#else
					IOS
				#endif
					();
				NimbusEvents = new AdEvents();
				_ctx = new CancellationTokenSource();
				if (!_configuration.enableManualInitialization)
				{
					InitializeNimbusSDK();
				}
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
		///    Method to manually initialize the Nimbus SDK instead of initialization happening on Awake()
		/// </summary>
		public void InitializeNimbusSDK()
		{
			if (!_configuration.sdkInitialized)
			{
				_configuration.sdkInitialized = true;
				_nimbusPlatformAPI.InitializeSDK(_configuration);
				var privacyRegs = NimbusPrivacyHelpers.getPrivacyRegulations();
				if (privacyRegs != null)
				{
					_regulations = privacyRegs;
				}
			}
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
		/// <param name="adSize">
		///		Allows the publisher to optionally set the Banner Size (only supports Banner320x50 and Leaderboard)
		/// </param>
		/// <param name="respectSafeArea">
		///		Allows the publisher to choose whether the banner ads respect the safe area or not.
		/// </param>
		/// <param name="adPosition">
		///		Enum that allows the publisher to choose the position of the banner ad relative to the screen.
		/// </param>
		public NimbusAdUnit RequestBannerAdAndLoad(string nimbusReportingPosition, 
				IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner320X50, bool respectSafeArea = false, 
				NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER) {
			var adUnit = new NimbusAdUnit(AdUnitType.Banner, NimbusEvents, nimbusReportingPosition, adSize, respectSafeArea, adPosition);
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
		public NimbusAdUnit RequestHybridFullScreenAndLoad(string nimbusReportingPosition) {
			var adUnit = new NimbusAdUnit(AdUnitType.Interstitial, NimbusEvents, nimbusReportingPosition);
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
		public NimbusAdUnit RequestRewardVideoAdAndLoad(string nimbusReportingPosition) {
			var adUnit = new NimbusAdUnit(AdUnitType.Rewarded, NimbusEvents, nimbusReportingPosition);
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
		/// <param name="adSize">
		///		Allows the publisher to optionally set the Banner Size (only supports Banner320x50 and Leaderboard)
		/// </param>
		/// <param name="respectSafeArea">
		///		Allows the publisher to choose whether the banner ads respect the safe area or not.
		/// </param>
		/// <param name="adPosition">
		///		Enum that allows the publisher to choose the position of the banner ad relative to the screen.
		/// </param>
		public NimbusAdUnit RequestRefreshingBannerAdAndLoad(CancellationTokenSource source,
			string nimbusReportingPosition, int refreshIntervalInSeconds = 30, IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner320X50,
			bool respectSafeArea = false, NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER) {
			var adUnit = new NimbusAdUnit(AdUnitType.Banner, NimbusEvents, nimbusReportingPosition, adSize, respectSafeArea, adPosition, refreshIntervalInSeconds);
			//FIGURE OUT HOW TO PASS THROUGH CANCEL
			ShowLoadedAd(adUnit);
			return adUnit;
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
			StartCoroutine(LoadAd(adUnit, true));
		}

		private void LoadAdinNativeSDKButDontShow(NimbusAdUnit adUnit)
		{
			StartCoroutine(LoadAd(adUnit, false));
		}

		private IEnumerator LoadAd(NimbusAdUnit adUnit, bool showAd)
		{
			_nimbusPlatformAPI.getAd(adUnit, showAd);
			yield break;
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
		public NimbusAdUnit RequestHybridFullScreenAd(string nimbusReportingPosition) {
			var adUnit = new NimbusAdUnit(AdUnitType.Interstitial, NimbusEvents, nimbusReportingPosition);
			LoadAdinNativeSDKButDontShow(adUnit);
			return adUnit;
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
		/// <param name="adSize">
		///		Allows the publisher to optionally set the Banner Size (only supports Banner320x50 and LeaderBoard)
		/// </param>
		/// <param name="respectSafeArea">
		///		Allows the publisher to choose whether the banner ads respect the safe area or not.
		/// </param>
		/// <param name="adPosition">
		///		Enum that allows the publisher to choose the position of the banner ad relative to the screen.
		/// </param>
		public NimbusAdUnit RequestBannerAd(string nimbusReportingPosition, IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner320X50,
			bool respectSafeArea = false, NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER) {
			var adUnit = new NimbusAdUnit(AdUnitType.Banner, NimbusEvents, nimbusReportingPosition, adSize, respectSafeArea, adPosition);
			LoadAdinNativeSDKButDontShow(adUnit);
			return adUnit;
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
		public NimbusAdUnit RequestRewardVideoAd(string nimbusReportingPosition) {
			var adUnit = new NimbusAdUnit(AdUnitType.Rewarded, NimbusEvents, nimbusReportingPosition);
			LoadAdinNativeSDKButDontShow(adUnit);
			return adUnit;
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
		///     If this inventory is subject to COPPA restrictions use this function to get the passed in RTB COPPA information for all Nimbus requests
		/// </summary>
		/// <return>
		///		Returns if COPPA as enabled or not
		/// </return>
		public Boolean GetCoppa() {
			if (_regulations == null)
			{
				return false;
			}
			return _regulations.Coppa == 1;
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
			//if (_configuration.enableSDKInTestMode) bidRequest.SetAppName(Application.productName);
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
		#endregion
	}

}