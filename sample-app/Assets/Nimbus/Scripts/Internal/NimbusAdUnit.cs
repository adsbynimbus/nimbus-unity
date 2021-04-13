using UnityEngine;

namespace Nimbus.Scripts.Internal {
	public sealed class NimbusAdUnit {
		private readonly AdEvents _adEvents;

		public readonly AdUnityType AdType;
		public readonly int InstanceID;
		public readonly string Position;
		internal AdError AdControllerError;
		internal AdError AdListenerError;
		internal bool AdWasRendered;
		internal AdEventTypes CurrentAdState;
		internal MetaData MetaData;

		public NimbusAdUnit(AdUnityType adType, string position, in AdEvents adEvents) {
			AdType = adType;
			InstanceID = GetHashCode();
			CurrentAdState = AdEventTypes.NOT_LOADED;
			Position = position;
			_adEvents = adEvents;
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
			return AdListenerError != null || AdControllerError != null;
		}

		/// <summary>
		///     Returns the combined error output from the ad listener and controller error
		/// </summary>
		public string ErrorMessage() {
			var message = "";
			if (AdListenerError != null) message = $"AdListener Error: {AdListenerError?.Message} ";
			if (AdControllerError != null) message += $"AdController Error: {AdControllerError?.Message}";
			return message;
		}

		/// <summary>
		///     Returns the unique auction id associated to the request to Nimbus, can be used by the Nimbus team to debug
		///     a particular auction event
		/// </summary>
		public string GetAuctionID() {
			return MetaData?.AuctionID ?? "";
		}

		/// <summary>
		///     Return the Ecpm value associated to the winning ad
		/// </summary>
		public double GetBidValue() {
			return MetaData?.Bid ?? 0.0;
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
			return MetaData?.Network ?? "";
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
	internal class AdError {
		public readonly string Message;

		public AdError(string errMessage) {
			Message = errMessage;
		}
	}

	internal class MetaData {
		public readonly string AuctionID;
		public readonly double Bid;
		public readonly string Network;

#if UNITY_ANDROID
		public MetaData(in AndroidJavaObject response) {
			AuctionID = response.Get<string>("auction_id");
			Bid = response.Get<double>("bid_raw");
			Network = response.Get<string>("network");
		}
#endif
	}
}