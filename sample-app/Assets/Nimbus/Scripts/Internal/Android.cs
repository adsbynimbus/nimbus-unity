using System;
using Nimbus.Scripts.ScriptableObjects;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
namespace Nimbus.Scripts.Internal {
	public class Android : NimbusAPI {
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private const string HelperClass = "com.nimbus.demo.UnityHelper";
		private const string AndroidLogger = "com.adsbynimbus.Nimbus$Logger$Default";
		private static AndroidJavaClass _helper;
		private AndroidJavaObject _currentActivity;
		private AndroidJavaClass _nimbus;
		private AndroidJavaClass _unityPlayer;


		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_nimbus = new AndroidJavaClass(NimbusPackage);
			_helper = new AndroidJavaClass(HelperClass);

			var androidLogger = new AndroidJavaObject(AndroidLogger, 0);
			_nimbus.CallStatic("addLogger", androidLogger);
			_nimbus.CallStatic("initialize", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim());
			_nimbus.CallStatic("setTestMode", configuration.enableSDKInTestMode);
			_helper.CallStatic("setApp", configuration.androidBundleID.Trim(),
				configuration.appName.Trim(), configuration.appDomain.Trim(), configuration.androidAppStoreURL.Trim());
		}

		internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			var listener = new AdManagerListener(logger, in _helper, ref nimbusAdUnit);
			var functionCall = nimbusAdUnit.AdType switch {
				AdUnityType.Banner => "showBannerAd",
				AdUnityType.Interstitial => "showInterstitialAd",
				AdUnityType.Rewarded => "showRewardedVideoAd",
				_ => throw new Exception("ad type not supported")
			};
			_helper.CallStatic(functionCall, _currentActivity, nimbusAdUnit.Position, nimbusAdUnit.BidFloors.BannerFloor, nimbusAdUnit.BidFloors.VideoFloor, nimbusAdUnit.CloseButtonDelayMillis, listener);
			return nimbusAdUnit;
		}
	}
}