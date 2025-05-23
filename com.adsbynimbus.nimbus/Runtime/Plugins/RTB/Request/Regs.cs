using Newtonsoft.Json;

namespace OpenRTB.Request {
    public class Regs {
        [JsonProperty("coppa", DefaultValueHandling = DefaultValueHandling.Ignore)] public int Coppa;

        [JsonProperty("ext", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public RegExt Ext;
    }

    public class RegExt {
        [JsonProperty("gdpr", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Gdpr;

        [JsonProperty("us_privacy", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string UsPrivacy;

        [JsonProperty("gpp", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GPP;

        [JsonProperty("gpp_sid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string GPPSIDs;
    }
}
