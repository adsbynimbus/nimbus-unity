using System;
using Nimbus.Scripts.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Scripts.Internal {
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
					break;
				case AdUnityType.Interstitial:
					functionCall = "InterstitialAd()";
					break;
				case AdUnityType.Rewarded:
					functionCall = "RewardedAd()";
					break;
				default:
					throw new Exception("ad type not supported");
			}

			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
					nimbusAdUnit.CurrentAdState = AdEventTypes.IMPRESSION;
					break;
				case AdUnityType.Interstitial:
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
				case AdUnityType.Rewarded:
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
			}

			nimbusAdUnit.EmitOnAdEvent(nimbusAdUnit);
			logger.Log($"In Editor mode, {functionCall} was called, however ads cannot be shown in the editor");
			return nimbusAdUnit;
		}
	}
}