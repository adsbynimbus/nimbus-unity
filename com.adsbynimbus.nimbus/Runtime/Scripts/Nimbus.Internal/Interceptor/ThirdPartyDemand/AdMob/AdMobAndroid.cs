using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob {
	internal class AdMobAndroid : IInterceptor, IProvider {
		private const string NimbusAdMobPackage = "com.adsbynimbus.request.NimbusRequestsAdmob";

		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly bool _testMode;
		private readonly AdMobAdUnit[] _adUnitIds;
		private AdUnitType _adUnitType;
		private string _adUnitId;
		private readonly AndroidJavaObject _applicationContext;
		
		public AdMobAndroid(string appID, AdMobAdUnit[] adUnitIds, bool enableTestMode) {
			_appID = appID;
			_adUnitIds = adUnitIds;
			_testMode = enableTestMode;
		}
		
		public AdMobAndroid(AndroidJavaObject applicationContext, string appID, AdMobAdUnit[] adUnitIds, bool enableTestMode) {
			_applicationContext = applicationContext;
			_appID = appID;
			_adUnitIds = adUnitIds;
			_enableTestMode = enableTestMode;
		}
		
		public void InitializeNativeSDK() {
			//do nothing
		}
		
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
			foreach (AdMobAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					_adUnitId = adUnit.AdUnitId;
					_adUnitType = type;
					Debug.unityLogger.Log("AdMob AdUnitId", adUnit.AdUnitId);
					return adUnit.AdUnitId;
				}
			}
			return "";
		}
		
		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			var width = 0;
			var height = 0;
			if (!bidRequest.Imp.IsNullOrEmpty())
			{
				if (bidRequest.Imp[0].Banner != null)
				{
					width = bidRequest.Imp[0].Banner.W ?? 0;
					height = bidRequest.Imp[0].Banner.H ?? 0;
				}
			}
			// data is the adUnitId
			var adMob = new AndroidJavaClass(NimbusAdMobPackage);
			var adMobSignal = "";
			switch (_adUnitType)
			{
				case AdUnitType.Banner: case AdUnitType.Undefined:
					adMobSignal = adMob.CallStatic<string>("fetchAdMobBannerSignal", data, width, height);
					break;
				case AdUnitType.Interstitial:
					adMobSignal = adMob.CallStatic<string>("fetchAdMobInterstitialSignal", data);
					break;
				case AdUnitType.Rewarded:
					adMobSignal = adMob.CallStatic<string>("fetchAdMobRewardedSignal", data);
					break;
			}
			bidRequest.User.Ext.AdMobSignals = adMobSignal;
			return bidRequest;
		}
	}
	
}