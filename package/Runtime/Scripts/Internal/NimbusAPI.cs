using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

// ReSharper disable CheckNamespace
namespace Nimbus.Runtime.Scripts.Internal {
	public abstract class NimbusAPI {
		internal abstract void InitializeSDK(ILogger logger, NimbusSDKConfiguration configuration);
		internal abstract NimbusAdUnit LoadAndShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit);
		internal abstract NimbusAdUnit LoadAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit);
		internal abstract NimbusAdUnit ShowAd(ILogger logger, ref NimbusAdUnit nimbusAdUnit);

		// ReSharper disable once InconsistentNaming
		internal abstract void SetGDPRConsentString(string consent);
	}
}