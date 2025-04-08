using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	#if UNITY_IOS && NIMBUS_ENABLE_META
	internal class MetaIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _testMode;
		
		[DllImport("__Internal")]
		private static extern void _initializeMeta(string appKey);

		[DllImport("__Internal")]
		private static extern string _fetchMetaBiddingToken();

		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			bidRequest.User ??= new User();
			bidRequest.User.Ext ??= new UserExt();
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
			_initializeMeta(_appID);
		}

	}
#endif
}