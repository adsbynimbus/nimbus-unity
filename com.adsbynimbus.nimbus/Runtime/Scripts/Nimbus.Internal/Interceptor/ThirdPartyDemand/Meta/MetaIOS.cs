using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

		internal BidRequestDelta GetBidRequestDelta(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.SimpleUserExt = new KeyValuePair<string, string>("facebook_buyeruid", data);
			if (bidRequest.Imp.Length > 0)
			{
				var impExt = new ImpExt();
				impExt.FacebookAppId = _appID;
				if (_testMode)
				{
					impExt.MetaTestAdType = "IMG_16_9_LINK";
				}
				bidRequestDelta.ImpressionExtension = impExt;
			}
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
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
		
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("META ERROR", e.Message);
					return null;
				}
			});
		}

	}
#endif
}