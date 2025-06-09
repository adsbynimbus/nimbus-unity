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
		private const string NimbusMolocoPackage = "com.moloco.sdk.publisher.Moloco";

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
				var molocoInitObj = new AndroidJavaObject("com.moloco.sdk.publisher.MolocoInitParams", _applicationContext, _appKey, molocoMediationInfoObj);
				var molocoClass = new AndroidJavaClass(NimbusMolocoPackage);
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
				var molocoBidManager = new AndroidJavaClass("com.mbridge.msdk.mbbid.out.BidManager");
				var buyerId = molocoBidManager.CallStatic<string>("getBuyerUid", _applicationContext);
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
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.simpleUserExt = 
				new KeyValuePair<string, string> ("moloco_buyeruid", data);	
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
					Debug.unityLogger.Log("Mintegral ERROR", e.Message);
					return null;
				}
			});
		}
	}
	
}