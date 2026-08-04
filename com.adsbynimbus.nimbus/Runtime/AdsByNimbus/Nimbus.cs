using System;
using System.Collections.Generic;
using Internal;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public static class Nimbus
{
	/// <summary>
	///    Method to manually initialize the Nimbus SDK instead of initialization happening on Awake()
	/// </summary>
	public static void initialize(string publisherKey, string apiKey)
	{
		NimbusManager.Instance.InitializeNimbusSDK();
	}
	
	/// <summary>
	///     bannerAd uses the RTB object data and creates an InlineAd object.  The InlineAd's methods
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
	public static InlineAd bannerAd(string nimbusReportingPosition, float bannerFloor = 0f,
		IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner, bool respectSafeArea = false,
		NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, int refreshIntervalInSeconds = 30,
		RequestModifiers requestModifiers = new RequestModifiers())
	{
		return new InlineAd(NimbusManager.Instance.NimbusEvents, nimbusReportingPosition, adPosition, modifiers: requestModifiers, adSize,
			respectSafeArea,
			refreshIntervalInSeconds, bannerBidFloor: bannerFloor);
	}

	/// <summary>
	///     fullscreenAd preconstructs a Nimbus hybrid auction RTB object.  The InlineAd's methods
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
	/// <param name="staticFloor">
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
	public static FullscreenAd fullscreenAd(string nimbusReportingPosition,
		float staticFloor = 0f, float videoFloor = 0f, RequestModifiers requestModifiers = new RequestModifiers())
	{
		return new FullscreenAd(NimbusManager.Instance.NimbusEvents, nimbusReportingPosition,
			bannerBidFloor: staticFloor, videoBidFloor: videoFloor, modifiers: requestModifiers);
	}

	/// <summary>
	///     rewardedAd preconstructs a Nimbus Video auction RTB object and communicates
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
	public static RewardedAd rewardedAd(string nimbusReportingPosition, float videoFloor = 0f,
		RequestModifiers requestModifiers = new RequestModifiers())
	{
		return new RewardedAd(NimbusManager.Instance.NimbusEvents, nimbusReportingPosition,
			videoBidFloor: videoFloor, modifiers: requestModifiers);
	}
	
	public static class configuration
	{
			
		/// <summary>
		///		Unique session id for the current app session
		/// </summary>
		/// <param name="sessionId">
		///		string for the preferred session Id
		/// </param>

		public static string sessionId
		{
			set => ConfigHelpers.SetSessionId(value);
		}

		/// <summary>
		///     If this inventory is subject to COPPA restrictions use this function to get the passed in RTB COPPA information for all Nimbus requests
		/// </summary>
		/// <param name="coppa">
		///		boolean depending on whether coppa restrictions are in place
		/// </param>
		public static bool coppa
		{
			set => ConfigHelpers.SetCoppa(value);
		}

		/// <summary>
		///		Details about the human user of the device; the advertising audience
		/// </summary>
		public static User user
		{
			set => ConfigHelpers.SetUser(value);
		}

		/// <summary>
		///		Identifies the app to buyers (e.g., bundle ID, store URL, name, categories, publisher, privacy flags)
		/// </summary>
		public static App app
		{
			set => ConfigHelpers.SetApp(value);
		}

		/// <summary>
		///		Block list of advertisers by their domains (e.g., “ford.com”)
		/// </summary>
		public static string[] blockedAdvertisingDomains
		{
			set => ConfigHelpers.SetBlockedAdvertisingDomains(value);
		}

		/// <summary>
		///		Set Request URL for bid requests
		/// </summary>
		public static string requestUrl
		{
			set => ConfigHelpers.SetRequestUrl(value);
		}

		/// <summary>
		///		Set additional request headers
		/// </summary>
		public static Dictionary<string, string> additionalRequestHeaders
		{
			set => ConfigHelpers.SetAdditionalRequestHeaders(value);
		}

		/// <summary>
		///		Maximum time (in milliseconds) interceptors have to modify the request before it fires. Default is 500 milliseconds.
		/// </summary>
		public static int interceptorTimeout
		{
			set => ConfigHelpers.SetInterceptorTimeout(value);
		}

		/// <summary>
		///		Whether the video player should show the mute button. True by default
		/// </summary>
		public static bool showMuteButton
		{
			set => ConfigHelpers.ShowMuteButton(value);
		}

		/// <summary>
		///		If enabled, only tap gestures are allowed for inline ads. Default is false
		///		(iOS only setting)
		/// </summary>
		public static bool enableSwipeProtection
		{
			set => ConfigHelpers.EnableSwipeProtection(value);
		}

		/// <summary>
		///		Sets if SKOverlay is enabled for all ad units (iOS only setting)
		/// </summary>
		public static bool isSkOverlayEnabledForAllUnits
		{
			set => ConfigHelpers.SetIsSkOverlayEnabledForAllUnits(value);
		}

		/// <summary>
		///		Set Verification Providers for Ad Viewability Tracking (OM SDK)
		/// </summary>
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
		public static VerificationProvider[] verificationProviders
		{
			set => ConfigHelpers.SetVerificationProviders(value);
		}
	}
}

