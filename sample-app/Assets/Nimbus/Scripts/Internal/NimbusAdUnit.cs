using UnityEngine;

namespace Nimbus.Scripts.Internal {
	public sealed class NimbusAdUnit {
		public readonly AdUnityType AdType;
		public readonly int InstanceID;
		public readonly string Position;

		internal AdError AdControllerError;
		internal AdError AdListenerError;
		internal bool AdWasRendered;
		internal BidFloors BidFloors;
		internal AdEventTypes CurrentAdState;
		internal MetaData MetaData;
		
		// Delay before close button is shown in milliseconds, set to max value to only show close button after video completion
		// where setting a value higher than the video length forces the x to show up at the end of the video
		internal readonly int CloseButtonDelayMillis;
		
		private readonly AdEvents _adEvents;
		
		

		public NimbusAdUnit(AdUnityType adType, string position, float bannerFloor, float videoFloor, in AdEvents adEvents) {
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
#endif
		}

		/// <summary>
		///     Checks to see of an error was returned from either the ad listener or controller and returns true if there
		///     was an error at any step
		/// </summary>
		public bool DidHaveAnError() {
			return AdListenerError.Message.Length > 0 || AdControllerError.Message.Length > 0;
		}

		/// <summary>
		///     Returns the combined error output from the ad listener and controller error
		/// </summary>
		public string ErrorMessage() {
			var message = "";
			if (AdListenerError.Message.Length > 0) message = $"AdListener Error: {AdListenerError.Message} ";
			if (AdControllerError.Message.Length > 0) message += $"AdController Error: {AdControllerError.Message}";
			return message;
		}

		/// <summary>
		///     Returns the unique auction id associated to the request to Nimbus, can be used by the Nimbus team to debug
		///     a particular auction event
		/// </summary>
		public string GetAuctionID() {
			return MetaData.AuctionID;
		}

		/// <summary>
		///     Return the Ecpm value associated to the winning ad
		/// </summary>
		public double GetBidValue() {
			return MetaData.Bid;
		}

		/// <summary>
		///     Returns the current state of the ad, this can be used instead of event listeners to execute conditional code
		/// </summary>
		public AdEventTypes GetCurrentAdState() {
			return CurrentAdState;
		}

		/// <summary>
		///     Returns the name of the demand source that won the auction
		/// </summary>
		public string GetNetwork() {
			return MetaData.Network;
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

		internal void EmitOnAdEvent(NimbusAdUnit obj) {
			_adEvents.EmitOnAdEvent(obj);
		}

		internal void SetAndroidController(AndroidJavaObject controller) {
			if (_androidController != null) return;
			_androidController = controller;
		}

		internal void SetAndroidHelper(AndroidJavaClass helper) {
			if (_androidHelper != null) return;
			_androidHelper = helper;
		}

		#region Android Objects

		private AndroidJavaObject _androidController;
		private AndroidJavaClass _androidHelper;

		#endregion
	}


	// ReSharper disable MemberCanBePrivate.Global
	internal readonly struct AdError {
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
	
	internal readonly struct MetaData {
		public readonly string AuctionID;
		public readonly double Bid;
		public readonly string Network;
		
		public MetaData(in AndroidJavaObject response) {
			AuctionID = response.Get<string>("auction_id");
			Bid = response.Get<double>("bid_raw");
			Network = response.Get<string>("network");
		}
	}
}