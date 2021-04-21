using System;
using Nimbus.Scripts.Internal;
using UnityEngine;

namespace Example.Scripts {
	public class NimbusEventListenerExample : MonoBehaviour, IAdEvents {
		public void AdWasRendered(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample Ad was rendered for ad instance {nimbusAdUnit.InstanceID}"); 
		}

		public void AdError(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log($"NimbusEventListenerExample Err {nimbusAdUnit.ErrorMessage()}");
		} // ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
		public void AdEvent(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log(
				$"NimbusEventListenerExample AdEvent {nimbusAdUnit.GetCurrentAdState()} for ad of type {nimbusAdUnit.AdType}");
			switch (nimbusAdUnit.AdType) {
				// Handle Events for Banner and Interstitial ads
				case AdUnityType.Banner:
				case AdUnityType.Interstitial:
					switch (nimbusAdUnit.GetCurrentAdState()) {
						case AdEventTypes.NOT_LOADED:
							break;
						case AdEventTypes.LOADED:
							break;
						case AdEventTypes.CLICKED:
							break;
						case AdEventTypes.IMPRESSION:
							break;
						case AdEventTypes.DESTROYED:
							break;
					}

					break;
				// Handle Events for Rewarded video ads
				case AdUnityType.Rewarded:
					switch (nimbusAdUnit.GetCurrentAdState()) {
						case AdEventTypes.NOT_LOADED:
							break;
						case AdEventTypes.LOADED:
							break;
						case AdEventTypes.PAUSED:
							break;
						case AdEventTypes.RESUME:
							break;
						case AdEventTypes.CLICKED:
							break;
						case AdEventTypes.FIRST_QUARTILE:
							break;
						case AdEventTypes.MIDPOINT:
							break;
						case AdEventTypes.THIRD_QUARTILE:
							break;
						case AdEventTypes.COMPLETED:
							break;
						case AdEventTypes.IMPRESSION:
							break;
						case AdEventTypes.DESTROYED:
							break;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}