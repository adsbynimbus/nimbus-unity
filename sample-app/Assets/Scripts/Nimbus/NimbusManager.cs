using Nimbus.Internal;
using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus {
	public class NimbusManager : MonoBehaviour {
		public NimbusSDKConfiguration configuration;

		public static NimbusManager Instance;
		private readonly NimbusLogger _logger = new NimbusLogger();
		private NimbusAPI _nimbusPlatformAPI;

		private void Awake() {
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
			
				_nimbusPlatformAPI.InitializeSDK(_logger, configuration);
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this) {
				Destroy(gameObject);
			}
		}

		public void LoadAndShowBannerAd() {
			_nimbusPlatformAPI.LoadAndShowBannerAd(_logger);
		}

		public void LoadAndShowInterstitialAd() {
			_nimbusPlatformAPI.LoadAndShowInterstitialAd(_logger);
		}

		public void LoadAndShowRewardedVideoAd() {
			_nimbusPlatformAPI.LoadAndShowRewardedVideoAd(_logger);
		}
	}
}