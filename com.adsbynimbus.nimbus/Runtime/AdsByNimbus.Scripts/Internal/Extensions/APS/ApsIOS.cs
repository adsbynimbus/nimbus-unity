using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdsByNimbus.Scripts;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Internal.Extensions.APS {
	#if UNITY_IOS && NIMBUS_ENABLE_APS
	internal class ApsIOS {
		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;


		public ApsIOS(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}


		public ApsIOS(string appID, ApsSlotData[] slotData, bool enableTestMode) {
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
		}
		
		public ApsSlotData[] GetAdUnitId(AdType type, int width, int height)
		{
			var slotData = new List<ApsSlotData>();
			foreach (ApsSlotData slot in _slotData)
			{
				if (type == AdType.Inline)
				{
					switch (slot.adUnitType)
					{
						case APSAdUnitType.Display320X50:
						{
							if (width == 320 || height == 50)
							{
								slotData.Add(slot);
							}
							break;
						}
						case APSAdUnitType.Display300X250:
						{
							if (width == 300 || height == 250)
							{
								slotData.Add(slot);
							}
							break;
						}
						case APSAdUnitType.Display728X90:
						{
							if (width == 728 || height == 90)
							{
								slotData.Add(slot);
							}
							break;
						}
					}
				} 
				else if (type == AdType.Fullscreen)
				{
					switch (slot.adUnitType)
					{
						case APSAdUnitType.InterstitialDisplay:
						{
							slotData.Add(slot);
							break;
						}
						case APSAdUnitType.InterstitialVideo:
						{
							slotData.Add(slot);
							break;
						}
					}
				}
				else
				{
					if (slot.adUnitType == APSAdUnitType.RewardedVideo)
					{
						slotData.Add(slot);
					}
				}

			}
			return slotData.ToArray();
		}
	}
#endif
}