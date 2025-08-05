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

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.InMobi {
	internal class InMobiAndroid : IInterceptor, IProvider {

		private readonly string _accountId;
		private readonly AndroidJavaObject _applicationContext;
		
		public InMobiAndroid(AndroidJavaObject applicationContext, string accountId) {
			_applicationContext = applicationContext;
			_accountId = accountId;
		}
		
		public void InitializeNativeSDK() {
			try
			{
				var inMobiClass = new AndroidJavaClass("com.inmobi.monetization.InMobiSDK");
				inMobiClass.CallStatic("init", _applicationContext, _accountId, null, null);
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("InMobi Initialization ERROR", e.Message);
			}
		}
		
		internal string GetInMobiToken() {
			try
			{
				AndroidJNI.AttachCurrentThread();
				var args = new object[] {};
				var buyerId = BridgeHelpers.GetStringFromJavaFuture(
					"com.adsbynimbus.request.internal.NimbusRequestsInMobiInternal",
					"token", args, 1000L);
				if (buyerId != null)
				{
					return buyerId;			
				}
			}
			catch (AndroidJavaException e)
			{
				Debug.unityLogger.Log("InMobi Token ERROR", e.Message);
			}

			return "";
		}
		
		internal BidRequestDelta GetBidRequestDelta(string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.SimpleUserExt = 
				new KeyValuePair<string, string> ("inmobi_buyeruid", data);	
			return bidRequestDelta;
		}
		
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetInMobiToken());
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("InMobi ERROR", e.Message);
					return null;
				}
			});
		}
	}
	
}