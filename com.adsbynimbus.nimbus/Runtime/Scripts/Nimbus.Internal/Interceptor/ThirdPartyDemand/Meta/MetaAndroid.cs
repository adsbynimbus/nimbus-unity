using System.Runtime.CompilerServices;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	internal class MetaAndroid : IInterceptor, IProvider {
		private const string NimbusMetaPackage = "com.adsbynimbus.request.FANDemandProvider";
		private readonly string _appID;
		private readonly AndroidJavaObject _applicationContext;

		public MetaAndroid(string appID) {
			_appID = appID;
		}
		
		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			bidRequest.User.Ext.FacebookBuyerId = data;
			if (bidRequest.Imp.Length > 0) {
				bidRequest.Imp[0].Ext.FacebookAppId = _appID;
				//below forces test ad
				//bidRequest.Imp[0].Ext.MetaTestAdType = "IMG_16_9_LINK";
			}

			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			Debug.unityLogger.Log("METABIDDINGTOKEN", "DO WE GET HERE");
			var meta = new AndroidJavaClass(NimbusMetaPackage);
			var buyerId = meta.GetStatic<string>("bidderToken");
			Debug.unityLogger.Log("METABIDDINGTOKEN", buyerId);
			return buyerId;
		}
		
		public MetaAndroid(AndroidJavaObject applicationContext, string appID) {
			_applicationContext = applicationContext;
			_appID = appID;
		}
		
		public void InitializeNativeSDK() {
			var meta = new AndroidJavaClass(NimbusMetaPackage);
			meta.CallStatic("initialize", _applicationContext, _appID);
		}
	}
}