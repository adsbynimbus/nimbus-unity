using System;
using System.Collections.Generic;
using AdsByNimbus;
using AdsByNimbus.Internal;
using AdsByNimbus.RTB;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using App = AdsByNimbus.RTB.App;
using User = AdsByNimbus.RTB.User;

public static class Nimbus
{
	/// <summary>
	///    Method to manually initialize the Nimbus SDK instead of initialization happening on Awake()
	/// </summary>
	/// <param name="publisherKey">
	///	   Publisher Key
	/// </param>
	/// <param name="apiKey">
	///	   API Key
	/// </param>
	/// <param name="extensions">
	///	   Optionally Install Extensions
	///		example: Nimbus.initialize("publisherKey", "apiKey", () => {
	///					new APSExtension("appKey")
	///					new AdMobExtension("appId")
	///					etc.
	///				});
	/// </param>
	public static void initialize(string publisherKey, string apiKey, [CanBeNull] Action extensions = null)
	{
		if (extensions != null)
		{
			extensions.Invoke();
		}
		NimbusManager.Instance.InitializeNimbusSDK(publisherKey, apiKey);
	}
	
	/// <summary>
	///		This function creates a banner ad.
	///		Banner ad is presented inline and includes a banner creative by default.
	///		Other types and configuration may be done in request builder the closure.
	/// 	Reminder: Load() should only be called if the ad needs to be cached beforehand.
	///		Show() does not need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="size">
	///		Ad size, default is `AdSize.banner`
	/// </param>\
	/// <param name="addFormats">
	///		Set of additional formats
	/// </param>
	/// <param name="adPosition">
	///		Ad position. Defaults to `RTB.Position.unknown`
	/// </param>
	/// <param name="bidFloor">
	///		Minimum bid for this ad impression expressed in CPM
	/// </param>
	/// <param name="refreshInterval">
	///     Expressed in seconds. 0 = no refresh, 10 is the lowest allowed refresh interval.
	///		Values larger than zero and lower than 10 will be set to 10.
	/// </param>
	/// <param name="screenPosition">
	///		Enum that allows the publisher to choose the position of the banner ad relative to the screen.
	///		Default is AdScreenPosition.BOTTOM_CENTER.
	/// </param>
	/// <param name="respectSafeArea">
	///		Boolean that allows the publisher to choose whether the screenPosition of the ad respects "safe area" bounds
	///		that are set by the respective platforms.  Defaults to false.
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.bannerAd(...,components: new() { new app(...), new user(...), etc.});
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	/// 	WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.bannerAd(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId1", APSAdFormat.Display320X50)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		InlineAd object that correlates to the Requested Ad
	/// </returns>
	public static InlineAd bannerAd(
		string position, 
		AdSize size = AdSize.banner, 
		Format[] addFormats = null, 
		Position adPosition = Position.unknown, 
		float bidFloor = 0f, 
		int refreshInterval = 0, 
		AdScreenPosition screenPosition = AdScreenPosition.BOTTOM_CENTER,
		bool respectSafeArea = false, 
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new InlineAd(NimbusManager.Instance.NimbusEvents, position, size, addFormats, adPosition, bidFloor, 
			refreshInterval, adScreenPosition: screenPosition, respectSafeArea:respectSafeArea, components: components, demand: demand);
	}
	
