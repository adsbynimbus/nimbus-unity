using Newtonsoft.Json;

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
        public ApsResponse[] Aps {get; set;}
        
        [JsonProperty("position", Required = Required.Always)]
        public string Position { get; set; }

        [JsonProperty("skadn", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Skadn Skadn { get; set; }

        [JsonProperty("facebook_app_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FacebookAppId { get; set; }

        [JsonProperty("facebook_test_ad_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string MetaTestAdType { get; set; }
    }
    
    public class ApsResponse {
        [JsonProperty("amzn_b", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object AmznB;
        [JsonProperty("amzn_vid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object AmznVid;
        [JsonProperty("amzn_h", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object AmznH;
        [JsonProperty("amznp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Amznp;
        [JsonProperty("amznrdr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Amznrdr;
        [JsonProperty("amznslots", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Amznslots;
        [JsonProperty("dc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Dc;
    }
}
