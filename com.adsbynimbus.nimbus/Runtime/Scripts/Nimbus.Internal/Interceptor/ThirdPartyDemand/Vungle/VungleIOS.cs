using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			bidRequest.User ??= new User();
			bidRequest.User.Ext ??= new UserExt();
			bidRequest.User.Ext.VungleBuyerId = data;

			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen, int width=0, int height=0)
		{
			var buyerId = _fetchVungleBuyerId();
			Debug.unityLogger.Log("VUNGLEBUYER", buyerId);
			return buyerId;
		}

	}
#endif
}