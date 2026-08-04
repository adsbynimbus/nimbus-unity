using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdsByNimbus.Scripts;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Internal.Extensions.APS {
	internal class ApsAndroid {
		private const string AndroidApsPackage = "com.adsbynimbus.request.ApsDemandProvider";

		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;
		private readonly AndroidJavaObject _currentActivity;
		private AndroidJavaClass _aps;
		
		public ApsAndroid(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public ApsAndroid(AndroidJavaObject currentActivity, string appID, ApsSlotData[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_currentActivity = currentActivity;
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
}