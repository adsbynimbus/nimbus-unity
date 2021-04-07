using UnityEditor;
using UnityEngine;

namespace Nimbus.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[Header("Publisher Credentials")]
		public string publisherKey;
		public string apiKey;
		[Header("Application Data")]
		public string appName;
		public string appStoreURL;
		[Header("SDK Flags")]
		public bool enableSDKInTestMode;
		[Header("Enable Unity Logs")]
		public bool enableUnityLogs;
	}
}