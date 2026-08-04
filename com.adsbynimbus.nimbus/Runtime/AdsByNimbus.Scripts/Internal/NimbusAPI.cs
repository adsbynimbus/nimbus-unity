using AdsByNimbus.Scripts;
using ScriptableObjects;

namespace Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(NimbusSDKConfiguration configuration);
		internal abstract void GetAd(Ad nimbusAdUnit, bool showAd);
		internal abstract void ShowAd(Ad nimbusAdUnit);
	}
}