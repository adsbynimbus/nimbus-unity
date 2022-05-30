using UnityEngine;

namespace Nimbus.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[Header("Publisher Credentials")]
		[Tooltip("Ask your account manager")]
		public string publisherKey;
		[Tooltip("Ask your account manager")]
		public string apiKey;
		
		[Header("SDK Flags")] 
		public bool enableSDKInTestMode;

		[Header("Enable Unity Logs")] 
		public bool enableUnityLogs;
		
		private void OnValidate() {
			publisherKey = publisherKey.Trim();
			apiKey = apiKey.Trim();
		}
	}
}