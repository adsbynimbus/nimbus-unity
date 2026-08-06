using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdsByNimbus;
using AdsByNimbus.Extensions;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace AdsByNimbus.Internal.Extensions.APS {
	internal class ApsAndroid {

		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly apsAd[] _slotData;
		private readonly AndroidJavaObject _currentActivity;
		private AndroidJavaClass _aps;
		
		public ApsAndroid(string appID, apsAd[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public ApsAndroid(AndroidJavaObject currentActivity, string appID, apsAd[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_currentActivity = currentActivity;
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
		}
		
		public apsAd[] GetAdUnitId(AdType type, int width, int height)
		{
			var slotData = new List<apsAd>();
			foreach (apsAd slot in _slotData)
			{
				if (type == AdType.Inline)
				{
					switch (slot.AdFormat)
					{
						case APSAdFormat.Display320X50:
						{
							if (width == 320 || height == 50)
							{
								slotData.Add(slot);
							}
							break;
						}
						case APSAdFormat.Display300X250:
						{
							if (width == 300 || height == 250)
							{
								slotData.Add(slot);
							}
							break;
						}
						case APSAdFormat.Display728X90:
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
					switch (slot.AdFormat)
					{
						case APSAdFormat.InterstitialDisplay:
						{
							slotData.Add(slot);
							break;
						}
						case APSAdFormat.InterstitialVideo:
						{
							slotData.Add(slot);
							break;
						}
					}
				}
				else
				{
					if (slot.AdFormat == APSAdFormat.RewardedVideo)
					{
						slotData.Add(slot);
					}
				}

			}
			return slotData.ToArray();
		}
	}
}