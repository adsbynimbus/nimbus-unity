using System;

namespace Nimbus.Internal {
	public class AdEvents {
		public event Action<NimbusAdUnit> OnAdRendered;
		public event Action<NimbusAdUnit> OnAdError;
		
		public event Action<NimbusAdUnit> OnAdEvent;
		
		
		public void EmitOnAdError(NimbusAdUnit obj) {
			OnAdError?.Invoke(obj);
		}
		public void EmitOnAdEvent(NimbusAdUnit obj) {
			OnAdEvent?.Invoke(obj);
		}
		public void EmitOnAdRendered(NimbusAdUnit obj) {
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
		DESTROYED,
	}
}