using OpenRTB.Request;
using Newtonsoft.Json;

namespace Nimbus.Internal.ThirdPartyDemandProviders {
	public class ThirdPartyProviderImpExt : ImpExt {
		[JsonProperty("aps", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public ApsResponse[] Aps;
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