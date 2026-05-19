using System.Collections.Generic;
using Nimbus.ScriptableObjects;

namespace Nimbus.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(NimbusSDKConfiguration configuration);
		internal abstract void getAd(NimbusAdUnit nimbusAdUnit, bool showAd);
	}
}