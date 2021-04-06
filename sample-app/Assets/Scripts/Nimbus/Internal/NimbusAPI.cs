using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration);
		internal abstract void LoadAndShowBannerAd(ILogger logger);
		internal abstract void LoadAndShowInterstitialAd(ILogger logger);
		internal abstract void LoadAndShowRewardedVideoAd(ILogger logger);
	}
}