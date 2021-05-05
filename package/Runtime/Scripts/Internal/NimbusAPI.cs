using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Runtime.Scripts.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration);
		internal abstract NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit);
		// ReSharper disable once InconsistentNaming
		internal abstract void SetGDPRConsentString(string consent);
	}
}