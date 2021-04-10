using UnityEngine;

namespace Nimbus.Internal {
	public sealed class NimbusAdUnit {
		internal bool AdWasRendered;
		internal AdError AdListenerError;
		internal AdError AdControllerError;
		internal AdEventTypes CurrentAdState;
		
		public readonly AdUnityType AdType;
		public readonly int InstanceID;
		private readonly AdEvents _adEvents;

		#region Android Objects
		private AndroidJavaObject _androidController;
		private AndroidJavaClass _androidHelper;
		#endregion
		
		public NimbusAdUnit(AdUnityType adType, in AdEvents adEvents) {
			AdType = adType;
			InstanceID = GetHashCode();
			CurrentAdState = AdEventTypes.NOT_LOADED;
			_adEvents = adEvents;
		}

		~NimbusAdUnit() {
			Destroy();
		}

		public bool DidHaveAnError() {
			return AdListenerError != null || AdControllerError != null;
		}

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

		public void EmitOnAdRendered(NimbusAdUnit obj) {
			_adEvents.EmitOnAdRendered(obj);
		}
		
		public void EmitOnAdError(NimbusAdUnit obj) {
			_adEvents.EmitOnAdError(obj);
		}
		
		public void EmitOnAdEvent(NimbusAdUnit obj) {
			_adEvents.EmitOnAdEvent(obj);
		}

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

		public AdEventTypes GetCurrentAdState() {
			return CurrentAdState;
		}
		
		public bool WasAdRendered() {
			return AdWasRendered;
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
	public class AdError {
		public readonly string Message;
		public AdError(string errMessage) {
			Message = errMessage;
		}
	}
	
}


