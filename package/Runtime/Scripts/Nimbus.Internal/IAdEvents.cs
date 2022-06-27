namespace Nimbus.Internal {
	public interface IAdEvents {
		void OnAdLoaded(NimbusAdUnit nimbusAdUnit);
		void OnAdWasRendered(NimbusAdUnit nimbusAdUnit);
		void OnAdClicked(NimbusAdUnit nimbusAdUnit);
		void OnAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped);
		void OnAdError(NimbusAdUnit nimbusAdUnit);
	}
}