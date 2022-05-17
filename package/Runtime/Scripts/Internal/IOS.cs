using System;
using System.Runtime.InteropServices;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

// ReSharper disable CheckNamespace
namespace Nimbus.Runtime.Scripts.Internal {
	public class IOS : NimbusAPI {
		private static void OnDestroyIOSAd(int adUnitInstanceId)
		{
			_destroyAd(adUnitInstanceId);
		}

		#region Declare external C interface

		[DllImport("__Internal")]
		private static extern void _initializeSDKWithPublisher(string publisher,
			string apiKey,
			bool enableSDKInTestMode,
			bool enableUnityLogs);

		[DllImport("__Internal")]
		private static extern void _showBannerAd(int adUnitInstanceId, string position, float bannerFloor);

		[DllImport("__Internal")]
		private static extern void _showInterstitialAd(int adUnitInstanceId, string position, float bannerFloor, float videoFloor,
			double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _showRewardedVideoAd(int adUnitInstanceId, string position, float videoFloor, double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _setGDPRConsentString(string consent);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);

		#endregion

		#region Wrapped methods and properties

		private readonly NimbusIOSAdManager _iOSAdManager;

		public IOS() {
			_iOSAdManager = NimbusIOSAdManager.Instance;
		}

		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Initializing IOS SDK");
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableSDKInTestMode,
				configuration.enableUnityLogs);
		}

		internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			_iOSAdManager.AddAdUnit(nimbusAdUnit);
			nimbusAdUnit.OnDestroyIOSAd += OnDestroyIOSAd;
			
			var closeButtonDelaySeconds = nimbusAdUnit.CloseButtonDelayInSeconds;
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					_showBannerAd(nimbusAdUnit.InstanceID, nimbusAdUnit.Position, nimbusAdUnit.BidFloors.BannerFloor);
					break;
				case AdUnityType.Interstitial:
					closeButtonDelaySeconds = 5;
					_showInterstitialAd(nimbusAdUnit.InstanceID, nimbusAdUnit.Position, nimbusAdUnit.BidFloors.BannerFloor,
						nimbusAdUnit.BidFloors.VideoFloor, closeButtonDelaySeconds);
					break;
				case AdUnityType.Rewarded:
					_showRewardedVideoAd(nimbusAdUnit.InstanceID, nimbusAdUnit.Position, nimbusAdUnit.BidFloors.VideoFloor,
						closeButtonDelaySeconds);
					break;
				default:
					throw new Exception("ad type not supported");
			}

			return nimbusAdUnit;
		}

		internal override NimbusAdUnit RequestAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			throw new Exception("ios not supported yet");
		}

		internal override NimbusAdUnit ShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			throw new Exception("ios not supported yet");
		}

		internal override void SetGDPRConsentString(string consent) {
			_setGDPRConsentString(consent);
		}

		#endregion
	}
}