using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse {
	#if UNITY_IOS && NIMBUS_ENABLE_MOBILEFUSE
	internal class MobileFuseIOS : IInterceptor, IProvider {
		
		[DllImport("__Internal")]
		private static extern void _initializeMobileFuse();
        
		[DllImport("__Internal")]
		private static extern string _fetchMobileFuseToken();

		internal BidRequestDelta GetBidRequestDelta(string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.ComplexUserExt = 
				new KeyValuePair<string,JObject>("mfx_buyerdata", JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject);
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK()
		{
			var biddingToken = _fetchMobileFuseToken();
			Debug.unityLogger.Log("MobileFuse Token", biddingToken);
			return biddingToken;
		}

		public void InitializeNativeSDK() {
			_initializeMobileFuse();
		}
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetProviderRtbDataFromNativeSDK());
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("MobileFuse ERROR", e.Message);
					return null;
				}
			});
		}
	}
#endif
}