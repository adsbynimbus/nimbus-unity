using System;

namespace Nimbus.Internal {
	public class AdEvents {
		public event Action<Ad> OnAdLoaded;
		public event Action<Ad> OnAdRendered;
		public event Action<Ad> OnAdError;
		public event Action<Ad> OnAdImpression;
		public event Action<Ad> OnAdClicked;
		public event Action<Ad> OnAdDestroyed;
		public event Action<Ad> OnVideoAdPaused;
		public event Action<Ad> OnVideoAdResume;
		public event Action<Ad> OnAdRewardEarned;
		public event Action<Ad, bool> OnAdCompleted;
		
		internal void FireOnAdLoadedEvent(Ad obj) {
			OnAdLoaded?.Invoke(obj);
		}

		internal void FireOnAdRenderedEvent(Ad obj) {
			OnAdRendered?.Invoke(obj);
		}

		internal void FireOnAdImpressionEvent(Ad obj) {
			OnAdImpression?.Invoke(obj);
		}

		internal void FireOnAdClickedEvent(Ad obj) {
			OnAdClicked?.Invoke(obj);
		}

		internal void FireOnAdDestroyedEvent(Ad obj) {
			OnAdDestroyed?.Invoke(obj);
		}

		internal void FireOnVideoAdPausedEvent(Ad obj) {
			OnVideoAdPaused?.Invoke(obj);
		}

		internal void FireOnVideoAdResumeEvent(Ad obj) {
			OnVideoAdResume?.Invoke(obj);
		}

		internal void FireOnAdCompletedEvent(Ad obj, bool skipped) {
			OnAdCompleted?.Invoke(obj, skipped);
		}

		internal void FireOnAdRewardEarnedEvent(Ad obj)
		{
			OnAdRewardEarned?.Invoke(obj);
		}
		
		internal void FireOnAdErrorEvent(Ad obj) {
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
		REWARDEARNED,
		// FIRST_QUARTILE,
		// MIDPOINT,
		// THIRD_QUARTILE,
		COMPLETED,

		// SKIPPED,
		// VOLUME_CHANGED
		DESTROYED
	}
}