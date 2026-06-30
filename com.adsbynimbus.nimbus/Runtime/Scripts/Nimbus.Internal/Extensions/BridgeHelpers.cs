using System;
using JetBrains.Annotations;
using Nimbus.Internal.Extensions.APS;
using UnityEngine;

namespace Nimbus.Internal.Extensions
{
    public class BridgeHelpers
    {
#if UNITY_ANDROID
        public static string GetStringFromJavaFuture(String className, String methodName, object[] methodParams,
            long timeout)
        {
            AndroidJNI.AttachCurrentThread();
            var timeUnit = new AndroidJavaClass("java.util.concurrent.TimeUnit");
            var timeUnitMillis = timeUnit.CallStatic<AndroidJavaObject>("valueOf", "MILLISECONDS");
            var unityHelper = new AndroidJavaClass(className);
            var future = unityHelper.CallStatic<AndroidJavaObject>(methodName, methodParams);
            return future.Call<String>("get", timeout, timeUnitMillis);
        }
#endif
    }

#if UNITY_IOS
    public class Extensions
    {
        public Aps aps;
        public Admob adMob;
        public InMobi inMobi;
        public Meta meta;
        public Mintegral mintegral;
        public MobileFuse mobileFuse;
        public Moloco moloco;
        public UnityAds unityAds;
        public Vungle vungle;
    }
    public struct Aps {
        public String appKey;
        public ApsSlotData[] slotData;
    }
    public struct Admob
    {
        public String[] adUnitIds;
    }
    
    public struct InMobi
    {
        public String accountId;
    }
    
    public struct Meta
    {
        public String appId;
        public Boolean forceTestAd;
    }
    
    public struct Mintegral
    {
        public String appId;
        public String appKey;
    }
    
    public struct MobileFuse {
    }
    
    public struct Moloco
    {
        public String appKey;
    }
    
    public struct UnityAds
    {
        public String gameId;
    }
    
    public struct Vungle
    {
        public String appId;
    }
#endif
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
    ///     Adds per-request app categories to the RTB request.
    /// </summary>
    public struct PerRequestApp
    {
        public String[] pageCat; // The RTB pagecat value to apply for this request. This set is written to the RTB App object as page categories.
        public String[] sectionCat; // The RTB sectioncat value to apply for this request. This set is written to the RTB App object as section categories.

        public PerRequestApp(string[] pageCat, string[] sectionCat)
        {
            this.pageCat = pageCat;
            this.sectionCat = sectionCat;
        }
    }

    /// <summary>
    ///     A banner creative to be attached to the ad request.
    /// </summary>
    public struct BannerCreative
    {
        public int? width; //  If omitted, size will be determined by context
        public int? height; //  If omitted, size will be determined by context
        [CanBeNull] public RTBFormat[] addFormats; // Set of additional formats
        public RTBPosition? adPosition; // Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        [CanBeNull] public RTBCreativeAttribute[] battr; // Set of blocked attributes

        public BannerCreative(int? width = null, int? height = null, [CanBeNull] RTBFormat[] addFormats = null, 
            RTBPosition? adPosition = null, float? bidFloor = null, [CanBeNull] RTBCreativeAttribute[] battr = null)
        {
            this.width = width;
            this.height = height;
            this.addFormats = addFormats;
            this.adPosition = adPosition;
            this.bidFloor = bidFloor;
            this.battr = battr;
        }
    }

    /// <summary>
    ///     Supported ad format with a width and height
    /// </summary>
    public enum RTBFormat: byte
    {
        banner = 0, //Standard banner format (320×50).
        halfScreen = 1, // Half-screen format (300×600).
        interstitial = 2, // Interstitial format chosen for the current device orientation.
        interstitialLandscape = 3, // Interstitial landscape format (480×320).
        interstitialPortrait = 4, // Interstitial portrait format (320×480).
        leaderboard = 5, // Leaderboard format (728×90).
        mrec = 6 // Medium rectangle (MREC) format (300×250).
    }

    /// <summary>
    ///     Describes the position of the ad as a relative measure of visibility or prominence.
    ///     This OpenRTB table has values derived from the Inventory Quality Guidelines (IQG). Values 4 - 7 apply to apps.
    ///     OpenRTB Section 5.4
    /// </summary>
    public enum RTBPosition: byte
    {
        aboveTheFold = 0,
        belowTheFold = 1,
        footer = 2,
        fullScreen = 3,
        header = 4,
        sidebar = 5,
        unknown = 6
    }

    /// <summary>
    ///     Standard list of creative attributes that can describe an ad being served or serve as restrictions
    ///     of thereof OpenRTB Section 5.3
    /// </summary>
    public enum RTBCreativeAttribute: byte
    {
        adProvidesSkipButton = 0, //Ad Provides Skip Button (e.g. VPAID-rendered skip button on pre-roll video)
        adobeFlash = 1,
        audioAdAutoPlay = 2,
        audioAdUserInitiated = 3,
        expandableAutomatic = 4,
        expandableUserInitiatedRollover = 5,
        hasAudioOnOffButton = 6,
        hasPopup = 7, //Pop (e.g., Over, Under, or Upon Exit)
        inBannerVideoAdAutoPlay = 8,
        inBannerVideoAdUserInitiated = 9,
        provocativeOrSuggestiveImagery = 10,
        shakyFlashingFlickeringExtremeAnimationSmileys = 11,
        surveys = 12,
        textOnly = 13,
        userInteractive = 14, //User Interactive (e.g., Embedded Games)
        windowsdialogOrAlertStyle = 15
    }