		/// <summary>
	///		This function creates a banner ad.
	///		Banner ad is presented inline and includes a banner creative by default.
	///		Other types and configuration may be done in request builder the closure.
	/// 	Reminder: Load() should only be called if the ad needs to be cached beforehand.
	///		Show() does not need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="xCoord">
	///		The top-left corner of the banner view is positioned at the x value passed,
	///		where the origin is the top-left of the screen.  
	/// </param>
	/// <param name="yCoord">
	///		The top-left corner of the banner view is positioned at the y value passed,
	///		where the origin is the top-left of the screen. 
	/// </param>
	/// <param name="size">
	///		Ad size, default is `AdSize.banner`
	/// </param>\
	/// <param name="addFormats">
	///		Set of additional formats
	/// </param>
	/// <param name="adPosition">
	///		Ad position. Defaults to `RTB.Position.unknown`
	/// </param>
	/// <param name="bidFloor">
	///		Minimum bid for this ad impression expressed in CPM
	/// </param>
	/// <param name="refreshInterval">
	///     Expressed in seconds. 0 = no refresh, 10 is the lowest allowed refresh interval.
	///		Values larger than zero and lower than 10 will be set to 10.
	/// </param>
	/// <param name="respectSafeArea">
	///		Boolean that allows the publisher to choose whether the screenPosition of the ad respects "safe area" bounds
	///		that are set by the respective platforms.  Defaults to false.
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.bannerAd(...,components: new() { new app(...), new user(...), etc.});
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	/// 	WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.bannerAd(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId1", APSAdFormat.Display320X50)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		InlineAd object that correlates to the Requested Ad
	/// </returns>
	public static InlineAd bannerAd(
		string position,
		int xCoord, 
		int yCoord,
		AdSize size = AdSize.banner,
		Format[] addFormats = null, 
		Position adPosition = Position.unknown, 
		float bidFloor = 0f, 
		int refreshInterval = 0, 
		bool respectSafeArea = false, 
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new InlineAd(NimbusManager.Instance.NimbusEvents, position, size, addFormats, adPosition, bidFloor, 
			refreshInterval, xCoord:xCoord, yCoord:yCoord, respectSafeArea:respectSafeArea, components: components, demand: demand);
	}
	
	/// <summary>
	///     This function creates a dynamic unit ad.
	///		Dynamic unit ad is presented inline and includes banner and video creatives by default.
	///		Other types and configuration may be done in request builder the closure.
	/// 	Reminder: Load() should only be called if the ad needs to be cached beforehand.
	///		Show() does not need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="addFormats">
	///		Set of additional formats, default is Format.mrec, Format.halfScreen
	/// </param>
	/// <param name="orientation">
	///		Preferred orientation of the ad, default is the current device orientation
	/// </param>
	/// <param name="adPosition">
	///		Ad position. Defaults to `RTB.Position.unknown`
	/// </param>
	/// <param name="bidFloor">
	///		Minimum bid for this ad impression expressed in CPM
	/// </param>
	/// <param name="refreshInterval">
	///     Expressed in seconds. 0 = no refresh, 10 is the lowest allowed refresh interval.
	///		Values larger than zero and lower than 10 will be set to 10.
	/// </param>
	/// <param name="width">
	///		Width of the ad's container, defaults to screen width.
	/// </param>
	/// <param name="height">
	///		Height of the ad's container, defaults to screen height.
	/// </param>
	/// <param name="screenPosition">
	///		Enum that allows the publisher to choose the position of the banner ad relative to the screen.
	///		Default is AdScreenPosition.BOTTOM_CENTER.
	/// </param>
	/// <param name="respectSafeArea">
	///		Boolean that allows the publisher to choose whether the screenPosition of the ad respects "safe area" bounds
	///		that are set by the respective platforms.  Defaults to false.
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.dynamicUnit(...,components: new() { new app(...), new user(...), etc.});
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	///		WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.dynamicUnit(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId1", APSAdFormat.Display320X50),
	///					new apsAd("slotId2", APSAdFormat.InterstitialVideo)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		InlineAd object that correlates to the Requested Ad
	/// </returns>
	public static InlineAd dynamicUnit(
		string position, 
		Format[] addFormats = null, 
		AdOrientation orientation = AdOrientation.deviceOrientation,
		Position adPosition = Position.unknown, 
		float bidFloor = 0f, 
		int refreshInterval = 0, 
		int width = 0, 
		int height = 0,
		AdScreenPosition screenPosition = AdScreenPosition.BOTTOM_CENTER,
		bool respectSafeArea = false, 
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new InlineAd(NimbusManager.Instance.NimbusEvents, position, addFormats: addFormats, adPosition:adPosition, bidFloor:bidFloor, 
			refreshInterval:refreshInterval, dynamicUnitWidth:width, dynamicUnitHeight:height, adScreenPosition:screenPosition, 
			respectSafeArea:respectSafeArea, orientation:orientation, dynamicUnit: true, components: components, demand: demand);
	}
	
