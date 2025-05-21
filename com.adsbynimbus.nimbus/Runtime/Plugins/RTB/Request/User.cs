using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public JObject Ext;

        [JsonProperty("gender", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Gender;

        [JsonProperty("keywords", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Keywords;

        [JsonProperty("yob", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Yob;
    }
}
