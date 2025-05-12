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
	internal class VungleAndroid : IInterceptor, IProvider {
		private const string NimbusVunglePackage = "com.adsbynimbus.request.VungleDemandProvider";
		private const string VunglePackage = "com.vungle.ads.VungleAds";
		private readonly string _appID;
		private readonly AndroidJavaObject _applicationContext;

		public VungleAndroid(string appID) {
			_appID = appID;
		}
		
		public VungleAndroid(AndroidJavaObject applicationContext, string appID) {
			_applicationContext = applicationContext;
			_appID = appID;
		}
		
		private BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("vungle_buyeruid", data);
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			AndroidJNI.AttachCurrentThread();
			var vungle = new AndroidJavaClass(VunglePackage);
			var buyerId = vungle.CallStatic<string>("getBiddingToken", _applicationContext);
			return buyerId;
		}
		
		public void InitializeNativeSDK() {
			var vungle = new AndroidJavaClass(NimbusVunglePackage);
			vungle.CallStatic("initialize", _appID);
		}
		
		public async Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return await Task<BidRequestDelta>.Run(async () =>
			{
				return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
			});
		}
	}
}