using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds {
	internal class UnityAdsAndroid : IInterceptor, IProvider {
		private const string NimbusUnityAdsPackage = "com.adsbynimbus.request.UnityDemandProvider";
		private const string UnityAdsPackage = "com.unity3d.ads.UnityAds";
		private readonly string _gameID;
		private readonly bool _testMode;
		private readonly AndroidJavaObject _applicationContext;
		
		private BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("unity_buyeruid", data);
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			AndroidJNI.AttachCurrentThread();
			var unityAds = new AndroidJavaClass(UnityAdsPackage);
			var token = unityAds.CallStatic<string>("getToken");
			if (token == null)
			{
				return "";
			}
			Debug.unityLogger.Log("Unity Ads Token", token);
			return token;
		}
		
		public UnityAdsAndroid(AndroidJavaObject applicationContext, bool testMode, string gameID) {
			_applicationContext = applicationContext;
			_testMode = testMode;
			_gameID = gameID;
		}
		
		public void InitializeNativeSDK() {
			var unityAds = new AndroidJavaClass(NimbusUnityAdsPackage);
			unityAds.CallStatic("initialize", _applicationContext, _gameID);
		}
		
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
			});
		}
	}
}