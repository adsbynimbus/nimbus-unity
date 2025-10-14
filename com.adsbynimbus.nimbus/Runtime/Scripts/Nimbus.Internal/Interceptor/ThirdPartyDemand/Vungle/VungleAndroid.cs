using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle {
	internal class VungleAndroid : IInterceptor, IProvider {
		private const string NimbusVunglePackage = "com.adsbynimbus.request.VungleDemandProvider";
		private const string VunglePackage = "com.vungle.ads.VungleAds";
		private readonly string _appID;
		private readonly AndroidJavaObject _applicationContext;
		public static TaskCompletionSource<string> BiddingTokenTask = new ();
		
		public VungleAndroid(AndroidJavaObject applicationContext, string appID) {
			_applicationContext = applicationContext;
			_appID = appID;
		}
		
		internal BidRequestDelta GetBidRequestDelta(string data) {
			AndroidJNI.AttachCurrentThread();
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			bidRequestDelta.SimpleUserExt = new KeyValuePair<string, string>("vungle_buyeruid", data);
			return bidRequestDelta;
		}
		
		private class BidTokenCallback : AndroidJavaProxy
		{
			public BidTokenCallback() : base("com.vungle.ads.BidTokenCallback") { }

			void onBidTokenCollected(string bidToken)
			{
				BiddingTokenTask.SetResult(bidToken); 
			}

			void onBidTokenError(string errorMessage)
			{
				Debug.unityLogger.Log("Vungle BidToken ERROR", errorMessage);
			}
		}

		internal async Task<string> GetProviderRtbDataFromNativeSDK()
		{
			AndroidJNI.AttachCurrentThread();
			BidTokenCallback callback = new BidTokenCallback();
			var vungle = new AndroidJavaClass(VunglePackage);
			vungle.CallStatic("getBiddingToken", _applicationContext, callback);
			var biddingToken = await BiddingTokenTask.Task;
			return biddingToken;
		}
		
		public void InitializeNativeSDK() {
			var nimbusVungle = new AndroidJavaClass(NimbusVunglePackage);
			nimbusVungle.CallStatic("initialize", _appID);
			var vungle = new AndroidJavaClass(VunglePackage);
			var vungleWrapperFramework = new AndroidJavaClass("com.vungle.ads.VungleWrapperFramework");
			var hbs = vungleWrapperFramework.GetStatic<AndroidJavaObject>("vunglehbs");
			vungle.CallStatic("setIntegrationName", hbs, "29");
		}
		
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(
				async () =>
				{
					try
					{
						return GetBidRequestDelta(await GetProviderRtbDataFromNativeSDK());
					}
					catch (Exception e)
					{
						Debug.unityLogger.Log("Vungle ERROR", e.Message);
						return null;
					}
				}
			);
		}
	}
}