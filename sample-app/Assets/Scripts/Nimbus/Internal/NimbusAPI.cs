using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration);
		internal abstract NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit);
		
	}
}