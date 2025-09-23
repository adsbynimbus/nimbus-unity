using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.RequestBuilder {
	public static class NimbusRtbBidRequestHelper {
		public static BidRequest ForHybridInterstitialAd(string reportingPosition) {
			var bidRequest = ForStaticInterstitialAd(reportingPosition);
			bidRequest.Imp[0].Video = new Video().Interstitial();
			return bidRequest;
		}

		public static BidRequest ForStaticInterstitialAd(string reportingPosition) {
			var impression = new[] {
				new Imp {
					Banner = new Banner().Interstitial(),
					Instl = 1,
					Secure = 1,
					Ext = new ImpExt {
						Position = reportingPosition
					}
				}
			};
			return new BidRequest {
				Imp = impression,
				Format = new Format {
					W = Screen.width,
					H = Screen.height
				}
			};
		}

		public static BidRequest ForBannerAd(string reportingPosition, IabSupportedAdSizes adUnitSize = IabSupportedAdSizes.Banner320X50) {
			var impression = new[] {
				new Imp {
					Banner = new Banner().SetupDefaults((adUnitSize == IabSupportedAdSizes.LeaderBoard) ? adUnitSize : IabSupportedAdSizes.Banner320X50, Position.Footer, 0f),
					Instl = 0,
					Secure = 1,
					Ext = new ImpExt {
						Position = reportingPosition
					}
				}
			};
			return new BidRequest {
				Imp = impression,
				Format = new Format {
					W = Screen.width,
					H = Screen.height
				}
			};
		}

		public static BidRequest ForVideoInterstitialAd(string position) {
			var impression = new[] {
				new Imp {
					Video = new Video().Interstitial(),
					Instl = 1,
					Secure = 1,
					Ext = new ImpExt {
						Position = position
					}
				}
			};
			return new BidRequest {
				Imp = impression,
				Format = new Format {
					W = Screen.width,
					H = Screen.height
				}
			};
		}
	}
}