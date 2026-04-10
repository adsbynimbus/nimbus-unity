using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using Unity.Plastic.Newtonsoft.Json.Linq;
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

		public JObject GetConfigObject()
		{
			var jObject = new JObject();
			jObject["demand"] = "Vungle";
			jObject["appId"] = _appID;
			return jObject;
		}

		internal BidRequestDelta GetBidRequestDelta(string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.SimpleUserExt = new KeyValuePair<string, string>("vungle_buyeruid", data);
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK()
		{
			var buyerId = _fetchVungleBuyerId();
			return buyerId;
		}
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetProviderRtbDataFromNativeSDK());
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