using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public class Android: NimbusAPI {
		private AndroidJavaClass _nimbus;
		private AndroidJavaClass _helper;
		private AndroidJavaClass _unityPlayer;
		private AndroidJavaObject _currentActivity;
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private const string HelperClass = "com.nimbus.demo.UnityHelper";
		private const string AndroidLogger = "com.adsbynimbus.Nimbus$Logger$Default";
		

		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_nimbus = new AndroidJavaClass(NimbusPackage);
			_helper = new AndroidJavaClass(HelperClass);
            
			var androidLogger = new AndroidJavaObject(AndroidLogger, 0);
			_nimbus.CallStatic("addLogger", androidLogger);
			_nimbus.CallStatic("initialize", _currentActivity, configuration.publisherKey.Trim(), configuration.apiKey.Trim());
			_nimbus.CallStatic("setTestMode", configuration.enableSDKInTestMode);
			_helper.CallStatic("setApp", Application.identifier, configuration.appName.Trim(), configuration.appStoreURL.Trim());
		}
		
		
		internal override void LoadAndShowBannerAd(ILogger logger) {
			_helper.CallStatic("showBannerAd", _currentActivity);
		}

		internal override void LoadAndShowInterstitialAd(ILogger logger) {
			_helper.CallStatic("showInterstitialAd", _currentActivity);
		}

		internal override void LoadAndShowRewardedVideoAd(ILogger logger) {
			_helper.CallStatic("showRewardedVideoAd", _currentActivity);
		}

	}
}