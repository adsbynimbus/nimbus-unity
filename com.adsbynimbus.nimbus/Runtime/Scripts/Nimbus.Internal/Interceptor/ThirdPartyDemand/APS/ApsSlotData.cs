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

	public class APSHelper
	{
		public static Tuple<int, int> AdTypeToDim(APSAdUnitType type) {
			switch (type) {
				case APSAdUnitType.Display320X50:
					return new Tuple<int, int>(320, 50);
				case APSAdUnitType.Display300X250:
					return new Tuple<int, int>(300, 250);
				case APSAdUnitType.Display728X90:
					return new Tuple<int, int>(728, 90);
				case APSAdUnitType.InterstitialDisplay:
					return new Tuple<int, int>(320, 480);
				case APSAdUnitType.InterstitialVideo:
					return new Tuple<int, int>(Screen.width, Screen.height);
				case APSAdUnitType.RewardedVideo:
					return new Tuple<int, int>(Screen.width, Screen.height);
				default:
					return new Tuple<int, int>(0, 0);
			}
		}
	}
}