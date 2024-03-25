using System;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.RequestBuilder {
	public static class BidRequestExtension {
		public static BidRequest SetReportingPosition(this BidRequest bidRequest, string reportingPosition) {
			if (bidRequest.Imp.IsNullOrEmpty()) return bidRequest;

			if (bidRequest.Imp[0].Ext == null) {
				bidRequest.Imp[0].Ext = new ImpExt {
					Position = reportingPosition
				};
				return bidRequest;
			}

			bidRequest.Imp[0].Ext.Position = reportingPosition;
			return bidRequest;
		}

		public static BidRequest SetSessionId(this BidRequest bidRequest, string sessionID) {
			if (bidRequest.Ext == null) {
				bidRequest.Ext = new BidRequestExt {
					SessionId = sessionID
				};
				return bidRequest;
			}

			bidRequest.Ext.SessionId = sessionID;
			return bidRequest;
		}

		public static BidRequest SetAppBundle(this BidRequest bidRequest, string bundle) {
			if (bidRequest.App == null) {
				bidRequest.App = new App {
					Bundle = bundle
				};
				return bidRequest;
			}

			bidRequest.App.Bundle = bundle;
			return bidRequest;
		}

		public static BidRequest SetAppName(this BidRequest bidRequest, string name) {
			if (bidRequest.App == null) {
				bidRequest.App = new App {
					Name = name
				};
				return bidRequest;
			}

			bidRequest.App.Name = name;
			return bidRequest;
		}
		
		
		public static BidRequest SetCoppa(this BidRequest bidRequest, bool coppa) {
			if (bidRequest.Regs == null) {
				bidRequest.Regs = new Regs {
					Coppa = coppa ? 1: 0
				};
				return bidRequest;
			}
			bidRequest.Regs.Coppa = coppa ? 1: 0;
			return bidRequest;
		}
		
		public static BidRequest SetDevice(this BidRequest bidRequest, Device device) {
			bidRequest.Device = device;
			return bidRequest;
		}
		
		public static BidRequest SetUsPrivacy(this BidRequest bidRequest, string usPrivacyString) {
			bidRequest.Regs ??= new Regs { };
			bidRequest.Regs.Ext ??= new RegExt {
				UsPrivacy = usPrivacyString
			};
			return bidRequest;
		}
		
		public static BidRequest SetGdprConsentString(this BidRequest bidRequest, string consentString) {
			bidRequest.User ??= new User();
			bidRequest.User.Ext ??= new UserExt {
				Consent = consentString
			};
			return bidRequest;
		}
		
		public static BidRequest SetTest(this BidRequest bidRequest, bool testMode) {
			bidRequest.Test = testMode ? 1 : 0;
			return bidRequest;
		}


		public static BidRequest SetBannerFloor(this BidRequest bidRequest, float floor) {
			if (!bidRequest.Imp.IsNullOrEmpty() && bidRequest.Imp[0].Banner != null)
				bidRequest.Imp[0].Banner.BidFloor = floor;
			return bidRequest;
		}


		public static BidRequest SetVideoFloor(this BidRequest bidRequest, float floor) {
			if (!bidRequest.Imp.IsNullOrEmpty() && bidRequest.Imp[0].Video != null)
				bidRequest.Imp[0].Video.BidFloor = floor;
			return bidRequest;
		}

		public static BidRequest AttemptToShowVideoEndCard(this BidRequest bidRequest) {
			if (bidRequest.Imp.IsNullOrEmpty() || bidRequest.Imp[0].Video == null) return bidRequest;

			var size = Input.deviceOrientation == DeviceOrientation.Portrait
				? IabSupportedAdSizes.FullScreenPortrait
				: IabSupportedAdSizes.FullScreenLandscape;
			var (width, height) = size.ToWidthAndHeight();

			bidRequest.Imp[0].Video.CompanionAd = new[] {
				new Banner {
					W = width,
					H = height,
					Vcm = 1
				}
			};
			return bidRequest;
		}
		
		public static BidRequest SetRewardedVideoFlag(this BidRequest bidRequest, bool rewarded = true) {
			if (!bidRequest.Imp.IsNullOrEmpty() && bidRequest.Imp[0].Video != null)
				bidRequest.Imp[0].Video.Ext ??= new VideoExt {
					IsRewarded = rewarded ? 1: 0
				};
			return bidRequest;
		}

		public static BidRequest SetOMInformation(this BidRequest bidRequest, string sdkVersion) {
			var source = new Source();
			source.Ext = new SourceExt();
			source.Ext.Omidpn = "Adsbynimbus";
			source.Ext.Omidpv = sdkVersion;
			bidRequest.Source = source;
			return bidRequest;
		}
		
		internal static bool IsHybridBidRequest(this BidRequest bidRequest) {
			if (bidRequest.Imp.IsNullOrEmpty()) return false;
			return bidRequest.Imp[0].Banner != null && bidRequest.Imp[0].Video != null;
		}
	}
}