/// <summary>
///     Modifiers Added to an Ad on a per-request basis
/// </summary>
public struct RequestModifiers
{
	// Adds per-request app categories to the RTB request.
	public PerRequestApp? app;
	// A banner creative to be attached to the ad request.
	public BannerCreative? banner;
	// Overrides the environment for a single ad.
	public Env? environment;
	// Adds device geolocation to the RTB request.
	public Location? location;
	// Adds per-request user keywords to the RTB request.
	//A comma-separated keyword string to assign to the RTB User object. 
	[CanBeNull] public String userKeywords;
	// Attaches a video creative to the ad request.
	public VideoCreative? video;
	// Adds viewability information to the RTB request.
	public Viewability? viewability;


	public RequestModifiers(PerRequestApp? app = null, BannerCreative? banner = null, 
		Env? environment = null, Location? location = null, [CanBeNull] string userKeywords = null, 
		VideoCreative? video = null, Viewability? viewability = null)
	{
		this.app = app;
		this.banner = banner;
		this.environment = environment;
		this.location = location;
		this.userKeywords = userKeywords;
		this.video = video;
		this.viewability = viewability;
	}
}

/// <summary>
    ///     A verification provider for ad viewability tracking
    /// </summary>
    public class VerificationProvider: AndroidJavaProxy
    {
        /// <summary>
        ///     This callback is fired once a bid response is received from Nimbus.  
        /// </summary>
        /// <param name="nimbusBidResponseMarkup">
        ///     Returns the bid response markup
        /// </param>
        /// <returns>
        ///     A string that provides markup to be injected into a static ad.
        /// </returns>
        public Func<string, string> VerificationMarkupCallback;
        
        /// <summary>
        ///     This callback is fired once a bid response is received from Nimbus.  
        /// </summary>
        /// <param name="nimbusBidResponseMarkup">
        ///     Returns the bid response markup
        /// </param>
        /// <returns>
        ///     VerificationScriptResource that is passed to the OM SDK.
        /// </returns>
        public Func<string, VerificationScriptResource> VerificationResourceCallback;

        public VerificationProvider(Func<string, string> verificationMarkupCallback, Func<string, VerificationScriptResource> verificationResourceCallback):
            base("com.adsbynimbus.unity.VerificationProviderCallbackInterface")
        {
            VerificationMarkupCallback = verificationMarkupCallback;
            VerificationResourceCallback = verificationResourceCallback;
        }
        

        // Internal Nimbus method, do not use. Must be public for implementation.
        public string _verificationMarkupCallback(string response) 
        {
            return VerificationMarkupCallback(response);
        }
        
        // Internal Nimbus method, do not use.  Must be public for implementation.
        public string _verificationResourceCallback(string response) 
        {
            return JsonConvert.SerializeObject(VerificationResourceCallback(response));
        }

        public class VerificationScriptResource
        {
            public string url;
            public string vendorKey;
            public string parameters;

            public VerificationScriptResource(string url, string vendorKey, string parameters)
            {
                this.url = url;
                this.vendorKey = vendorKey;
                this.parameters = parameters;
            }
        }
    }