    /// <summary>
    ///     Overrides the environment for a single ad.
    /// </summary>
    public struct Env
    {
        public string publisherKey; // Publisher key to be used for this ad
        public string apiKey; // API Key to be used for this ad

        public Env(string publisherKey, string apiKey)
        {
            this.publisherKey = publisherKey;
            this.apiKey = apiKey;
        }
    }

    /// <summary>
    ///     Adds device geolocation to the RTB request.
    /// </summary>
    public struct Location
    {
        public double latitude; // The latitude in decimal degrees. Valid range is -90...90.
        public double longitude; // The longitude in decimal degrees. Valid range is -180...180.
        public LocationType locationType; // Source of location data, e.g. GPS
        public int? accuracy; // The estimated horizontal accuracy radius in meters. Pass nil to omit. Values less than or equal to 0 are treated as unknown and omitted.

        public Location(double latitude, double longitude, LocationType locationType, int? accuracy = null)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.locationType = locationType;
            this.accuracy = accuracy;
        }
    }

    /// <summary>
    ///     Attaches a video creative to the ad request.
    /// </summary>
    public struct VideoCreative
    {
        public RTBPosition? adPosition; //Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        public int? minDuration; // Minimum video ad duration in seconds
        public int? maxDuration; // Maximum video ad duration in seconds
        public int? width; // Width of the video player in points
        public int? height; // Height of the video player in points
        public RTBVideoPlacementType? placementType; // Placement type for this video impression
        [CanBeNull] public RTBPlaybackMethod[] playbackMethod; // Playback methods that may be in use. If none are specified, any method may be used

        public VideoCreative(RTBPosition adPosition = RTBPosition.unknown, float? bidFloor = null, int? minDuration = null, 
            int? maxDuration = null, int? width = null, int? height = null, RTBVideoPlacementType? placementType = null, [CanBeNull] RTBPlaybackMethod[] playbackMethod = null)
        {
            this.adPosition = adPosition;
            this.bidFloor = bidFloor;
            this.minDuration = minDuration;
            this.maxDuration = maxDuration;
            this.width = width;
            this.height = height;
            this.placementType = placementType;
            this.playbackMethod = playbackMethod;
        }
    }
    
    /// <summary>
    /// Placements for the video (vast) ad. OpenRTB Section 5.9
    /// </summary>
    public enum RTBVideoPlacementType: byte
    {
        // Loads and plays dynamically between paragraphs of editorial content; existing as a standalone branded message
        inArticle = 0,
        /*
         * Exists within a web banner that leverages the banner space to deliver a video experience as opposed
         * to another static or rich media format. The format relies on the existence of display ad inventory on the page for its delivery
         */
        inBanner = 1,
        // Found in content, social, or product feeds
        inFeed = 2,
        /*
         * Played before, during or after the streaming video content that the consumer has requested (e.g., Pre-roll, Mid-roll, Post-roll)
         */
        inStream = 3,
        /*
         * Covers the entire or a portion of screen area, but is always on screen while displayed (i.e. cannot be scrolled out of view).
         * Note that a full-screen interstitial (e.g., in mobile) can be distinguished from a floating/slider unit by RTB.Impression.isInterstitial
         */
        interstitialSliderFloating = 4
    }
    
    /// <summary>
    /// Video playback method. OpenRTB Section 5.10
    /// </summary>
    public enum RTBPlaybackMethod: byte
    {
        clickWithSoundOn = 0, //Initiates on Click with Sound On
        enteringViewportWithSoundOffByDefault = 1, // Initiates on Entering Viewport with Sound Off by Default
        enteringViewportWithSoundOn = 2, //Initiates on Entering Viewport with Sound On
        mouseOverWithSoundOn = 3, //Initiates on Mouse-Over with Sound On
        pageLoadWithSoundOffByDefault = 4, // Initiates on Page Load with Sound Off by Default
        pageLoadWithSoundOn = 5, // Initiates on Page Load with Sound On
    }

    /// <summary>
    /// Adds viewability information to the RTB request.
    /// </summary>
    public struct Viewability
    {
        public string omidPn; //The viewability measurement partner identifier (for example, the vendor or SDK name).
        public string omidPv; //The viewability SDK version string associated with partner.

        public Viewability(string omidPn, string omidPv)
        {
            this.omidPn = omidPn;
            this.omidPv = omidPv;
        }
    }
#if UNITY_ANDROID
    public class Extensions
    {
        public String apsAppKey;
        public ApsSlotData[] apsSlotData;
        public String[] adMobAdUnitIds;
        public String inMobiAccountId;
        public String metaAppId;
        public Boolean metaForceTestAd;
        public String mintegralAppId;
        public String mintegralAppKey;
        public String molocoAppKey;
        public String unityAdsGameId;
        public String vungleAppId;
        public String[] appPageCat;
        public String[] appSectionCat;
        public String userKeywords;
        public String viewabilityOmidPn;
        public String viewabilityOmidPv;
        public double latitude;
        public double longitude;
        public LocationType locationType;
        public int accuracy;
    }
#endif
    public enum LocationType : byte {
        gps = 0,
        ipLookup = 1,
        userProvided = 2
    }

}