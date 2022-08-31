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
		[HideInInspector] public bool enableAPS;
		[HideInInspector] public string appID;
		[HideInInspector] public ApsSlotData[] slotData;

		private void OnValidate() {
			Sanitize();
		}

		public void Sanitize() {
			publisherKey = publisherKey?.Trim();
			apiKey = apiKey?.Trim();
			appID = appID?.Trim();

			if (slotData == null) return;

			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < slotData.Length; i++) {
				slotData[i].SlotId = slotData[i].SlotId?.Trim();
			}
		}
	}
}