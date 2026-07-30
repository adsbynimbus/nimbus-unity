using System;
using UnityEngine;

namespace Internal {
	internal static class NimbusCallbackParser {
		internal static T ParseMessage<T>(string jsonParams) {
			return JsonUtility.FromJson<T>(jsonParams);
		}
	}

	[Serializable]
	internal class NimbusEventParams {
		public int adUnitInstanceID;
	}

	[Serializable]
	internal class NimbusErrorData : NimbusEventParams {
		public string errorMessage;
	}

	[Serializable]
	internal class NimbusAdEventData : NimbusEventParams {
		public string eventName;
	}
}