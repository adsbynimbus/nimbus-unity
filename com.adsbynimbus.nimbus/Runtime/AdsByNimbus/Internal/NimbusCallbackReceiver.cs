using System;
using System.Collections.Generic;
using AdsByNimbus;
using UnityEngine;

namespace AdsByNimbus.Internal {
	internal class NimbusCallbackReceiver : MonoBehaviour {
		private static NimbusCallbackReceiver _instance;

		private readonly Dictionary<int, Ad> _adUnitDictionary = new Dictionary<int, Ad>();

		internal static NimbusCallbackReceiver Instance {
			get {
				if (_instance != null) return _instance;
				
				var obj = new GameObject("NimbusCallbackReceiver");
				_instance = (NimbusCallbackReceiver)obj.AddComponent(typeof(NimbusCallbackReceiver));
				return _instance;
			}
		}

		private void Awake() {
			if (_instance != null) {
				Destroy(gameObject);
				return;
			}
			DontDestroyOnLoad(gameObject);
		}

		internal void AddAdUnit(Ad adUnit) { 
			_adUnitDictionary.Add(adUnit.InstanceID, adUnit);
		}
		
		internal Ad AdUnitForInstanceID(int instanceID) {
			_adUnitDictionary.TryGetValue(instanceID, out var adUnit);
			return adUnit;
		}
		
		internal void OnAdRendered(string jsonParams) {
			var data = NimbusCallbackParser.ParseMessage<NimbusEventParams>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);

			if (adUnit == null && data.adUnitInstanceID != -1) {
				Debug.unityLogger.LogError("NimbusError", "AdUnit not found: " + data.adUnitInstanceID);
				return;
			}
			adUnit.FireMobileAdRenderedEvent();
		}
		
		internal void OnAdEvent(string jsonParams) {
			var data = NimbusCallbackParser.ParseMessage<NimbusAdEventData>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);

			if (adUnit == null && data.adUnitInstanceID != -1) {
				Debug.unityLogger.LogError("NimbusError", $"AdUnit not found: {data.adUnitInstanceID}");
				return;
			}

			if (!Enum.TryParse(data.eventName, out AdEvent state)) return;
			adUnit.FireMobileAdEvents(state);
			// clean up internal map
			if (state == AdEvent.DESTROYED) {
				_adUnitDictionary.Remove(data.adUnitInstanceID);
			}
		}

		internal void OnError(string jsonParams) {
			var data = NimbusCallbackParser.ParseMessage<NimbusErrorData>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);

			if (adUnit == null && data.adUnitInstanceID != -1) {
				Debug.unityLogger.LogError("NimbusError", $"AdUnit not found: {data.adUnitInstanceID}");
				return;
			}

			Debug.unityLogger.LogError("NimbusError", $"Listener Ad error: {data.adUnitInstanceID}");
			adUnit.FireMobileOnAdErrorEvent();
		}
	}
}