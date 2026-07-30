using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("nimbus.test")]

namespace Internal.Extensions.AdMob {
	#if UNITY_IOS && NIMBUS_ENABLE_ADMOB
	
	internal class AdMobIOS {
		private readonly AdMobAdUnit[] _adUnitIds;

		public string[] GetAdUnitId(AdType type)
		{
			var ids = new List<string>();
			foreach (AdMobAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					ids.Add(adUnit.AdUnitId);
				}
			}
			return ids.ToArray();
		}

		public AdMobIOS(AdMobAdUnit[] adUnitIds) {
			_adUnitIds = adUnitIds;
		}

	}
#endif
}