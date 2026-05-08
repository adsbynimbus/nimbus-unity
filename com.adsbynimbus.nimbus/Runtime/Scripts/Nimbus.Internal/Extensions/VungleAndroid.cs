using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Extensions {
	internal class VungleAndroid {
		private const string NimbusVunglePackage = "com.adsbynimbus.request.VungleDemandProvider";
		private const string VunglePackage = "com.vungle.ads.VungleAds";
		private readonly string _appID;
		private readonly AndroidJavaObject _applicationContext;
		public static TaskCompletionSource<string> BiddingTokenTask = new ();
		
		public VungleAndroid(AndroidJavaObject applicationContext, string appID) {
			_applicationContext = applicationContext;
			_appID = appID;
		}
		
		public void InitializeNativeSDK() {
			var nimbusVungle = new AndroidJavaClass(NimbusVunglePackage);
			nimbusVungle.CallStatic("initialize", _appID);
			var vungle = new AndroidJavaClass(VunglePackage);
			var vungleWrapperFramework = new AndroidJavaClass("com.vungle.ads.VungleWrapperFramework");
			var hbs = vungleWrapperFramework.GetStatic<AndroidJavaObject>("vunglehbs");
			vungle.CallStatic("setIntegrationName", hbs, "29");
		}
	}
}