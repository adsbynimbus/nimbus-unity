using AdsByNimbus.Scripts;

public interface IAdEventsExtended : IAdEvents {
	void OnAdImpression(Ad nimbusAdUnit);
	void OnAdDestroyed(Ad nimbusAdUnit);
	void OnAdRewardEarned(Ad nimbusAdUnit);
}

public interface IAdEventsVideoExtended : IAdEvents {
	void OnVideoAdPaused(Ad nimbusAdUnit);
	void OnVideoAdResume(Ad nimbusAdUnit);
}