using System;
using JetBrains.Annotations;

namespace AdsByNimbus.RTB.Request
{
    /// <summary>
    ///     Attaches a video creative to the ad request.
    /// </summary>
    public struct video: RequestComponent
    {
        public Position? adPosition; //Ad position. Defaults to RTB.Position.unknown
        public float? bidFloor; // Minimum bid for this ad impression expressed in CPM
        public int? minDuration; // Minimum video ad duration in seconds
        public int? maxDuration; // Maximum video ad duration in seconds
        public int? width; // Width of the video player in points
        public int? height; // Height of the video player in points
        public VideoPlacementType? placementType; // Placement type for this video impression
        public PlaybackMethod[] playbackMethod; // Playback methods that may be in use. If none are specified, any method may be used

        public video(Position adPosition = Position.unknown, float? bidFloor = null, int? minDuration = null, 
            int? maxDuration = null, int? width = null, int? height = null, VideoPlacementType? placementType = null, 
            [CanBeNull] PlaybackMethod[] playbackMethod = null)
        {
            this.adPosition = adPosition;
            this.bidFloor = bidFloor;
            this.minDuration = minDuration;
            this.maxDuration = maxDuration;
            this.width = width;
            this.height = height;
            if (playbackMethod != null)
            {
                this.playbackMethod = playbackMethod;
            }
            else
            {
                this.playbackMethod = Array.Empty<PlaybackMethod>();
            }
            this.placementType = placementType;
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
}