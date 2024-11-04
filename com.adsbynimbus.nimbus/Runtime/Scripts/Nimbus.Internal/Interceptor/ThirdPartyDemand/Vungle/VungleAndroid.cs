using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle {
	internal class VungleAndroid : IInterceptor, IProvider {
		private const string AndroidVunglePackage = "com.adsbynimbus.request.VungleDemandProvider";
		private readonly string _appID;
		private readonly AndroidJavaObject _currentActivity;
		private AndroidJavaClass _vungle;

		public VungleAndroid(string appID) {
			_appID = appID;
		}
		
		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			if (bidRequest.User.Ext != null) {
				bidRequest.User.Ext.VungleBuyerId = data;
			}
			
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			return _fetchVungleBuyerId();
		}
		
		public void InitializeNativeSDK() {
			_vungle = new AndroidJavaClass(AndroidVunglePackage);
			_vungle.CallStatic("initialize", _appID);
		}
	}
}