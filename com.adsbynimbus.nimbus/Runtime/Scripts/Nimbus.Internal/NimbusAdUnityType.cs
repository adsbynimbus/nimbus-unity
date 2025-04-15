using Nimbus.Internal.RequestBuilder;
using Nimbus.Internal.Utility;
using OpenRTB.Request;

namespace Nimbus.Internal {
	/// <summary>
	///     Specifies the type of ad that is being constructed
	/// </summary>
	public enum AdUnitType : byte {
		Undefined = 0,
		Banner = 1,
		Interstitial = 2,
		Rewarded = 3,
		Banner320X50 = 4,
		Banner300X250 = 5,
		Banner728X90 = 6,
		InterstitialDisplay = 7,
		InterstitialVideo = 8
	}


	internal static class AdUnitHelper {
		private const int MRecArea = 75000;

		public static AdUnitType BidRequestToAdType(BidRequest bidRequest) {
			if (bidRequest.Imp.IsNullOrEmpty()) return AdUnitType.Undefined;
			// lets determine if this is a hybrid auction, if so this is an Interstitial unit
			if (bidRequest.IsHybridBidRequest()) return AdUnitType.Interstitial;

			if (bidRequest.Imp[0].Banner != null) {
				var w = bidRequest.Imp[0].Banner.W;
				var h = bidRequest.Imp[0].Banner.H;
				// TODO this makes a poor assumption that 300x250 will be part of blocking ads
				return w * h >= MRecArea ? AdUnitType.Interstitial : AdUnitType.Banner;
			}

			return bidRequest.Imp[0].Video != null ? AdUnitType.Rewarded : AdUnitType.Undefined;
		}
		
		public static bool IsAdTypeFullScreen(AdUnitType adUnitType) {
			// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
			switch (adUnitType) {
				case AdUnitType.Interstitial: case AdUnitType.Rewarded:
					return true;
			}
			return false;
		}
		
	}
}