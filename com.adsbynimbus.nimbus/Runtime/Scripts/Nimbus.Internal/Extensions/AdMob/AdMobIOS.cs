using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("nimbus.test")]

namespace Nimbus.Internal.Extensions.AdMob {
	#if UNITY_IOS && NIMBUS_ENABLE_ADMOB
	
	
	internal class AdMobIOS {
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private readonly bool _autoInit;
		
		[DllImport("__Internal")]
		private static extern void _initializeAdMob();

		public string[] GetAdUnitId(AdType type)
		{
			var ids = new List<string>();
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					ids.Add(adUnit.AdUnitId);
				}
			}
			return ids.ToArray();
		}

		public AdMobIOS(ThirdPartyAdUnit[] adUnitIds) {
			_adUnitIds = adUnitIds;
		}

		public static void ManuallyInitAdMob()
		{
			_initializeAdMob();
		}

	}
#endif
}