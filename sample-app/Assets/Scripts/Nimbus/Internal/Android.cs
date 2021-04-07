using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public class Android: NimbusAPI {
		private AndroidJavaClass _nimbus;
		private AndroidJavaClass _helper;
		private AndroidJavaClass _unityPlayer;
		private AndroidJavaObject _currentActivity;
		private AndroidJavaProxy _managerListener;
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private const string HelperClass = "com.nimbus.demo.UnityHelper";
		private const string AndroidLogger = "com.adsbynimbus.Nimbus$Logger$Default";
		

		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_nimbus = new AndroidJavaClass(NimbusPackage);
			_helper = new AndroidJavaClass(HelperClass);
			_managerListener = new AdManagerListener(logger);
            
			var androidLogger = new AndroidJavaObject(AndroidLogger, 0);
			_nimbus.CallStatic("addLogger", androidLogger);
			_nimbus.CallStatic("initialize", _currentActivity, configuration.publisherKey.Trim(), configuration.apiKey.Trim());
			_nimbus.CallStatic("setTestMode", configuration.enableSDKInTestMode);
			_helper.CallStatic("setApp", Application.identifier, configuration.appName.Trim(), configuration.appStoreURL.Trim());
		}
		
		
		internal override void LoadAndShowBannerAd(ILogger logger) {
			_helper.CallStatic("showBannerAd", _currentActivity, _managerListener);
		}

		internal override void LoadAndShowInterstitialAd(ILogger logger) {
			_helper.CallStatic("showInterstitialAd", _currentActivity, _managerListener);
		}

		internal override void LoadAndShowRewardedVideoAd(ILogger logger) {
			_helper.CallStatic("showRewardedVideoAd", _currentActivity, _managerListener);
		}

	}

	public class AdManagerListener : AndroidJavaProxy {
		private ILogger _logger;

		public AdManagerListener(ILogger logger) : base("com.adsbynimbus.NimbusAdManager$Listener") {
			_logger = logger;
		}

		void onAdResponse(AndroidJavaObject response) {
			_logger.Log("Responded with ad type " + response.Call<string>("type"));
		}

        void onAdRendered(AndroidJavaObject controller) {
			_logger.Log("Ad Rendered");
			controller.Call<AndroidJavaClass>("listeners").Call("add", new AdControllerListener(_logger));
		}

		void onError(AndroidJavaObject adError) { 
			_logger.Log("Ad error " + adError.Call<string>("getMessage"));
		}
	}

	public class AdControllerListener : AndroidJavaProxy {
		private ILogger _logger;

		public AdControllerListener(ILogger logger) : base("com.adsbynimbus.render.AdController$Listener") {
			_logger = logger;
		}

		void onAdEvent(AndroidJavaObject adEvent) {
			_logger.Log("Ad event " + adEvent.Call<string>("name"));
		}

		void onError(AndroidJavaObject adError) { 
			_logger.Log("Ad error " + adError.Call<string>("getMessage"));
		}
	}
}