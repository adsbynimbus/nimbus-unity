using System.Collections.Generic;
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