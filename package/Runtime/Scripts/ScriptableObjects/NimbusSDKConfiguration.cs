using System;
using System.Linq;
using UnityEngine;

namespace Nimbus.Runtime.Scripts.ScriptableObjects {
	[CreateAssetMenu(fileName = "Nimbus SDK Configuration", menuName = "Nimbus/Create SDK Configuration", order = 0)]
	public class NimbusSDKConfiguration : ScriptableObject {
		[Header("Publisher Credentials")] [Tooltip("Typically your applications name")]
		public string publisherKey;

		[Tooltip("A UUID used to authenticate your requests to Nimbus")]
		public string apiKey;
		
		[Header("SDK Flags")]
		public bool enableSDKInTestMode;

		[Header("Enable Unity Logs")] 
		public bool enableUnityLogs;

		private void OnValidate() {
			if (publisherKey.Trim().Length == 0) throw new Exception("Publisher key cannot be empty");

			if (apiKey.Trim().Length == 0) throw new Exception("Apikey cannot be empty");
		}
	}
}