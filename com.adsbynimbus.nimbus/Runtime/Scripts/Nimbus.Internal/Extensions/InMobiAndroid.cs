using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]

namespace Nimbus.Internal.Extensions {
	internal class InMobiAndroid {

		private readonly string _accountId;
		private readonly AndroidJavaObject _applicationContext;
		
		public InMobiAndroid(AndroidJavaObject applicationContext, string accountId) {
			_applicationContext = applicationContext;
			_accountId = accountId;
		}
		
		
		public void InitializeNativeSDK() {
			try
			{
				var inMobiClass = new AndroidJavaObject("com.inmobi.sdk.InMobiSdk");
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
					"token", args, 3000L);
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
	}
	
}