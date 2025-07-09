using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenRTB.Request {
    public class Data {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Id;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name;

        [JsonProperty("segment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Segment[] Segment;
    }
    
    public enum Gender
    {
        F, //female
        M, //male
        O //other
    }
}
