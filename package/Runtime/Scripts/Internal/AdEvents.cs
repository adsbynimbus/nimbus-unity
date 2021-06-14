using System;

namespace Nimbus.Runtime.Scripts.Internal {
	public class AdEvents {
		public event Action<NimbusAdUnit> OnAdRendered;
		public event Action<NimbusAdUnit> OnAdError;
		public event Action<NimbusAdUnit> OnAdLoaded;
		public event Action<NimbusAdUnit> OnAdImpression;
		public event Action<NimbusAdUnit> OnAdClicked;
		public event Action<NimbusAdUnit> OnAdDestroyed;
		public event Action<NimbusAdUnit> OnVideoAdPaused;
		public event Action<NimbusAdUnit> OnVideoAdResume;
		public event Action<NimbusAdUnit> OnVideoAdCompleted;

		internal void EmitOnAdError(NimbusAdUnit obj) {
			OnAdError?.Invoke(obj);
		}

		internal void EmitOnAdRendered(NimbusAdUnit obj) {
			OnAdRendered?.Invoke(obj);
		}

		internal void EmitOnOnAdLoaded(NimbusAdUnit obj) {
			OnAdLoaded?.Invoke(obj);
		}

		internal void EmitOnOnAdImpression(NimbusAdUnit obj) {
			OnAdImpression?.Invoke(obj);
		}

		internal void EmitOnOnAdClicked(NimbusAdUnit obj) {
			OnAdClicked?.Invoke(obj);
		}

		internal void EmitOnOnAdDestroyed(NimbusAdUnit obj) {
			OnAdDestroyed?.Invoke(obj);
		}

		internal void EmitOnOnVideoAdPaused(NimbusAdUnit obj) {
			OnVideoAdPaused?.Invoke(obj);
		}

		internal void EmitOnOnVideoAdResume(NimbusAdUnit obj) {
			OnVideoAdResume?.Invoke(obj);
		}

		internal void EmitOnOnVideoAdCompleted(NimbusAdUnit obj) {
			OnVideoAdCompleted?.Invoke(obj);
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
		SKIPPED,
		// VOLUME_CHANGED
		DESTROYED
	}
}