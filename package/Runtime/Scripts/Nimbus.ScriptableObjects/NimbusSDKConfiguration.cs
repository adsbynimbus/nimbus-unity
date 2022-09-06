using Nimbus.Internal.ThirdPartyDemandProviders;
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
		}
	}
}