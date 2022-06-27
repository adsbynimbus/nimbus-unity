using System;

namespace Nimbus.Internal.RequestBuilder {
	public enum IabSupportedAdSizes : byte {
		Banner300X50,
		Banner320X50,
		FullScreenPortrait,
		FullScreenLandscape,
		HalfScreen,
		Letterbox,
		LeaderBoard
	}

	public static class IabSupportedAdSizesExtension {
		public static Tuple<int, int> ToWidthAndHeight(this IabSupportedAdSizes isa) {
			switch (isa) {
				case IabSupportedAdSizes.Banner300X50:
					return new Tuple<int, int>(300, 50);
				case IabSupportedAdSizes.Banner320X50:
					return new Tuple<int, int>(320, 50);
				case IabSupportedAdSizes.FullScreenPortrait:
					return new Tuple<int, int>(320, 480);
				case IabSupportedAdSizes.FullScreenLandscape:
					return new Tuple<int, int>(480, 320);
				case IabSupportedAdSizes.HalfScreen:
					return new Tuple<int, int>(300, 600);
				case IabSupportedAdSizes.Letterbox:
					return new Tuple<int, int>(300, 250);
				case IabSupportedAdSizes.LeaderBoard:
					return new Tuple<int, int>(728, 90);
				default:
					return new Tuple<int, int>(0, 0);
			}
		}
	}
}