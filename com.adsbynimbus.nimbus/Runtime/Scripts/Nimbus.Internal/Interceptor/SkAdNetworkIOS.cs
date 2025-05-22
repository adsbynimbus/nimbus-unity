using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

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

		public BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (_skAdNetwork == null || _skAdNetwork.SkadnetIds.Length == 0) {
				return bidRequestDelta;
			}

			if (bidRequest.Imp.IsNullOrEmpty()) return bidRequestDelta;

			if (bidRequest.Imp[0].Ext == null) {
				bidRequestDelta.impressionExtension = new ImpExt {
					Skadn = _skAdNetwork
				};
				return bidRequestDelta;
			}
			var impExt = new ImpExt();
			impExt.Skadn = _skAdNetwork;
			bidRequestDelta.impressionExtension = impExt;
			return bidRequestDelta;
		}
		
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("META ERROR", e.Message);
					return null;
				}
			});
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