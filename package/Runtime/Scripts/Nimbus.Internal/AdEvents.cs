using System;

namespace Nimbus.Internal {
	public class AdEvents {
		public event Action<NimbusAdUnit> OnAdLoaded;
		public event Action<NimbusAdUnit> OnAdRendered;
		public event Action<NimbusAdUnit> OnAdError;
		public event Action<NimbusAdUnit> OnAdImpression;
		public event Action<NimbusAdUnit> OnAdClicked;
		public event Action<NimbusAdUnit> OnAdDestroyed;
		public event Action<NimbusAdUnit> OnVideoAdPaused;
		public event Action<NimbusAdUnit> OnVideoAdResume;
		public event Action<NimbusAdUnit, bool> OnAdCompleted;
		
		internal void FireOnAdLoadedEvent(NimbusAdUnit obj) {
			OnAdLoaded?.Invoke(obj);
		}

		internal void FireOnAdRenderedEvent(NimbusAdUnit obj) {
			OnAdRendered?.Invoke(obj);
		}

		internal void FireOnAdImpressionEvent(NimbusAdUnit obj) {
			OnAdImpression?.Invoke(obj);
		}

		internal void FireOnAdClickedEvent(NimbusAdUnit obj) {
			OnAdClicked?.Invoke(obj);
		}

		internal void FireOnAdDestroyedEvent(NimbusAdUnit obj) {
			OnAdDestroyed?.Invoke(obj);
		}

		internal void FireOnVideoAdPausedEvent(NimbusAdUnit obj) {
			OnVideoAdPaused?.Invoke(obj);
		}

		internal void FireOnVideoAdResumeEvent(NimbusAdUnit obj) {
			OnVideoAdResume?.Invoke(obj);
		}

		internal void FireOnAdCompletedEvent(NimbusAdUnit obj, bool skipped) {
			OnAdCompleted?.Invoke(obj, skipped);
		}
		
		internal void FireOnAdErrorEvent(NimbusAdUnit obj) {
			OnAdError?.Invoke(obj);
		}
	}


	// ReSharper disable InconsistentNaming
	// Events as named by the Nimbus Android SDK
	public enum AdEventTypes {
		NOT_LOADED,

		LOADED,
		IMPRESSION,
		CLICKED,
		PAUSED,
		RESUMED,

		// FIRST_QUARTILE,
		// MIDPOINT,
		// THIRD_QUARTILE,
		COMPLETED,

		// SKIPPED,
		// VOLUME_CHANGED
		DESTROYED
	}
}