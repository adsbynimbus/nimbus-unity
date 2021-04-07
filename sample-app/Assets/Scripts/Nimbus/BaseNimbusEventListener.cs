using Nimbus.Internal;
using UnityEngine;

namespace Nimbus {
	public class BaseNimbusEventListener: MonoBehaviour {
		protected virtual void Start() {
			NimbusManager.Instance.NimbusEvents.OnAdRendered += AdWasRendered;
			NimbusManager.Instance.NimbusEvents.OnAdError += AdError;
			NimbusManager.Instance.NimbusEvents.OnAdEvent += AdEvent;
		}
		
		protected virtual void OnDestroy() {
			NimbusManager.Instance.NimbusEvents.OnAdRendered -= AdWasRendered;
			NimbusManager.Instance.NimbusEvents.OnAdError += AdError;
			NimbusManager.Instance.NimbusEvents.OnAdEvent += AdEvent;
		}
		
		protected virtual void AdWasRendered(NimbusAdUnit nimbusAdUnit) {
			nimbusAdUnit.AdWasRendered = true;
		}
		
		protected virtual void AdError(NimbusAdUnit nimbusAdUnit) { }
		
		protected virtual void AdEvent(NimbusAdUnit nimbusAdUnit) { }
	}
}