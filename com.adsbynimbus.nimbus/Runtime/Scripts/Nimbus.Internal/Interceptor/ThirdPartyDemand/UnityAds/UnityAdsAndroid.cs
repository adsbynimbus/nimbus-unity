using System;
using System.Runtime.CompilerServices;
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
		
		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			bidRequest.User.Ext.UnityBuyerId = data;
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
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
	}
}