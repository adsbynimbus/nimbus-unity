using System;
using JetBrains.Annotations;
using UnityEngine;


namespace Nimbus.Internal {
	public sealed class NimbusAdUnit {
		public bool AdWasRendered;
		public AdError AdListenerError;
		public AdError AdControllerError;
		public AdEventTypes CurrentAdState;

		public readonly AdUnityType AdType;
		public readonly int InstanceID;
		private readonly AdEvents _adEvents;
		private AndroidJavaObject _androidController;

		public NimbusAdUnit(AdUnityType adType, ref AdEvents adEvents) {
			this.AdType = adType;
			_adEvents = adEvents;
			InstanceID = GetHashCode();
			CurrentAdState = AdEventTypes.NOT_LOADED;
		}

		public bool DidHaveAnError() {
			return AdListenerError != null || AdControllerError != null;
		}

		public string ErrorMessage() {
			return $"AdListener Error: {AdListenerError?.Message} AdController Error: {AdControllerError?.Message}";
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
			if (_androidController == null) return;
			_androidController.Call("destroy");
			_androidController = null;
		}

		public void SetAndroidController(ref AndroidJavaObject controller) {
			_androidController = controller;
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


