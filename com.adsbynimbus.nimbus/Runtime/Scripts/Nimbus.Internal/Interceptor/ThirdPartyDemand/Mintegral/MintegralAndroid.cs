using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral {
	internal class MintegralAndroid : IInterceptor, IProvider {
		private const string NimbusMintegralPackage = "com.adsbynimbus.request.MintegralDemandProvider";

		private readonly string _appID;
		private readonly string _appKey;
		private readonly bool _enableTestMode;
		private readonly bool _testMode;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _adUnitType;
		private string _adUnitId;
		private string _adUnitPlacementId;
		private readonly AndroidJavaObject _applicationContext;
		
		public MintegralAndroid(string appID, string appKey, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_appID = appID;
			_appKey = appKey;
			_adUnitIds = adUnitIds;
			_testMode = enableTestMode;
		}
		
		public MintegralAndroid(AndroidJavaObject applicationContext, string appID, string appKey, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_applicationContext = applicationContext;
			_appID = appID;
			_appKey = appKey;
			_adUnitIds = adUnitIds;
			_enableTestMode = enableTestMode;
		}
		
		public void InitializeNativeSDK() {
			try
			{
				var mintegralDemandClass = new AndroidJavaClass(NimbusMintegralPackage);
				mintegralDemandClass.CallStatic("initialize", _appID, _appKey);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Mintegral Initialization ERROR", e.Message);
			}

		}
		
		private string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					_adUnitId = adUnit.AdUnitId;
					_adUnitPlacementId = adUnit.AdUnitPlacementId;
					_adUnitType = type;
					return adUnit.AdUnitId;
				}
			}
			return "";
		}
		
		private BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			try
			{
				AndroidJNI.AttachCurrentThread();
				var mintegralBidManager = new AndroidJavaClass("com.mbridge.msdk.mbbid.out.BidManager");
				var buyerId = mintegralBidManager.CallStatic<string>("getBuyerUid", _applicationContext);
				var mintegralMbConfiguration = new AndroidJavaClass("com.mbridge.msdk.out.MBConfiguration");
				var sdkVersion = mintegralMbConfiguration.GetStatic<string>("SDK_VERSION");
				var mintegralObj = new JObject();
				mintegralObj.Add("buyeruid", buyerId);
				mintegralObj.Add("sdkv",sdkVersion);
				bidRequestDelta.complexUserExt = 
					new KeyValuePair<string, JObject> ("mintegral_sdk", mintegralObj);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Mintegral ERROR", e.Message);
			}

			return bidRequestDelta;
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