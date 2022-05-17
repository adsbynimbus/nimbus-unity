using System;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

// ReSharper disable CheckNamespace
namespace Nimbus.Runtime.Scripts.Internal {
	public class Editor : NimbusAPI {
		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Mock SDK initialized for editor");
		}

		internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			nimbusAdUnit.AdWasRendered = true;
			var functionCall = "";
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					functionCall = "BannerAd()";
					nimbusAdUnit.CurrentAdState = AdEventTypes.IMPRESSION;
					break;
				case AdUnityType.Interstitial:
					functionCall = "InterstitialAd()";
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
				case AdUnityType.Rewarded:
					functionCall = "RewardedAd()";
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
				default:
					throw new Exception("ad type not supported");
			}

			logger.Log($"In Editor mode, {functionCall} was called, however ads cannot be shown in the editor");
			return nimbusAdUnit;
		}

		internal override NimbusAdUnit RequestAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			nimbusAdUnit.AdWasRendered = true;
			var functionCall = "";
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					functionCall = "makeBannerRequest()";
					nimbusAdUnit.CurrentAdState = AdEventTypes.IMPRESSION;
					break;
				case AdUnityType.Interstitial:
					functionCall = "makeBannerRequest()";
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
				case AdUnityType.Rewarded:
					functionCall = "makeRewardedVideoRequest()";
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
				default:
					throw new Exception("ad type not supported");
			}

			logger.Log($"In Editor mode, {functionCall} was called, however ads cannot be requested in the editor");
			return nimbusAdUnit;
		}

		internal override NimbusAdUnit ShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			logger.Log($"In Editor mode, ShowAd was called, however ads cannot be shown in the editor");
			return nimbusAdUnit;
		}


		internal override void SetGDPRConsentString(string consent) {
			Debug.unityLogger.Log("Mock SDK consent string cannot be added");
		}
	}
}