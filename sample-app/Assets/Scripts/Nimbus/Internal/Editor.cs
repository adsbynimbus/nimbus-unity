using System;
using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public class Editor : NimbusAPI {
		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Mock SDK initialized for editor");
		}

		internal override NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit) {
			var functionCall = nimbusAdUnit.AdType switch {
				AdUnityType.Banner => "BannerAd()",
				AdUnityType.Interstitial => "InterstitialAd()",
				AdUnityType.Rewarded => "RewardedAd()",
				_ => throw new Exception("ad type not supported")
			};
			switch (nimbusAdUnit.AdType) {
				case AdUnityType.Banner:
				case AdUnityType.Interstitial:
					nimbusAdUnit.CurrentAdState = AdEventTypes.IMPRESSION;
					break;
				case AdUnityType.Rewarded:
					nimbusAdUnit.CurrentAdState = AdEventTypes.COMPLETED;
					break;
			}
			nimbusAdUnit.EmitOnAdEvent(nimbusAdUnit);
			logger.Log($"In Editor mode, {functionCall} was called, ads cannot be shown in the editor");
			return nimbusAdUnit;
		}
	}
}