using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nimbus.Internal;
using Nimbus.Internal.Extensions;
using Nimbus.Internal.Extensions.AdMob;
using Nimbus.RTB;
using Nimbus.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nimbus.Runtime.Scripts {
	[DisallowMultipleComponent]
	public class NimbusManager : MonoBehaviour {
		[field: SerializeField] private NimbusSDKConfiguration _configuration;
		
		private bool _isTheApplicationBackgrounded;
		public NimbusAPI NimbusPlatformAPI;
		private CancellationTokenSource _ctx;
		public AdEvents NimbusEvents;
		public static NimbusManager Instance;
		private bool _coppa;

		private void Awake() {
			if (_configuration == null) throw new Exception("The configuration object cannot be null");

			if (Instance == null) {
				Debug.unityLogger.logEnabled = _configuration.enableUnityLogs;
				NimbusPlatformAPI = NimbusPlatformAPI ?? new
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
				Instance = this;
				if (!_configuration.enableManualInitialization)
				{
					InitializeNimbusSDK();
				}
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
					Instance.NimbusEvents.OnAdRewardEarned += iAdEventExt.OnAdRewardEarned;
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
				NimbusPlatformAPI.InitializeSDK(_configuration);
			}
		}
		
		
		/// <summary>
		///     BannerAd uses the RTB object data and creates an InlineAd object.  The InlineAd's methods
		///		Load() and/or Show() can then be called to communicate to Nimbus servers and invoke a server side auction
		///     to potentially return a bid from one of the publishers integrated demand partners and then show it.
		///		Reminder: Load() should only be called if the ad needs to be cached beforehand.  Show() does not
		///		need Load() to be called first.
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
		/// <param name="refreshIntervalInSeconds">
		///		Defines the rate at which Banner ads are refreshed with a new ad.
		///		Defaults to the IAB recommended 30 seconds. Nimbus does not allow anything lower than 10 seconds
		/// </param>
		/// <param name="requestModifiers">
		///		Object that allows the publisher to add modifiers on a per-request basis
		/// </param>
		/// <returns>
		///		InlineAd object that correlates to the Requested Ad
		/// </returns>
		public InlineAd BannerAd(string nimbusReportingPosition, float bannerFloor = 0f,
				IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner, bool respectSafeArea = false, 
				NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, int refreshIntervalInSeconds = 30,
				RequestModifiers requestModifiers = new RequestModifiers()) {
			return new InlineAd(NimbusEvents, nimbusReportingPosition, adPosition, modifiers: requestModifiers, adSize, respectSafeArea,  
				refreshIntervalInSeconds, bannerBidFloor: bannerFloor);
		}
				
		/// <summary>
		///     FullscreenAd pre constructs a Nimbus hybrid auction RTB object.  The InlineAd's methods
		///		Load() and/or Show() can then be called to communicate to Nimbus servers and invoke a server side auction
		///     to potentially return a bid from one of the publishers integrated demand partners and then show it.
		///		Note: though RTB Banner and Video objects are being sent, creative types
		///		if of either type will only be returned if matching placements have been set up operationally
		///		by the Demand partner and Nimbus team. 
		///		Reminder: Load() should only be called if the ad needs to be cached beforehand.  Show() does not
		///		need Load() to be called first.
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
		/// <param name="requestModifiers">
		///		Object that allows the publisher to add modifiers on a per-request basis
		/// </param>
		/// <returns>
		///		NimbusAdUnit that correlates to the Requested Ad
		/// </returns>
		public FullscreenAd FullscreenAd(string nimbusReportingPosition, 
			float bannerFloor = 0f, float videoFloor = 0f, RequestModifiers requestModifiers = new RequestModifiers()) {
			return new FullscreenAd(NimbusEvents, nimbusReportingPosition, 
				bannerBidFloor: bannerFloor, videoBidFloor: videoFloor, modifiers: requestModifiers);
		}
		
		/// <summary>
		///     RewardedAd pre constructs a Nimbus Video auction RTB object and communicates
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
		/// <param name="requestModifiers">
		///		Object that allows the publisher to add modifiers on a per-request basis
		/// </param>
		/// <returns>
		///		NimbusAdUnit that correlates to the Requested Ad
		/// </returns>
		public RewardedAd RewardedAd(string nimbusReportingPosition, float videoFloor = 0f, 
			RequestModifiers requestModifiers = new RequestModifiers()) {
			return new RewardedAd(NimbusEvents, nimbusReportingPosition, 
				videoBidFloor: videoFloor, modifiers: requestModifiers);
		}
		
		/// <summary>
		///		Unique session id for the current app session
		/// </summary>
		/// <param name="sessionId">
		///		string for the preferred session Id
		/// </param>

		public void SetSessionId(string sessionId)
		{
			ConfigHelpers.SetSessionId(sessionId);
		}

		/// <summary>
		///     If this inventory is subject to COPPA restrictions use this function to get the passed in RTB COPPA information for all Nimbus requests
		/// </summary>
		/// <param name="coppa">
		///		boolean depending on whether coppa restrictions are in place
		/// </param>
		public void SetCoppa(bool coppa)
		{
			ConfigHelpers.SetCoppa(coppa);
		}
		
		/// <summary>
		///     Details about the human user of the device; the advertising audience
		/// </summary>
		/// <param name="user">
		///		RTB User object with customizable properties
		/// </param>
		public void SetUser(User user)
		{
			ConfigHelpers.SetUser(user);
		}

		/// <summary>
		///		Identifies the app to buyers (e.g., bundle ID, store URL, name, categories, publisher, privacy flags)
		/// </summary>
		/// <param name="app">
		///		RTB App object with customizable properties
		/// </param>
		public void SetApp(App app)
		{
			ConfigHelpers.SetApp(app);
		}

		/// <summary>
		///		Block list of advertisers by their domains (e.g., “ford.com”)
		/// </summary>
		/// <param name="blockedAdvertisingDomains">
		///		string array of blocked domains
		/// </param>
		public void SetBlockedAdvertisingDomains(string[] blockedAdvertisingDomains)
		{
			ConfigHelpers.SetBlockedAdvertisingDomains(blockedAdvertisingDomains);
		}

		/// <summary>
		///		Set Request URL for bid requests
		/// </summary>
		/// <param name="requestUrl"/>
		public void SetRequestUrl(string requestUrl)
		{
			ConfigHelpers.SetRequestUrl(requestUrl);
		}

		/// <summary>
		///		Set additional request headers
		/// </summary>
		/// <param name="additionalRequestHeaders"/>
		public void SetAdditionalRequestHeaders(Dictionary<string, string> additionalRequestHeaders)
		{
			ConfigHelpers.SetAdditionalRequestHeaders(additionalRequestHeaders);
		}

		/// <summary>
		///		Maximum time (in milliseconds) interceptors have to modify the request before it fires. Default is 500 milliseconds.
		/// </summary>
		/// <param name="interceptorTimeoutInMillis"></param>
		public void SetInterceptorTimeout(int interceptorTimeoutInMillis)
		{
			ConfigHelpers.SetInterceptorTimeout(interceptorTimeoutInMillis);
		}

		/// <summary>
		///		Whether the video player should show the mute button. True by default
		/// </summary>
		/// <param name="showMuteButton"/>
		public void ShowMuteButton(bool showMuteButton)
		{
			ConfigHelpers.ShowMuteButton(showMuteButton);
		}

		/// <summary>
		///		If enabled, only tap gestures are allowed for inline ads. Default is false
		///		(iOS only setting)
		/// </summary>
		/// <param name="enableSwipeProtection"/>
		public void EnableSwipeProtection(bool enableSwipeProtection)
		{
			ConfigHelpers.EnableSwipeProtection(enableSwipeProtection);
		}

		/// <summary>
		///		Sets if SKOverlay is enabled for all ad units (iOS only setting)
		/// </summary>
		/// <param name="isSkOverlayEnabledForAllUnits"/>
		public void SetIsSkOverlayEnabledForAllUnits(bool isSkOverlayEnabledForAllUnits)
		{
			ConfigHelpers.SetIsSkOverlayEnabledForAllUnits(isSkOverlayEnabledForAllUnits);
		}

		/// <summary>
		///		Set Verification Providers for Ad Viewability Tracking (OM SDK)
		/// </summary>
		/// <param name="providers">
		///		Array of Verification Providers with callback methods
		/// </param>
		/*
		 * /// Example of the methods in the Native Nimbus iOS SDK
		   public protocol VerificationProvider : Sendable {
		   
		       func verificationMarkup(response: NimbusKit.NimbusResponse) -> String
		   
		       func verificationResource(response: NimbusKit.NimbusResponse) -> NimbusKit.VerificationScriptResource?
		   }
		   
		   public struct VerificationScriptResource {
		   
		       public init?(url: URL, vendorKey: String?, parameters: String?)
		   }
		 */
		public void SetVerificationProviders(VerificationProvider[] providers)
		{
			ConfigHelpers.SetVerificationProviders(providers);
		}
		
		#if NIMBUS_ENABLE_LIVERAMP
		/// <summary>
		///     This method will initialize the LiveRamp Identity SDK
		/// </summary>
		/// <param name="configId">
		///		Config ID provided by LiveRamp
		/// </param>
		/// <param name="email">
		///		Email is the preferred method for identifying a user
		/// </param>
		/// <param name="hasConsentForNoLegislation">
		///		Set to true if the user is not governed by consent laws (i.e CCPA/GDPR)
		///		Refer to https://developers.liveramp.com/authenticatedtraffic-api/docs/init-best-practices#consent-requirements
		/// </param>
		/// <param name="isTestMode">
		///		Set to true if wishing to use test mode.
		/// </param>
			public static void initializeLiveRamp(String configId, String email,
				Boolean hasConsentForNoLegislation, Boolean isTestMode)
			{
					// if Nimbus SDK hasn't been initialized yet, wait for SDK initialization
					NimbusLiveRampHelpers.initializeLiveRamp(configId, email, hasConsentForNoLegislation, isTestMode);
			}
		#endif
		
		public void SetNimbusSDKConfiguration(NimbusSDKConfiguration configuration) {
			_configuration = configuration;
		}
		
		public NimbusSDKConfiguration GetNimbusConfiguration() {
			return _configuration;
		}

	}

}