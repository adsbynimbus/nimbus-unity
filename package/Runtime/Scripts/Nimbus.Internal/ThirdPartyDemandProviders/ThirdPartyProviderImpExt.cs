using OpenRTB.Request;
using Newtonsoft.Json;

namespace Nimbus.Internal.ThirdPartyDemandProviders {
	public class ThirdPartyProviderImpExt : ImpExt {
		[JsonProperty("aps", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public string Aps;
	}
}