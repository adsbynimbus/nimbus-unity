using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	internal class MetaAndroid : IInterceptor, IProvider {
		private const string NimbusMetaPackage = "com.adsbynimbus.request.FANDemandProvider";
		private readonly string _appID;
		private readonly bool _testMode;
		private readonly AndroidJavaObject _applicationContext;
		
		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("facebook_buyeruid", data);
			if (bidRequest.Imp.Length > 0)
			{
				var impExt = new ImpExt();
				impExt.FacebookAppId = _appID;
				if (_testMode)
				{
					impExt.MetaTestAdType = "IMG_16_9_LINK";
				}
				bidRequestDelta.impressionExtension = impExt;
			}
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			AndroidJNI.AttachCurrentThread();
			var meta = new AndroidJavaClass(NimbusMetaPackage);
			var buyerId = meta.GetStatic<string>("bidderToken");
			return buyerId;
		}
		
		public MetaAndroid(AndroidJavaObject applicationContext, bool testMode, string appID) {
			_applicationContext = applicationContext;
			_testMode = testMode;
			_appID = appID;
		}
		
		public void InitializeNativeSDK() {
			var meta = new AndroidJavaClass(NimbusMetaPackage);
			meta.CallStatic("initialize", _applicationContext, _appID);
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
					Debug.unityLogger.Log("META ERROR", e.Message);
					return null;
				}
			});
		}
	}
}