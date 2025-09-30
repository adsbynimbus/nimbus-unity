using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenRTB.Request {
    public class Imp {
        [JsonProperty("banner", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Banner Banner;

        [JsonProperty("ext", Required = Required.Always)]
        public ImpExt Ext;

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id;

        [JsonProperty("instl")] public int Instl;

        [JsonProperty("secure", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Secure;

        [JsonProperty("video", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Video Video;
    }

    public class ImpExt {
        
        [JsonProperty("aps", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JArray Aps {get; set;}
        
        [JsonProperty("position", Required = Required.Always)]
        public string Position { get; set; }

        [JsonProperty("skadn", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Skadn Skadn { get; set; }

        [JsonProperty("facebook_app_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FacebookAppId { get; set; }

        [JsonProperty("facebook_test_ad_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string MetaTestAdType { get; set; }

        [JsonProperty("adunit", Required = Required.Always)]
        public ImpExtAdUnitType AdUnitType { get; set; } = ImpExtAdUnitType.Unknown;
    }

    public enum ImpExtAdUnitType
    {
        // AdUnitTypeUnknown represents an unknown ad unit type.
        Unknown = 0,

        // AdUnitTypeInline represents banner ads typically sized at or below 300x250.
        // Common dimensions include 300x250, 300x50, 320x50, 320x100, and 728x90.
        // It May also include video ads when the video.placementType is set to "inline".
        Inline = 1,

        // AdUnitTypeInterstitial represents full-screen, blocking ads, typically sized at 320x480.
        Interstitial = 2,

        // AdUnitTypeRewarded represents ads that grant a reward to users upon completion.
        // While often video-based, the format is not limited to video.
        // For video ads, users are typically required to watch to completion; behavior for other formats is still to be defined.
        Rewarded = 3,

        // AdUnitTypeNative represents native ads, where the ad UI is constructed client-side by the publisher.
        // These ads are typically non-blocking and styled to match the app’s content.
        // Currently used as a placeholder, as official support may vary.
        Native = 4,
	
        // AdUnitTypeNimbusDynamic represents Nimbus’s proprietary dynamic ad unit.
        // It simultaneously requests RTB Banner, RTB Video, and occasionally RTB Native creatives.
        // This unit is typically displayed inline, which distinguishes it from interstitial units.
        NimbusDynamic = 5
    }
}
