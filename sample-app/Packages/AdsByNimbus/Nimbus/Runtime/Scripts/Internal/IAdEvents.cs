namespace Nimbus.Runtime.Scripts.Internal {
	public interface IAdEvents {
		void AdWasRendered(NimbusAdUnit nimbusAdUnit);
		void AdError(NimbusAdUnit nimbusAdUnit);
		void AdEvent(NimbusAdUnit nimbusAdUnit);
	}
}