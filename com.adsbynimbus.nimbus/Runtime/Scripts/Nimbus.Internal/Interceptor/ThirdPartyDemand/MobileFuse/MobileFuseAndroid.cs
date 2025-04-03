using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse {
	internal class MobileFuseAndroid : IInterceptor, IProvider {
		private readonly string _gameID;
		private readonly bool _testMode;
		
		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			var mobileFuseObject = JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject;
			bidRequest.User.Ext.MobileFuseBuyerData = mobileFuseObject;
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			var token = "";
			try
			{
				var timeUnit = new AndroidJavaClass("java.util.concurrent.TimeUnit");
				var timeUnitMillis = timeUnit.CallStatic<AndroidJavaObject>("valueOf", "MILLISECONDS");
				var unityHelper = new AndroidJavaClass("com.adsbynimbus.request.internal.NimbusRequestsMobileFuseInternal");
				var future = unityHelper.CallStatic<AndroidJavaObject>("token");
				token = future.Call<String>("get", 500L, timeUnitMillis);
				Debug.unityLogger.Log("MobileFuse Token", token);
			}
			catch (Exception e)
			{
				Debug.unityLogger.Log("Unable to retrieve MobileFuse Token", e.Message);
			}
			return token;
		}
		public void InitializeNativeSDK() {
			// No initialization needed
		}
	}
}