using System;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

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
	}
}