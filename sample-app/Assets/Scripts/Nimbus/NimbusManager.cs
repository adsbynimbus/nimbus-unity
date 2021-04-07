using System;
using Nimbus.Internal;
using Nimbus.ScriptableObjects;
using UnityEngine;


namespace Nimbus {
	public class NimbusManager : MonoBehaviour {
		public NimbusSDKConfiguration configuration;
		public static NimbusManager Instance;
		private NimbusAPI _nimbusPlatformAPI;

		#region NimbusEvents

		public AdEvents NimbusEvents;
		

		#endregion

		private void Awake() {
			if (configuration == null) {
				throw new Exception("The configuration object cannot be null");
			}
			
			if (Instance == null) {
				_nimbusPlatformAPI ??= new
#if UNITY_EDITOR
					Editor
#elif UNITY_ANDROID
                Android
#else
   //TODO swap for IOS
                Android
#endif
					();
				
				NimbusEvents = new AdEvents();
				Debug.unityLogger.logEnabled = configuration.enableUnityLogs;
				_nimbusPlatformAPI.InitializeSDK(Debug.unityLogger, configuration);
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this) {
				Destroy(gameObject);
			}
		}
		
		public NimbusAdUnit LoadAndShowBannerAd() {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, ref NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		public NimbusAdUnit LoadAndShowInterstitialAd() {
			var adUnit = new NimbusAdUnit(AdUnityType.Interstitial, ref NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		public NimbusAdUnit LoadAndShowRewardedVideoAd() {
			var adUnit = new NimbusAdUnit(AdUnityType.Rewarded, ref NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}
	}
}