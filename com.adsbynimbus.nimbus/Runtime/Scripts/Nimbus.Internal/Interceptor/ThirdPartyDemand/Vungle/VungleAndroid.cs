using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
		
		public VungleAndroid(AndroidJavaObject applicationContext, string appID) {
			_applicationContext = applicationContext;
			_appID = appID;
		}
		
		internal BidRequestDelta GetBidRequestDelta(string data) {
			AndroidJNI.AttachCurrentThread();
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.SimpleUserExt = new KeyValuePair<string, string>("vungle_buyeruid", data);
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK()
		{
			AndroidJNI.AttachCurrentThread();
			var vungle = new AndroidJavaClass(VunglePackage);
			var buyerId = vungle.CallStatic<string>("getBiddingToken", _applicationContext);
			return buyerId;
		}
		
		public void InitializeNativeSDK() {
			var nimbusVungle = new AndroidJavaClass(NimbusVunglePackage);
			nimbusVungle.CallStatic("initialize", _appID);
			var vungle = new AndroidJavaClass(VunglePackage);
			var vungleWrapperFramework = new AndroidJavaClass("com.vungle.ads.VungleWrapperFramework");
			var hbs = vungleWrapperFramework.GetStatic<AndroidJavaObject>("vunglehbs");
			vungle.CallStatic("setIntegrationName", hbs, "29");
			
		}
		
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
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
}