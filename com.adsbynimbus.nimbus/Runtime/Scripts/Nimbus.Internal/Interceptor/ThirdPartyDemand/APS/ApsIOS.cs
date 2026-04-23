using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.APS {
	#if UNITY_IOS && NIMBUS_ENABLE_APS
	internal class ApsIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;

		private float _timeoutInSeconds = 3.0f;

		public ApsIOS(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public ThirdPartyDemandObj GetConfigObject()
		{
			return new ThirdPartyDemandObj(ThirdPartyDemandEnum.Aps, firstKey:_appID);
		}

		public ApsIOS(string appID, ApsSlotData[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
			_timeoutInSeconds = Math.Clamp(timeoutInMilliseconds, 300, 3000)/1000.0f;
		}
		
		public Tuple<string, string> GetAdUnitId(AdType type, int width, int height)
		{
			var interstitialId1 = "";
			var interstitialId2 = "";
			foreach (ApsSlotData slot in _slotData)
			{
				if (type == AdType.Banner)
				{
					switch (slot.APSAdUnitType)
					{
						case APSAdUnitType.Display320X50:
						{
							if (width == 320 || height == 50)
							{
								return new Tuple<string, string>(slot.SlotId, "");
							}
							break;
						}
						case APSAdUnitType.Display300X250:
						{
							if (width == 300 || height == 250)
							{
								return new Tuple<string, string>(slot.SlotId, "");
							}
							break;
						}
						case APSAdUnitType.Display728X90:
						{
							if (width == 728 || height == 90)
							{
								return new Tuple<string, string>(slot.SlotId, "");
							}
							break;
						}
					}
				} 
				else if (type == AdType.Interstitial)
				{
					switch (slot.APSAdUnitType)
					{
						case APSAdUnitType.InterstitialDisplay:
						{
							interstitialId1 = slot.SlotId;
							break;
						}
						case APSAdUnitType.InterstitialVideo:
						{
							interstitialId2 = slot.SlotId;
							break;
						}
					}
				}
				else
				{
					if (slot.APSAdUnitType == APSAdUnitType.RewardedVideo)
					{
						return new Tuple<string, string>(slot.SlotId, "");
					}
				}

			}
			return new Tuple<string, string>(interstitialId1, interstitialId2);
		}
	}
#endif
}