		/// <summary>
	///     This function creates a dynamic unit ad.
	///		Dynamic unit ad is presented inline and includes banner and video creatives by default.
	///		Other types and configuration may be done in request builder the closure.
	/// 	Reminder: Load() should only be called if the ad needs to be cached beforehand.
	///		Show() does not need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="xCoord">
	///		The top-left corner of the banner view is positioned at the x value passed,
	///		where the origin is the top-left of the screen. 
	/// </param>
	/// <param name="yCoord">
	///		The top-left corner of the banner view is positioned at the y value passed,
	///		where the origin is the top-left of the screen.
	/// </param>
	/// <param name="addFormats">
	///		Set of additional formats, default is Format.mrec, Format.halfScreen
	/// </param>
	/// <param name="orientation">
	///		Preferred orientation of the ad, default is the current device orientation
	/// </param>
	/// <param name="adPosition">
	///		Ad position. Defaults to `RTB.Position.unknown`
	/// </param>
	/// <param name="bidFloor">
	///		Minimum bid for this ad impression expressed in CPM
	/// </param>
	/// <param name="refreshInterval">
	///     Expressed in seconds. 0 = no refresh, 10 is the lowest allowed refresh interval.
	///		Values larger than zero and lower than 10 will be set to 10.
	/// </param>
	/// <param name="width">
	///		Width of the ad's container, defaults to screen width.
	/// </param>
	/// <param name="height">
	///		Height of the ad's container, defaults to screen height.
	/// </param>
	/// <param name="respectSafeArea">
	///		Boolean that allows the publisher to choose whether the screenPosition of the ad respects "safe area" bounds
	///		that are set by the respective platforms.  Defaults to false.
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.dynamicUnit(...,components: new() { new app(...), new user(...), etc.});
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	///		WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.dynamicUnit(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId1", APSAdFormat.Display320X50),
	///					new apsAd("slotId2", APSAdFormat.InterstitialVideo)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		InlineAd object that correlates to the Requested Ad
	/// </returns>
	public static InlineAd dynamicUnit(
		string position, 
		int xCoord,
		int yCoord,
		Format[] addFormats = null, 
		AdOrientation orientation = AdOrientation.deviceOrientation,
		Position adPosition = Position.unknown, 
		float bidFloor = 0f, 
		int refreshInterval = 0, 
		int width = 0, 
		int height = 0,
		bool respectSafeArea = false, 
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new InlineAd(NimbusManager.Instance.NimbusEvents, position, addFormats: addFormats, adPosition:adPosition, bidFloor:bidFloor, 
			refreshInterval:refreshInterval, dynamicUnitWidth:width, dynamicUnitHeight:height, xCoord:xCoord, yCoord:yCoord, 
			respectSafeArea:respectSafeArea, orientation:orientation, dynamicUnit: true, components: components, demand: demand);
	}

	/// <summary>
	///		This function creates a fullscreen ad
	///		Fullscreen ad covers the entire screen and has no creatives by default. Similar to
	///		`Nimbus.inlineAd`, this function offers most freedom, but it should only be used if
	///		more specific functions (e.g. `Nimbus.interstitialAd()`) do not meet your needs.
	///		Reminder: Load() should only be called if the ad needs to be cached beforehand.  Show() does not
	///		need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="orientation">
	///		Preferred orientation of the ad, default is the current device orientation
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.fullscreenAd(...,components: new() { new app(...), new user(...), etc.});
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	/// 	WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.fullscreenAd(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId1", APSAdFormat.InterstitialDisplay),
	///					new apsAd("slotId2", APSAdFormat.InterstitialVideo)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		FullscreenAd that correlates to the Requested Ad
	/// </returns>
	public static FullscreenAd fullscreenAd(
		string position, 
		AdOrientation orientation = AdOrientation.deviceOrientation,
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new FullscreenAd(NimbusManager.Instance.NimbusEvents, position, orientation:  orientation, interstitial:false,
			components: components, demand: demand);
	}

