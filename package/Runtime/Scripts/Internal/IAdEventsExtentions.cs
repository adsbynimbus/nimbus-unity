// ReSharper disable CheckNamespace

namespace Nimbus.Runtime.Scripts.Internal {
	public interface IAdEventsExtended : IAdEvents {
		void OnAdImpression(NimbusAdUnit nimbusAdUnit);
		void OnAdDestroyed(NimbusAdUnit nimbusAdUnit);
	}

	public interface IAdEventsVideoExtended : IAdEvents {
		void OnVideoAdPaused(NimbusAdUnit nimbusAdUnit);
		void OnVideoAdResume(NimbusAdUnit nimbusAdUnit);
	}
}