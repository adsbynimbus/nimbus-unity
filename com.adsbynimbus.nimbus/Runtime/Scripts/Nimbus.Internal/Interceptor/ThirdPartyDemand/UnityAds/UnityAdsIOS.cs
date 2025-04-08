using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			bidRequest.User ??= new User();
			bidRequest.User.Ext ??= new UserExt();
			bidRequest.User.Ext.UnityBuyerId = data;
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
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

	}
#endif
}