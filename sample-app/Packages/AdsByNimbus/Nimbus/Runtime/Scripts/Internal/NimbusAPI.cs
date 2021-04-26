using Nimbus.Scripts.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Scripts.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration);
		internal abstract NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit);
	}
}