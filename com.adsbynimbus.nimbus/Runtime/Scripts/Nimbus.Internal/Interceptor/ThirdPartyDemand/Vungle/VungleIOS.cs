using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle {
	#if UNITY_IOS && NIMBUS_ENABLE_VUNGLE
	internal class VungleIOS : IInterceptor, IProvider {
		private readonly string _appID;
		
		[DllImport("__Internal")]
		private static extern void _initializeVungle(string appKey);

		[DllImport("__Internal")]
		private static extern string _fetchVungleBuyerId();

		public VungleIOS(string appID) {
			_appID = appID;
		}

		public void InitializeNativeSDK() {
			_initializeVungle(_appID);
		}

		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("vungle_buyeruid", data);
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			var buyerId = _fetchVungleBuyerId();
			Debug.unityLogger.Log("VUNGLEBUYER", buyerId);
			return buyerId;
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
					Debug.unityLogger.Log("Vungle ERROR", e.Message);
					return null;
				}
			});
		}
	}
#endif
}