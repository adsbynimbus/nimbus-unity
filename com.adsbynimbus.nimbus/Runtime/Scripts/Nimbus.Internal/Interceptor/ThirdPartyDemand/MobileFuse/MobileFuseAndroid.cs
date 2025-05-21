using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse {
	internal class MobileFuseAndroid : IInterceptor, IProvider {
		private readonly string _gameID;
		private readonly bool _testMode;
		
		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.complexUserExt = 
				new KeyValuePair<string,JObject>("mfx_buyerdata", JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject);
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			var token = "";
			try
			{
				token = BridgeHelpers.GetStringFromJavaFuture(
					"com.adsbynimbus.request.internal.NimbusRequestsMobileFuseInternal",
					"token", new object[]{},  500L);
				Debug.unityLogger.Log("MobileFuse Token", token);
			}
			catch (Exception e)
			{
				Debug.unityLogger.Log("Unable to retrieve MobileFuse Token", e.Message);
			}
			return token;
		}
		public void InitializeNativeSDK() {
			// No initialization needed
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
					Debug.unityLogger.Log("MobileFuse ERROR", e.Message);
					return null;
				}
			});
		}
	}
}