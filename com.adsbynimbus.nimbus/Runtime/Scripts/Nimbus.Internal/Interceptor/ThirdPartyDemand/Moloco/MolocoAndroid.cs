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

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Moloco {
	internal class MolocoAndroid : IInterceptor, IProvider {
		private const string NimbusMolocoPackage = "com.adsbynimbus.request.MolocoDemandProvider";

		private readonly string _appKey;
		private readonly bool _enableTestMode;
		private readonly bool _testMode;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _adUnitType;
		private string _adUnitId;
		private string _adUnitPlacementId;
		private readonly AndroidJavaObject _applicationContext;
		
		public MolocoAndroid(AndroidJavaObject applicationContext, string appKey, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_applicationContext = applicationContext;
			_appKey = appKey;
			_adUnitIds = adUnitIds;
			_enableTestMode = enableTestMode;
		}
		
		public void InitializeNativeSDK() {
			try
			{
				var mintegralDemandClass = new AndroidJavaClass(NimbusMolocoPackage);
				mintegralDemandClass.CallStatic("initialize", _appKey);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Mintegral Initialization ERROR", e.Message);
			}

		}
		
		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
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
		
		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			try
			{
				AndroidJNI.AttachCurrentThread();
				var molocoBidManager = new AndroidJavaClass("com.mbridge.msdk.mbbid.out.BidManager");
				var buyerId = molocoBidManager.CallStatic<string>("getBuyerUid", _applicationContext);
				if (buyerId != null)
				{
					bidRequestDelta.simpleUserExt = 
						new KeyValuePair<string, string> ("moloco_buyeruid", buyerId);			
				}
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
				try
				{
					return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("Mintegral ERROR", e.Message);
					return null;
				}
			});
		}
	}
	
}