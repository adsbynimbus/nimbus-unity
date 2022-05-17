using System;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable CheckNamespace
namespace Nimbus.Runtime.Scripts.Internal {
	public class Android : NimbusAPI {
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private const string HelperClass = "com.adsbynimbus.unity.UnityHelper";
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
		}

		internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			var listener = new AdManagerListener(logger, in _helper, ref nimbusAdUnit);
			var closeButtonDelayInSeconds = nimbusAdUnit.CloseButtonDelayInSeconds;
			string functionCall;
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					functionCall = "showBannerAd";
					break;
				case AdUnityType.Interstitial:
					functionCall = "showInterstitialAd";
					break;
				case AdUnityType.Rewarded:
					functionCall = "showRewardedVideoAd";
					break;
				default:
					throw new Exception("ad type not supported");
			}

			_helper.CallStatic(functionCall, _currentActivity, nimbusAdUnit.Position,
				nimbusAdUnit.BidFloors.BannerFloor, nimbusAdUnit.BidFloors.VideoFloor, closeButtonDelayInSeconds,
				listener);
			return nimbusAdUnit;
		}

		internal override NimbusAdUnit LoadAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			var listener = new AdManagerListener(logger, in _helper, ref nimbusAdUnit);
			string functionCall;
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					functionCall = "makeBannerRequest";
					break;
				case AdUnityType.Interstitial:
					functionCall = "makeInterstitialRequest";
					break;
				case AdUnityType.Rewarded:
					functionCall = "makeRewardedVideoRequest";
					break;
				default:
					throw new Exception("ad type not supported");
			}

			_helper.CallStatic(functionCall, _currentActivity, nimbusAdUnit.Position,
				nimbusAdUnit.BidFloors.BannerFloor, nimbusAdUnit.BidFloors.VideoFloor, listener);
			return nimbusAdUnit;
		}

		internal override NimbusAdUnit ShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			var listener = new AdManagerListener(logger, in _helper, ref nimbusAdUnit);
			string functionCall;
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					functionCall = "render";
					break;
				case AdUnityType.Interstitial:
					functionCall = "renderBlocking";
					break;
				case AdUnityType.Rewarded:
					functionCall = "renderBlocking";
					break;
				default:
					throw new Exception("ad type not supported");
			}
			var response = nimbusAdUnit.ResponseMetaData;
			_helper.CallStatic(functionCall, _currentActivity, response.Type, response.AuctionID, response.Markup, 
			response.Network, response.PlacementID, response.Width, response.Height, response.IsInterstitial,
			response.IsMraid, response.Position, response.ImpressionTrackers, response.ClickTrackers,
			response.Duration, 0, 0, listener);
			return nimbusAdUnit;
		}

		internal override void SetGDPRConsentString(string consent) {
			_helper.CallStatic("setUser", consent);
		}
	}
}