using Newtonsoft.Json;

namespace OpenRTB.Request {
    public class User {
        [JsonProperty("age", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Age;

        [JsonProperty("buyeruid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string BuyerId;

        [JsonProperty("custom_data", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CustomData;

        [JsonProperty("data", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Data Data;

        [JsonProperty("ext", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public UserExt Ext;

        [JsonProperty("gender", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Gender;

        [JsonProperty("keywords", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Keywords;

        [JsonProperty("yob", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Yob;
    }

    public class UserExt {
        [JsonProperty("consent", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Consent;

        [JsonProperty("eids", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Eid[] Eids;
        
        [JsonProperty("vungle_buyeruid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string VungleBuyerId;

        [JsonProperty("facebook_buyeruid", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FacebookBuyerId;
        
        [JsonProperty("admob_gde_signals", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AdMobSignals;
        
    }
}
