using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable CheckNamespace
namespace Nimbus.Runtime.Scripts.Internal {
	internal class NimbusIOSAdManager : MonoBehaviour {
		
		private static NimbusIOSAdManager _instance;
		
		private readonly Dictionary<int, NimbusAdUnit> _adUnitDictionary = new Dictionary<int, NimbusAdUnit>();

		internal static NimbusIOSAdManager Instance {
			get {
				if (_instance != null) return _instance;
				var obj = new GameObject("NimbusIOSAdManager");
				_instance = (NimbusIOSAdManager) obj.AddComponent(typeof(NimbusIOSAdManager));
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
		
		internal void AddAdUnit(NimbusAdUnit adUnit)
		{
			_adUnitDictionary.Add(adUnit.InstanceID, adUnit);
		}

		private NimbusAdUnit AdUnitForInstanceID(int instanceID)
		{
			_adUnitDictionary.TryGetValue(instanceID, out var adUnit);
			return adUnit;
		}

		#region iOS Event Callbacks

		private void OnAdResponse(string jsonParams) {
			var data = NimbusIOSParser.ParseMessage<NimbusIOSAdResponse>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);
			
			if (adUnit == null)
			{
				Debug.unityLogger.LogError("NimbusError", "AdUnit not found: " + data.adUnitInstanceID);
				return;
			}
			
			adUnit.ResponseMetaData = new MetaData(data);
		}

		internal void OnAdRendered(string jsonParams)
		{
			var data = NimbusIOSParser.ParseMessage<NimbusIOSParams>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);

			if (adUnit == null)
			{
				Debug.unityLogger.LogError("NimbusError", "AdUnit not found: " + data.adUnitInstanceID);
				return;
			}
            
			adUnit.AdWasRendered = true;
			adUnit.EmitOnAdRendered(adUnit);
		}

		internal void OnError(string jsonParams) {
			var data = NimbusIOSParser.ParseMessage<NimbusIOSErrorData>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);
            
			if (adUnit == null)
			{
				Debug.unityLogger.LogError("NimbusError", "AdUnit not found: " + data.adUnitInstanceID);
				return;
			}
            
			Debug.unityLogger.LogError("NimbusError", "Listener Ad error: " + data.errorMessage);
			adUnit.AdListenerError = new AdError(data.errorMessage);
			adUnit.EmitOnAdError(adUnit);
		}

		internal void OnAdEvent(string jsonParams) {
			var data = NimbusIOSParser.ParseMessage<NimbusIOSAdEventData>(jsonParams);
			var adUnit = AdUnitForInstanceID(data.adUnitInstanceID);
            
			if (adUnit == null)
			{
				Debug.unityLogger.LogError("NimbusError", "AdUnit not found: " + data.adUnitInstanceID);
				return;
			}
            
			var eventType = (AdEventTypes)Enum.Parse(typeof(AdEventTypes), data.eventName, true);
			adUnit.EmitOnAdEvent(eventType);
		}

		#endregion
	}
}
