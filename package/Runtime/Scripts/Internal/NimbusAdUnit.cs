using UnityEngine;

namespace Nimbus.Runtime.Scripts.Internal {

	public delegate void DestroyAdDelegate();

	public sealed class NimbusAdUnit {
		private readonly AdEvents _adEvents;
		public readonly AdUnityType AdType;
		
		public readonly int InstanceID;
		public readonly string Position;
		public MetaData ResponseMetaData;

		internal AdError AdControllerError;
		internal AdError AdListenerError;
		internal bool AdWasRendered;
		internal BidFloors BidFloors;
		internal AdEventTypes CurrentAdState;
		
		// Delay before close button is shown in milliseconds, set to max value to only show close button after video completion
		// where setting a value higher than the video length forces the x to show up at the end of the video
		internal readonly int CloseButtonDelayMillis;
		# region IOS specific
		internal event DestroyAdDelegate DestroyIOSAd;
		private void OnDestroyIOSAd() {
			DestroyIOSAd?.Invoke();
		}
		
		#endregion
		
		#region Android Specific

		private AndroidJavaObject _androidController;
		private AndroidJavaClass _androidHelper;

		#endregion
		
		public NimbusAdUnit(AdUnityType adType, string position, float bannerFloor, float videoFloor,
			in AdEvents adEvents) {
			AdType = adType;
			InstanceID = GetHashCode();
			CurrentAdState = AdEventTypes.NOT_LOADED;
			Position = position;
			_adEvents = adEvents;
			BidFloors = new BidFloors(bannerFloor, videoFloor);
			// leave this at MaxValue for now
			CloseButtonDelayMillis = int.MaxValue;
		}

		~NimbusAdUnit() {
			Destroy();
		}

		/// <summary>
		///     Destroys the ad at the mobile bridge level
		/// </summary>
		public void Destroy() {
#if UNITY_ANDROID
			if (_androidController == null || _androidHelper == null) return;
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_androidHelper.CallStatic("destroyController", currentActivity, _androidController);
			_androidController = null;
			_androidHelper = null;
# elif UNITY_IOS
			OnDestroyIOSAd();
#endif
		}

		/// <summary>
		///     Checks to see of an error was returned from either the ad listener or controller and returns true if there
		///     was an error at any step
		/// </summary>
		public bool DidHaveAnError() {
			// TODO capture errors for IOS
			return AdListenerError != null || AdControllerError != null;
		}

		/// <summary>
		///     Returns the combined error output from the ad listener and controller error
		/// </summary>
		public string ErrorMessage() {
			// TODO capture errors for IOS
			var message = "";
			if (AdListenerError != null) message = $"AdListener Error: {AdListenerError.Message} ";
			if (AdControllerError != null) message += $"AdController Error: {AdControllerError.Message}";
			return message;
		}
		
		/// <summary>
		///     Returns the current state of the ad, this can be used instead of event listeners to execute conditional code
		/// </summary>
		public AdEventTypes GetCurrentAdState() {
			return CurrentAdState;
		}
		
		/// <summary>
		///     Returns returns true of the ad was rendered even if the ad has already been destroyed
		/// </summary>
		public bool WasAdRendered() {
			return AdWasRendered;
		}

		internal void EmitOnAdRendered(NimbusAdUnit obj) {
			_adEvents.EmitOnAdRendered(obj);
		}

		internal void EmitOnAdError(NimbusAdUnit obj) {
			_adEvents.EmitOnAdError(obj);
		}

		internal void EmitOnAdEvent(AdEventTypes e) {
			switch (e) {
				case AdEventTypes.LOADED:
					_adEvents.EmitOnOnAdLoaded(this);
					break;
				case AdEventTypes.IMPRESSION:
					_adEvents.EmitOnOnAdImpression(this);
					break;
				case AdEventTypes.CLICKED:
					_adEvents.EmitOnOnAdClicked(this);
					break;
				case AdEventTypes.PAUSED:
					_adEvents.EmitOnOnVideoAdPaused(this);
					break;
				case AdEventTypes.RESUME:
					_adEvents.EmitOnOnVideoAdResume(this);
					break;
				case AdEventTypes.COMPLETED:
					_adEvents.EmitOnOnVideoAdCompleted(this);
					break;
				case AdEventTypes.SKIPPED:
					if (AdType == AdUnityType.Rewarded) {
					    _adEvents.EmitOnOnVideoAdSkipped(this);
					}
					break;
				case AdEventTypes.DESTROYED:
					_adEvents.EmitOnOnAdDestroyed(this);
					break;
			}
		}

		internal void SetAndroidController(AndroidJavaObject controller) {
			if (_androidController != null) return;
			_androidController = controller;
		}

		internal void SetAndroidHelper(AndroidJavaClass helper) {
			if (_androidHelper != null) return;
			_androidHelper = helper;
		}
	}


	// ReSharper disable MemberCanBePrivate.Global
	internal class AdError {
		public readonly string Message;

		public AdError(string errMessage) {
			Message = errMessage;
		}
	}

	internal readonly struct BidFloors {
		internal readonly float BannerFloor;
		internal readonly float VideoFloor;

		public BidFloors(float bannerFloor, float videoFloor) {
			BannerFloor = bannerFloor;
			VideoFloor = videoFloor;
		}
	}

	// ReSharper disable IdentifierTypo
	// ReSharper disable StringLiteralTypo
	public class MetaData {
		/// <summary>
		///     Returns the nimbus auction id, used for debugging
		/// </summary>
		public readonly string AuctionID;
		/// <summary>
		///     Returns returns the network bid as a floating integer 
		/// </summary>
		public readonly double BidRaw;
		/// <summary>
		///     Returns returns the network bid as a integer converted from dollars to cents 
		/// </summary>
		public readonly int BidInCents;
		/// <summary>
		///     Returns the name of the winning network
		/// </summary>
		public readonly string Network;
		/// <summary>
		///     Returns the winning network's placement id
		/// </summary>
		public readonly string PlacementID;
		// TODO pull in response data from IOS
		internal MetaData(in AndroidJavaObject response) {
			AuctionID = response.Get<string>("auction_id");
			BidRaw = response.Get<double>("bid_raw");
			BidInCents = response.Get<int>("bid_in_cents");
			Network = response.Get<string>("network");
			PlacementID = response.Get<string>("placement_id");
		}
	}
}