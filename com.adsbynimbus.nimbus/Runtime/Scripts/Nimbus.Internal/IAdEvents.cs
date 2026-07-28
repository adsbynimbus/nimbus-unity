namespace Nimbus.Internal {
	public interface IAdEvents {
		void OnAdLoaded(Ad nimbusAdUnit);
		void OnAdWasRendered(Ad nimbusAdUnit);
		void OnAdClicked(Ad nimbusAdUnit);
		void OnAdCompleted(Ad nimbusAdUnit, bool skipped);
		void OnAdError(Ad nimbusAdUnit);
	}
}