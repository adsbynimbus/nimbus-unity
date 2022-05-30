using System;
using UnityEngine;
// TODO can probably remove this entire file
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
	internal class NimbusIOSAdResponse : NimbusIOSParams {
		public string auctionId;
		public int bidRaw;
		public int bidInCents;
		public string network;
		public string placementId;
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