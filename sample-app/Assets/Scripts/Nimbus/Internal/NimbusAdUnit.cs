using UnityEngine;

namespace Nimbus.Internal {
	public sealed class NimbusAdUnit {
		internal bool AdWasRendered;
		internal AdError AdListenerError;
		internal AdError AdControllerError;
		internal AdEventTypes CurrentAdState;
		
		public readonly AdUnityType AdType;
		public readonly int InstanceID;
		public readonly string Position;
		private readonly AdEvents _adEvents;

		#region Android Objects
		private AndroidJavaObject _androidController;
		private AndroidJavaClass _androidHelper;
		#endregion
		
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
		/// Checks to see of an error was returned from either the ad listener or controller and returns true if there
		/// was an error at any step
		/// </summary>
		public bool DidHaveAnError() {
			return AdListenerError != null || AdControllerError != null;
		}
		
		/// <summary>
		/// Returns the combined error output from the ad listener and controller error
		/// </summary>
		public string ErrorMessage() {
			var message = ""; 
			if (AdListenerError != null) {
				message = $"AdListener Error: {AdListenerError?.Message} ";
			}
			if (AdControllerError != null) {
				message += $"AdController Error: {AdControllerError?.Message}";
			}
			return message;
		}
		
		/// <summary>
		/// Destroys the ad at the mobile bridge level
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
		/// Returns the current state of the ad, this can be used instead of event listeners to execute conditional code
		/// </summary>
		public AdEventTypes GetCurrentAdState() {
			return CurrentAdState;
		}
		
		/// <summary>
		/// Returns returns true of the ad was rendered even if the ad has already been destroyed
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
	}
	
	
	// ReSharper disable MemberCanBePrivate.Global
	internal class AdError {
		public readonly string Message;
		public AdError(string errMessage) {
			Message = errMessage;
		}
	}
	
}


