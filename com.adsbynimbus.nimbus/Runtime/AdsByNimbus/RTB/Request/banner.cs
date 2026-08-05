using JetBrains.Annotations;

namespace AdsByNimbus.RTB.Request
{
    /// <summary>
    ///     A banner creative to be attached to the ad request.
    /// </summary>
    public struct banner: RequestComponent
    {
        public int? width; //  If omitted, size will be determined by context
        public int? height; //  If omitted, size will be determined by context
        [CanBeNull] public Format[] addFormats; // Set of additional formats
        public Position? adPosition; // Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        [CanBeNull] public CreativeAttribute[] battr; // Set of blocked attributes

        public banner(int? width = null, int? height = null, [CanBeNull] Format[] addFormats = null, 
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
}