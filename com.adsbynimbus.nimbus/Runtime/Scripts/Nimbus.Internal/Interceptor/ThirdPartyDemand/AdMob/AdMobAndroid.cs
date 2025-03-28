using System.Runtime.CompilerServices;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob {
	internal class AdMobAndroid : IInterceptor, IProvider {
		private const string NimbusAdMobPackage = "com.adsbynimbus.request.internal.NimbusRequestsAdMobInternal";

		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly bool _testMode;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _adUnitType;
		private string _adUnitId;
		private readonly AndroidJavaObject _applicationContext;
		
		
		public AdMobAndroid(AndroidJavaObject applicationContext, string appID, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_applicationContext = applicationContext;
			_appID = appID;
			_adUnitIds = adUnitIds;
			_enableTestMode = enableTestMode;
		}
		
		public void InitializeNativeSDK() {
			//do nothing
		}
		
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					_adUnitId = adUnit.AdUnitId;
					_adUnitType = type;
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
			try
			{
				var unityHelper = new AndroidJavaClass("com.adsbynimbus.unity.UnityHelper");
				var adMobSignal = unityHelper.CallStatic<string>("fetchAdMobSignal", (int) _adUnitType, data);

				bidRequest.User.Ext.AdMobSignals = adMobSignal;
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("AdMob AdUnitSignal ERROR", e.Message);
			}
			return bidRequest;
		}
	}
	
}