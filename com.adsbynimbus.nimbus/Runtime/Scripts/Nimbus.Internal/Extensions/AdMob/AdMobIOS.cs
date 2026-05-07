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

		public string GetAdUnitId(AdType type)
		{
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					return adUnit.AdUnitId;
				}
			}
			return "";
		}

		public AdMobIOS(ThirdPartyAdUnit[] adUnitIds, bool autoInit) {
			_adUnitIds = adUnitIds;
			_autoInit = autoInit;
		}

		public static void ManuallyInitAdMob()
		{
			_initializeAdMob();
		}

	}
#endif
}