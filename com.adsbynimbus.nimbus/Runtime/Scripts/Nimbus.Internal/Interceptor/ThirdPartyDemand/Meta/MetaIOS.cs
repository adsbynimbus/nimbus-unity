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
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	#if UNITY_IOS && NIMBUS_ENABLE_META
	internal class MetaIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _testMode;
		
		[DllImport("__Internal")]
		private static extern void _initializeMeta(string appKey);

		[DllImport("__Internal")]
		private static extern string _fetchMetaBiddingToken();

		private BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("facebook_buyeruid", data);
			if (bidRequest.Imp.Length > 0)
			{
				var impExt = new ImpExt();
				impExt.FacebookAppId = _appID;
				if (_testMode)
				{
					impExt.MetaTestAdType = "IMG_16_9_LINK";
				}
				bidRequestDelta.impressionExtension = impExt;
			}
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
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
		
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
			});
		}

	}
#endif
}