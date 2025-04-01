using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds {
	#if UNITY_IOS && NIMBUS_ENABLE_MOBILEFUSE
	internal class MobileFuseIOS : IInterceptor, IProvider {
		
		[DllImport("__Internal")]
		private static extern void _initializeMobileFuse();
        
		[DllImport("__Internal")]
		private static extern string _fetchMobileFuseToken();

		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			var mobileFuseObject = JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject;
			bidRequest.User.Ext.MobileFuseBuyerData = mobileFuseObject;
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			var biddingToken = _fetchMobileFuseToken();
			Debug.unityLogger.Log("MobileFuse Token", biddingToken);
			return biddingToken;
		}

		public void InitializeNativeSDK() {
			_initializeMobileFuse();
		}

	}
#endif
}