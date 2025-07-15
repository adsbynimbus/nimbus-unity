using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private readonly AndroidJavaObject _applicationContext;
		
		public MintegralAndroid(AndroidJavaObject applicationContext, string appID, string appKey, ThirdPartyAdUnit[] adUnitIds) {
			_applicationContext = applicationContext;
			_appID = appID;
			_appKey = appKey;
			_adUnitIds = adUnitIds;
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
		
		private JObject GetProviderRtbDataFromNativeSDK(AdUnitType type)
		{
			var adUnitId = "";
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					adUnitId = adUnit.AdUnitId;
					break;
				}
			}
			if (adUnitId.IsNullOrEmpty())
			{
				return null;
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
				return mintegralObj;
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Mintegral ERROR", e.Message);
			}
			return null;
		}
		
		internal BidRequestDelta GetBidRequestDelta(JObject data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data == null) {
				return bidRequestDelta;
			}
			bidRequestDelta.ComplexUserExt = 
				new KeyValuePair<string, JObject> ("mintegral_sdk", data);
			return bidRequestDelta;
		}
		
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetProviderRtbDataFromNativeSDK(type));
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