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
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds {
	#if UNITY_IOS && NIMBUS_ENABLE_UNITY_ADS
	internal class UnityAdsIOS : IInterceptor, IProvider {
		private readonly string _gameID;
		
		[DllImport("__Internal")]
		private static extern void _initializeUnityAds(string gameId);
        
		[DllImport("__Internal")]
		private static extern string _fetchUnityAdsToken();

		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("unity_buyeruid", data);
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			var biddingToken = _fetchUnityAdsToken();
			Debug.unityLogger.Log("Unity Token", biddingToken);
			return biddingToken;
		}

		public UnityAdsIOS(string gameID) {
			_gameID = gameID;
		}

		public void InitializeNativeSDK() {
			_initializeUnityAds(_gameID);
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
					Debug.unityLogger.Log("Unity Ads ERROR", e.Message);
					return null;
				}
			});
		}
	}
#endif
}