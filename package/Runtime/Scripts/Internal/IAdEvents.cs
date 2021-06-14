namespace Nimbus.Runtime.Scripts.Internal {
	public interface IAdEvents {
		void OnAdWasRendered(NimbusAdUnit nimbusAdUnit);

		void OnAdError(NimbusAdUnit nimbusAdUnit);

		// basic ad events
		void OnAdLoaded(NimbusAdUnit nimbusAdUnit);
		void OnAdImpression(NimbusAdUnit nimbusAdUnit);
		void OnAdClicked(NimbusAdUnit nimbusAdUnit);

		void OnAdDestroyed(NimbusAdUnit nimbusAdUnit);

		// video specific events
		void OnVideoAdPaused(NimbusAdUnit nimbusAdUnit);
		void OnVideoAdResume(NimbusAdUnit nimbusAdUnit);
		void OnVideoAdCompleted(NimbusAdUnit nimbusAdUnit, bool skipped);
	}
}