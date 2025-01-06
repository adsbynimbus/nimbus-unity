using System;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using UnityEngine;

namespace Nimbus.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[HideInInspector] public string publisherKey;
		[HideInInspector] public string apiKey;
		[HideInInspector] public bool enableSDKInTestMode;
		[HideInInspector] public bool enableUnityLogs;

		// APS data
		[HideInInspector] public string androidAppID;
		[HideInInspector] public ApsSlotData[] androidApsSlotData;
		
		[HideInInspector] public string iosAppID;
		[HideInInspector] public ApsSlotData[] iosApsSlotData;
		
		// Vungle Data
		[HideInInspector] public string androidVungleAppID;
		[HideInInspector] public string iosVungleAppID;

		// Meta Data
		[HideInInspector] public string androidMetaAppID;
		[HideInInspector] public string iosMetaAppID;
		[HideInInspector] public bool iosMetaAdvertiserTrackingEnabled = false;
		
		private void OnValidate() {
			Sanitize();
		}

		public void Sanitize() {
			publisherKey = publisherKey?.Trim();
			apiKey = apiKey?.Trim();
			#if NIMBUS_ENABLE_APS
				androidAppID = androidAppID?.Trim();
				iosAppID = iosAppID?.Trim();

				// ReSharper disable ForCanBeConvertedToForeach
				// ReSharper disable InvertIf
				if (androidApsSlotData != null) {
					for (var i = 0; i < androidApsSlotData.Length; i++) {
						androidApsSlotData[i].SlotId = androidApsSlotData[i].SlotId?.Trim();
					}
				}
				
				if (iosApsSlotData != null) {
					for (var i = 0; i < iosApsSlotData.Length; i++) {
						iosApsSlotData[i].SlotId = iosApsSlotData[i].SlotId?.Trim();
					}
				}
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				androidVungleAppID = androidVungleAppID?.Trim();
				iosVungleAppID = iosVungleAppID?.Trim();
			#endif
			
			#if NIMBUS_ENABLE_META
				androidMetaAppID = androidMetaAppID?.Trim();
				iosMetaAppID = iosMetaAppID?.Trim();
			#endif
		}


		public Tuple<string, ApsSlotData[]> GetApsData() {
			var appID = androidAppID;
			var slots = androidApsSlotData;
			#if UNITY_IOS
				appID = iosAppID;
				slots =  iosApsSlotData;
			#endif
			return new Tuple<string, ApsSlotData[]>(appID, slots);
		}

		public string GetVungleData()
		{
			var appID = androidVungleAppID;
			#if UNITY_IOS
				appID = iosVungleAppID;
			#endif
			return appID;
		}
		
		public string GetMetaData()
		{
			var appID = androidMetaAppID;
		#if UNITY_IOS
			appID = iosMetaAppID;
		#endif
			return appID;
		}
	}
}