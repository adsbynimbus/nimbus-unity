// ReSharper disable CheckNamespace

namespace Nimbus.Runtime.Scripts.Internal {
	public interface IAdEvents {
		void OnAdWasRendered(NimbusAdUnit nimbusAdUnit);
		void OnAdClicked(NimbusAdUnit nimbusAdUnit);
		void OnAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped);
		void OnAdError(NimbusAdUnit nimbusAdUnit);
	}
}