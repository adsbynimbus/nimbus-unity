using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Extensions {
	internal class UnityAdsAndroid {
		private const string NimbusUnityAdsPackage = "com.adsbynimbus.request.UnityDemandProvider";
		private const string UnityAdsPackage = "com.unity3d.ads.UnityAds";
		private readonly string _gameID;
		private readonly AndroidJavaObject _applicationContext;
		
		public UnityAdsAndroid(AndroidJavaObject applicationContext, string gameID) {
			_applicationContext = applicationContext;
			_gameID = gameID;
		}
		
		public void InitializeNativeSDK() {
			var unityAds = new AndroidJavaClass(NimbusUnityAdsPackage);
			unityAds.CallStatic("initialize", _applicationContext, _gameID);
		}
	}
}