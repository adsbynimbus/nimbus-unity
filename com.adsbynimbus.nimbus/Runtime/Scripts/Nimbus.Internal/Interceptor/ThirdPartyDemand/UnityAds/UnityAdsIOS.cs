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
		private readonly bool _testMode;
		
		[DllImport("__Internal")]
		private static extern void _initializeUnityAds();
        
		[DllImport("__Internal")]
		private static extern string _fetchMetaBiddingToken();

		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			bidRequest.User.Ext.FacebookBuyerId = data;
			if (bidRequest.Imp.Length > 0) {
				bidRequest.Imp[0].Ext.FacebookAppId = _appID;
				if (_testMode) {
					bidRequest.Imp[0].Ext.MetaTestAdType = "IMG_16_9_LINK";
				}
			}

			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			var biddingToken = _fetchMetaBiddingToken();
			Debug.unityLogger.Log("METABIDDINGTOKEN", biddingToken);
			return biddingToken;
		}

		public MetaIOS(string appID, bool enableTestMode) {
			_appID = appID;
			_testMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_initializeUnityAds();
		}

	}
#endif
}