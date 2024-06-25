using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Request;

[assembly: InternalsVisibleTo("nimbus.test")]

namespace Nimbus.Internal.Interceptor {
	public class SkAdNetworkIOS : IInterceptor {
		private readonly Skadn _skAdNetwork;

		public SkAdNetworkIOS(string rawPlistJson) {
			if (rawPlistJson.IsNullOrEmpty()) return;
			
			var plistData = JsonConvert.DeserializeObject<Root>(rawPlistJson);
			if (plistData.SKAdNetworkItems == null) return;
			
			var networkIds = new List<string>();
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var item in plistData.SKAdNetworkItems) {
				networkIds.Add(item.SKAdNetworkIdentifier);
			}
			
			if (networkIds.Count == 0) return;
			_skAdNetwork = new Skadn {
				SkadnetIds = networkIds.ToArray(),
				Version = "2.0"
			};
		}


		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
			return "";
		}

		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (_skAdNetwork == null || _skAdNetwork.SkadnetIds.Length == 0) {
				return bidRequest;
			}

			if (bidRequest.Imp.IsNullOrEmpty()) return bidRequest;

			if (bidRequest.Imp[0].Ext == null) {
				bidRequest.Imp[0].Ext = new ImpExt {
					Skadn = _skAdNetwork
				};
				return bidRequest;
			}

			bidRequest.Imp[0].Ext.Skadn = _skAdNetwork;
			return bidRequest;
		}
	}
	
    public struct Root {
	    [JsonProperty("SKAdNetworkItems")]
        public List<SkAdNetworkIdentifier> SKAdNetworkItems;
    }

    public struct SkAdNetworkIdentifier {
        [JsonProperty("SKAdNetworkIdentifier")]
        public string SKAdNetworkIdentifier;
    }
}