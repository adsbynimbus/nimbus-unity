using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public class Editor : NimbusAPI {
		internal override void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration) {
			logger.Log("Mock SDK initialized for editor");
		}

		internal override void LoadAndShowBannerAd(ILogger logger) {
			logger.Log("In Editor mode, LoadAndShowBannerAd() was called, ads cannot be shown in the editor");
		}

		internal override void LoadAndShowInterstitialAd(ILogger logger) {
			logger.Log("In Editor mode, LoadAndShowInterstitialAd() was called, ads cannot be in the editor");
		}

		internal override void LoadAndShowRewardedVideoAd(ILogger logger) {
			logger.Log("In Editor mode, LoadAndShowRewardedVideoAd() was called, ads cannot be in the editor");
		}
	}
}