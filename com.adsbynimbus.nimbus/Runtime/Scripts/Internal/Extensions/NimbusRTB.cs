using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Internal.Extensions
{
    
    /// <summary>
    /// This object contains information known or derived about the human user of the device (i.e., the audience for advertising).
    /// The user id is an exchange artifact and may be subject to rotation or other privacy policies.
    /// However, this user ID must be stable long enough to serve reasonably as the basis for frequency capping and retargeting.
    /// OpenRTB Section 3.2.20
    /// </summary>
    public class User
    {
        // The age of the user
        public int? age; 
        /*
         * Optional feature to pass bidder data that was set in the exchange’s cookie.
         * The string must be in base85 cookie safe characters and be in any format. Proper JSON encoding must be used to include “escaped” quotation marks
         */
        #if UNITY_ANDROID
        [JsonProperty("custom_data")]
        #endif
        [CanBeNull] public string customData; 
        // The gender of the user

        public Gender? gender;
        // Comma separated list of keywords, interests, or intent
        [CanBeNull] public string keywords;

        public User(int? age = null, [CanBeNull] string customData = null, 
            Gender? gender = null, [CanBeNull] string keywords = null)
        {
            this.age = age;
            this.customData = customData;
            this.gender = gender;
            this.keywords = keywords;
        }
    }
    
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Gender
    {
        #if UNITY_IOS
        [EnumMember(Value = "male")]
        #else
        [EnumMember(Value = "M")]
        #endif
        male,
        #if UNITY_IOS
        [EnumMember(Value = "female")]
        #else
        [EnumMember(Value = "F")]
        #endif
        female,
        #if UNITY_IOS
        [EnumMember(Value = "other")]
        #else
        [EnumMember(Value = "O")]
        #endif
        other
        
    }
    
    internal class Segment
    {
        [CanBeNull] public string id;
        [CanBeNull] public string name;
        [CanBeNull] public string value;

        public Segment([CanBeNull] string id, [CanBeNull] string name, [CanBeNull] string value)
        {
            this.id = id;
            this.name = name;
            this.value = value;
        }
    }

    internal class Data
    {
        [CanBeNull] public string id;
        [CanBeNull] public string name;
        internal Segment[] segment;

        internal Data([CanBeNull] string id = null, [CanBeNull] string name = null)
        {
            this.id = id;
            this.name = name;
        }
    }

    /// <summary>
    /// This object should be included if the ad supported content is a non-browser application (typically in mobile) as opposed to a website.
    /// OpenRTB Section 3.2.14
    /// </summary>
    public class App
    {
        [CanBeNull] public string bundle; // A platform-specific application identifier intended to be unique to the app and independent of the exchange. On iOS, it is typically a numeric ID. Default: nil
        public string[] cat; // IAB content categories of the app OpenRTB Section 5.1
        [CanBeNull] public string domain; // Domain of the app (e.g., “adsbynimbus.com”). Default: nil
        [CanBeNull] public string name; // App name (may be aliased at the publisher’s request). Default: nil
        public string[] pagecat; // IAB content categories that describe the current page or view of the app. OpenRTB Section 5.1
        [JsonIgnore]
        public bool? paid; // Whether the app is paid or not
        [JsonProperty("paid")]
        internal byte? paidJson => paid == null ? null : paid.Value ? (byte)1 : (byte)0;
        [JsonIgnore]
        public bool? privacypolicy; // Indicates if the app has a privacy policy
        [JsonProperty("privacypolicy")]
        internal byte? privacyPolicyJson => privacypolicy == null ? null : privacypolicy.Value ? (byte)1 : (byte)0;
        [CanBeNull] public Publisher publisher; // Details about the publisher of the app
        public string[] sectioncat; // IAB content categories that describe the current section of the app. OpenRTB Section 5.1
        [CanBeNull] public string storeurl; // App store URL for an installed app; for IQG 2.1 compliance. Default: nil
        [CanBeNull] public string ver; // Application version

        public App([CanBeNull] string bundle = null, string[] cat = null, [CanBeNull] string domain = null, 
            [CanBeNull] string name = null, string[] pagecat = null, bool? paid = default, bool? privacypolicy = default, 
            [CanBeNull] Publisher publisher = null, string[] sectioncat = null, [CanBeNull] string storeurl = null, [CanBeNull] string ver = null)
        {
            this.bundle = bundle;
            this.cat = cat;
            this.domain = domain;
            this.name = name;
            this.pagecat = pagecat;
            this.paid = paid;
            this.privacypolicy = privacypolicy;
            this.publisher = publisher;
            this.sectioncat = sectioncat;
            this.storeurl = storeurl;
            this.ver = ver;
        }
    }

    /// <summary>
    /// This describes the publisher of the media in which the ad will be displayed. The publisher is typically the seller in an OpenRTB transaction.
    /// OpenRTB Section 3.2.15
    /// </summary>
    public class Publisher
    {
        public string[] cat; // IAB content categories that describe the publisher. OpenRTB Section 5.1 Default: nil
        public string domain; // Highest level domain of the publisher (e.g., “adsbynimbus.com”). Default: nil
        public string name; // Publisher name (may be aliased at the publisher’s request). Default: nil

        public Publisher(string[] cat, string domain, string name)
        {
            this.cat = cat;
            this.domain = domain;
            this.name = name;
        }
    }
    
    /// <summary>
    /// Placements for the video (vast) ad. OpenRTB Section 5.9
    /// </summary>
    public enum VideoPlacementType: byte
    {
        /*
         * Played before, during or after the streaming video content that the consumer has requested (e.g., Pre-roll, Mid-roll, Post-roll)
         */
        inStream = 1,
        
        /*
         * Exists within a web banner that leverages the banner space to deliver a video experience as opposed
         * to another static or rich media format. The format relies on the existence of display ad inventory on the page for its delivery
         */
        inBanner = 2,
        
        // Loads and plays dynamically between paragraphs of editorial content; existing as a standalone branded message
        inArticle = 3,

        // Found in content, social, or product feeds
        inFeed = 4,

        /*
         * Covers the entire or a portion of screen area, but is always on screen while displayed (i.e. cannot be scrolled out of view).
         * Note that a full-screen interstitial (e.g., in mobile) can be distinguished from a floating/slider unit by RTB.Impression.isInterstitial
         */
        interstitialSliderFloating = 5
    }
    
    /// <summary>
    /// Video playback method. OpenRTB Section 5.10
    /// </summary>
    public enum PlaybackMethod: byte
    {
        pageLoadWithSoundOn = 1, // Initiates on Page Load with Sound On
        pageLoadWithSoundOffByDefault = 2, // Initiates on Page Load with Sound Off by Default
        clickWithSoundOn = 3, //Initiates on Click with Sound On
        mouseOverWithSoundOn = 4, //Initiates on Mouse-Over with Sound On
        enteringViewportWithSoundOn = 5, //Initiates on Entering Viewport with Sound On
        enteringViewportWithSoundOffByDefault = 6, // Initiates on Entering Viewport with Sound Off by Default

    }
    
    
    /// <summary>
    ///     Supported ad format with a width and height
    /// </summary>
    public enum Format: byte
    {
        banner = 1, //Standard banner format (320×50).
        mrec = 2, // Medium rectangle (MREC) format (300×250).
        halfScreen = 3, // Half-screen format (300×600).
        leaderboard = 4, // Leaderboard format (728×90).
        interstitialPortrait = 5, // Interstitial portrait format (320×480).
        interstitialLandscape = 6, // Interstitial landscape format (480×320).
        interstitial = 7, // Interstitial format chosen for the current device orientation.
    }

    /// <summary>
    ///     Describes the position of the ad as a relative measure of visibility or prominence.
    ///     This OpenRTB table has values derived from the Inventory Quality Guidelines (IQG). Values 4 - 7 apply to apps.
    ///     OpenRTB Section 5.4
    /// </summary>
    public enum Position: byte
    {
        unknown = 0,
        aboveTheFold = 1,
        belowTheFold = 2,
        header = 3,
        footer = 4,
        sidebar = 5,
        fullScreen = 6,
    }

    /// <summary>
    ///     Standard list of creative attributes that can describe an ad being served or serve as restrictions
    ///     of thereof OpenRTB Section 5.3
    /// </summary>
    public enum CreativeAttribute: byte
    {
        // Audio Ad (Auto-Play)
        audioAdAutoPlay = 1,

        // Audio Ad (User Initiated)
        audioAdUserInitiated = 2,

        // Expandable (Automatic)
        expandableAutomatic = 3,

        // Expandable (User Initiated - Click)
        expandableUserInitiatedClick = 4,

        // Expandable (User Initiated - Rollover)
        expandableUserInitiatedRollover = 5,

        // In-Banner Video Ad (Auto-Play)
        inBannerVideoAdAutoPlay = 6,
        
        // In-Banner Video Ad (User Initiated)
        inBannerVideoAdUserInitiated = 7,
        
        // Pop (e.g., Over, Under, or Upon Exit)
        hasPopup = 8,
        
        //Provocative or Suggestive Imagery
        provocativeOrSuggestiveImagery = 9,
        
        // Shaky, Flashing, Flickering, Extreme Animation, Smileys
        shakyFlashingFlickeringExtremeAnimationSmileys = 10,
        
        // Surveys
        surveys = 11,
        
        // Text Only
        textOnly = 12,
        
        // User Interactive (e.g., Embedded Games)
        userInteractive = 13,
        
        // Windows Dialog or Alert Style
        windowsDialogOrAlertStyle = 14,
        
        // Has Audio On/Off Button
        hasAudioOnOffButton = 15,
        
        // Ad Provides Skip Button (e.g. VPAID-rendered skip button on pre-roll video)
        adProvidesSkipButton = 16,
        
        // Adobe Flash
        adobeFlash = 17
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
        [CanBeNull] public Format[] addFormats; // Set of additional formats
        public Position? adPosition; // Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        [CanBeNull] public CreativeAttribute[] battr; // Set of blocked attributes

        public BannerCreative(int? width = null, int? height = null, [CanBeNull] Format[] addFormats = null, 
            Position? adPosition = null, float? bidFloor = null, [CanBeNull] CreativeAttribute[] battr = null)
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
        public Position? adPosition; //Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        public int? minDuration; // Minimum video ad duration in seconds
        public int? maxDuration; // Maximum video ad duration in seconds
        public int? width; // Width of the video player in points
        public int? height; // Height of the video player in points
        public VideoPlacementType? placementType; // Placement type for this video impression
        [CanBeNull] public PlaybackMethod[] playbackMethod; // Playback methods that may be in use. If none are specified, any method may be used

        public VideoCreative(Position adPosition = Position.unknown, float? bidFloor = null, int? minDuration = null, 
            int? maxDuration = null, int? width = null, int? height = null, VideoPlacementType? placementType = null, [CanBeNull] PlaybackMethod[] playbackMethod = null)
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
    
    public enum LocationType : byte {
        gps = 1,
        ipLookup = 2,
        userProvided = 3
    }
}