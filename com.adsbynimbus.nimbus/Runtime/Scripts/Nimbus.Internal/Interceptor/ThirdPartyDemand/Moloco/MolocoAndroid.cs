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

		private readonly string _appKey;
		private readonly bool _enableTestMode;
		private readonly bool _testMode;
		private readonly AndroidJavaObject _applicationContext;
		
		public MolocoAndroid(AndroidJavaObject applicationContext, string appKey, bool enableTestMode) {
			_applicationContext = applicationContext;
			_appKey = appKey;
			_enableTestMode = enableTestMode;
		}
		
		public void InitializeNativeSDK() {
			try
			{
				var molocoMediationInfoObj = new AndroidJavaObject("com.moloco.sdk.publisher.MediationInfo", "none");
				var molocoInitObj = new AndroidJavaObject("com.moloco.sdk.publisher.init.MolocoInitParams", _applicationContext, _appKey, molocoMediationInfoObj);
				var molocoClass = new AndroidJavaClass("com.moloco.sdk.publisher.Moloco");
				molocoClass.CallStatic("initialize", molocoInitObj);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Moloco Initialization ERROR", e.Message);
			}
		}
		
		internal string GetMolocoToken() {
			try
			{
				AndroidJNI.AttachCurrentThread();
				var args = new object[] {};
				var buyerId = BridgeHelpers.GetStringFromJavaFuture(
					"com.adsbynimbus.request.internal.NimbusRequestsMolocoInternal",
					"token", args, 1000L);
				if (buyerId != null)
				{
					return buyerId;			
				}
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("Moloco Token ERROR", e.Message);
			}

			return "";
		}
		
		internal BidRequestDelta GetBidRequestDelta(string data) {
			var bidRequestDelta = new BidRequestDelta();
			var jsonObject = JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject;
			if (jsonObject == null || data.IsNullOrEmpty() || !jsonObject.ContainsKey("first")) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = 
				new KeyValuePair<string, string> ("moloco_buyeruid", jsonObject["first"].ToString());	
			return bidRequestDelta;
		}
		
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetMolocoToken());
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("Moloco ERROR", e.Message);
					return null;
				}
			});
		}
	}
	
}