using System;
using UnityEngine;

namespace Nimbus.Internal {
	internal static class NimbusIOSParser {
		internal static T ParseMessage<T>(string jsonParams) {
			return JsonUtility.FromJson<T>(jsonParams);
		}
	}

	[Serializable]
	internal class NimbusIOSParams {
		public int adUnitInstanceID;
	}

	[Serializable]
	internal class NimbusIOSErrorData : NimbusIOSParams {
		public string errorMessage;
	}

	[Serializable]
	internal class NimbusIOSAdEventData : NimbusIOSParams {
		public string eventName;
	}
}