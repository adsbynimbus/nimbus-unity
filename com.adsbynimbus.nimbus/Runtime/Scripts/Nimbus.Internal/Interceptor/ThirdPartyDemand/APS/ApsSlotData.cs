using System;
using UnityEngine;

namespace Nimbus.Internal.Interceptor.ThirdPartyDemand {
	[Serializable]
	public class ApsSlotData {
		public string SlotId;
		public APSAdUnitType APSAdUnitType;
	}
	public enum APSAdUnitType : byte {
		Display320X50 = 0,
		Display300X250 = 1,
		Display728X90 = 2,
		InterstitialDisplay = 3,
		InterstitialVideo = 4,
		RewardedVideo = 5
	}
}