	/// <summary>
	///		This function creates an interstitial ad.
	///		Interstitial ad covers the entire screen and includes banner and video creative by default.
	///		Other types and configuration may be done in request builder the closure.
	/// 	Reminder: Load() should only be called if the ad needs to be cached beforehand.
	///		Show() does not need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="addFormats">
	///		Set of additional formats
	/// </param>
	/// <param name="orientation">
	///		Preferred orientation of the ad, default is the current device orientation
	/// </param>
	/// <param name="bidFloor">
	///		Minimum bid for this ad impression expressed in CPM
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.interstitialAd(...,components: new() { new app(...), new user(...), etc.}
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	/// 	WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.interstitialAd(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId1", APSAdFormat.InterstitialDisplay),
	///					new apsAd("slotId2", APSAdFormat.InterstitialVideo)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		FullscreenAd that correlates to the Requested Ad
	/// </returns>
	public static FullscreenAd interstitialAd(
		string position, 
		Format[] addFormats = null, 
		AdOrientation orientation = AdOrientation.deviceOrientation, 
		float bidFloor = 0f,
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new FullscreenAd(NimbusManager.Instance.NimbusEvents, position, addFormats,
			orientation, bidFloor, true, components, demand);
	}

	/// <summary>
	///		This function creates a rewarded ad.
	///		Rewarded ad covers the entire screen and includes video creative by default.
	///		Rewarded ad can be closed only after full playback.
	///		Other types and configuration may be done in request builder the closure.
	/// 	Reminder: Load() should only be called if the ad needs to be cached beforehand.
	///		Show() does not need Load() to be called first.
	/// </summary>
	/// <param name="position">
	///     Position / identifier of the ad
	/// </param>
	/// <param name="orientation">
	///		Preferred orientation of the ad, default is the current device orientation
	/// </param>
	/// <param name="bidFloor">
	///		Minimum bid for this ad impression expressed in CPM
	/// </param>
	/// <param name="components">
	///		Parameter that allows the publisher to add modifiers on a per-request basis
	///     example:
	///			var ad = Nimbus.rewardedAd(...,components: new() { new app(...), new user(...), etc.}
	/// </param>
	/// <param name="demand">
	///		Parameter that allows the publisher to add demand elements (APS/AdMob) to the request
	///		WARNING: Using this will override the demand configuration set in the Unity editor
	///     example:
	///			var ad = Nimbus.rewardedAd(...,demand: new() {
	///				new admob("adUnitId"),
	///				new aps(new() {
	///					new apsAd("slotId2", APSAdFormat.RewardedVideo)
	///				})
	///			});
	/// </param>
	/// <returns>
	///		RewardedAd that correlates to the Requested Ad
	/// </returns>
	public static RewardedAd rewardedAd(
		string position, 
		AdOrientation orientation = AdOrientation.deviceOrientation, 
		float bidFloor = 0f, 
		List<RequestComponent> components = null, 
		List<DemandComponent> demand = null)
	{
		return new RewardedAd(NimbusManager.Instance.NimbusEvents, position, orientation, bidFloor, components, demand);
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

		/// <summary>
		///		To attach an extended ID to Nimbus bid requests,
		///		provide the identity provider's domain (source) and one or more ID values.
		///		These will be included in the eids field of outgoing OpenRTB requests.
		/// <param name="source">
		///		Identity provider's domain
		/// </param>
		/// <param name="ids">
		///		Array of one or more ID values to be included in eids field of OpenRTB requests
		/// </param>
		/// </summary>
		public static class identity
		{
			public static void add(string source, UID[] ids)
			{
				ConfigHelpers.addExtendedIds(source, ids);
			}
			
			/// <summary>
			///		Clear Extended Ids
			/// </summary>
			/// <param name="source">
			///		If included, all extended Ids from source will be cleared.
			///		If not included, extended ids from all sources will be cleared
			/// </param>
			public static void clear(string source = "")
			{
				ConfigHelpers.clearExtendedIds(source);
			}
		}

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
    }