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
		private readonly AndroidJavaObject _applicationContext;
		
		internal BidRequestDelta GetBidRequestDelta(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.SimpleUserExt = new KeyValuePair<string, string>("unity_buyeruid", data);
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
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
		
		public UnityAdsAndroid(AndroidJavaObject applicationContext, string gameID) {
			_applicationContext = applicationContext;
			_gameID = gameID;
		}
		
		public void InitializeNativeSDK() {
			var unityAds = new AndroidJavaClass(NimbusUnityAdsPackage);
			unityAds.CallStatic("initialize", _applicationContext, _gameID);
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
					Debug.unityLogger.Log("Unity Ads ERROR", e.Message);
					return null;
				}
			});
		}
	}
}