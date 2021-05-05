using Nimbus.Runtime.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	public class NimbusEventListenerExample : MonoBehaviour, IAdEvents {
		public void OnSDKInitialize(string gdprConsentString) {
			// TODO
		}

		public void OnAdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad was rendered for ad instance {nimbusAdUnit.InstanceID}");
		}

		public void OnAdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log($"NimbusEventListenerExample Err {nimbusAdUnit.ErrorMessage()}");
		}

		public void OnAdLoaded(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnAdImpression(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnAdClicked(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnAdDestroyed(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnVideoAdPaused(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnVideoAdResume(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}

		public void OnVideoAdCompleted(NimbusAdUnit nimbusAdUnit) {
			// TODO
		}
	}
}