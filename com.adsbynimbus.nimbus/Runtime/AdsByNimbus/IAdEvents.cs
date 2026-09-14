using AdsByNimbus;

public interface IAdEvents {
	void OnAdLoaded(Ad nimbusAdUnit);
	void OnAdWasRendered(Ad nimbusAdUnit);
	void OnAdClicked(Ad nimbusAdUnit);
	void OnAdCompleted(Ad nimbusAdUnit);
	void OnAdError(Ad nimbusAdUnit,  NimbusError nimbusError);
}
