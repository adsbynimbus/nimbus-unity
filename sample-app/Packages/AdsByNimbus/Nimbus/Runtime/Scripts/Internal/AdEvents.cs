using System;

namespace Nimbus.Runtime.Scripts.Internal {
	public class AdEvents {
		internal event Action<NimbusAdUnit> OnAdRendered;
		internal event Action<NimbusAdUnit> OnAdError;

		internal event Action<NimbusAdUnit> OnAdEvent;


		internal void EmitOnAdError(NimbusAdUnit obj) {
			OnAdError?.Invoke(obj);
		}

		internal void EmitOnAdEvent(NimbusAdUnit obj) {
			OnAdEvent?.Invoke(obj);
		}

		internal void EmitOnAdRendered(NimbusAdUnit obj) {
			OnAdRendered?.Invoke(obj);
		}
	}


	// ReSharper disable InconsistentNaming
	public enum AdEventTypes {
		NOT_LOADED,
		LOADED,
		IMPRESSION,
		CLICKED,
		PAUSED,
		RESUME,
		FIRST_QUARTILE,
		MIDPOINT,
		THIRD_QUARTILE,
		COMPLETED,

		// VOLUME_CHANGED
		DESTROYED
	}
}