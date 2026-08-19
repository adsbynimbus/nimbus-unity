using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AdsByNimbus.RTB.Request
{
    /// <summary>
    ///     A banner creative to be attached to the ad request.
    /// </summary>
    public struct banner: RequestComponent
    {
        public int width; //  If omitted, size will be determined by context
        public int height; //  If omitted, size will be determined by context
        public Format[] addFormats; // Set of additional formats
        public Position? adPosition; // Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        [CanBeNull] public CreativeAttribute[] battr; // Set of blocked attributes

        public banner(int width = 0, int height = 0, [CanBeNull] Format[] addFormats = null, 
            Position? adPosition = null, float? bidFloor = null, [CanBeNull] CreativeAttribute[] battr = null)
        {
            this.width = width;
            this.height = height;
            if (addFormats == null)
            {
                this.addFormats = Array.Empty<Format>();
            }
            else
            {
                this.addFormats = addFormats;
            }
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
        audioAdUserInitiated,

        // Expandable (Automatic)
        expandableAutomatic,

        // Expandable (User Initiated - Click)
        expandableUserInitiatedClick,

        // Expandable (User Initiated - Rollover)
        expandableUserInitiatedRollover,

        // In-Banner Video Ad (Auto-Play)
        inBannerVideoAdAutoPlay,
    
        // In-Banner Video Ad (User Initiated)
        inBannerVideoAdUserInitiated,
    
        // Pop (e.g., Over, Under, or Upon Exit)
        hasPopup,
    
        //Provocative or Suggestive Imagery
        provocativeOrSuggestiveImagery,
    
        // Shaky, Flashing, Flickering, Extreme Animation, Smileys
        shakyFlashingFlickeringExtremeAnimationSmileys,
    
        // Surveys
        surveys,
    
        // Text Only
        textOnly,
    
        // User Interactive (e.g., Embedded Games)
        userInteractive,
    
        // Windows Dialog or Alert Style
        windowsDialogOrAlertStyle,
    
        // Has Audio On/Off Button
        hasAudioOnOffButton,
    
        // Ad Provides Skip Button (e.g. VPAID-rendered skip button on pre-roll video)
        adProvidesSkipButton,
    
        // Adobe Flash
        adobeFlash
    }
}