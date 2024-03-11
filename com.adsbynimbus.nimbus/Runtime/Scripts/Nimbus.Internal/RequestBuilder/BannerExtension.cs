using System;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.RequestBuilder {
	public static class BannerExtension {
		private static readonly Api[] DefaultApis = { Api.Mraid1, Api.Mraid2, Api.Mraid3, Api.Omid };

		public static Banner Interstitial(this Banner banner) {
			var size = Input.deviceOrientation == DeviceOrientation.Portrait
				? IabSupportedAdSizes.FullScreenPortrait
				: IabSupportedAdSizes.FullScreenLandscape;
			var (width, height) = size.ToWidthAndHeight();
			banner.W = width;
			banner.H = height;
			banner.Pos = Position.Fullscreen;
			banner.Api = DefaultApis;
			return banner;
		}

		public static Banner SetupDefaults(this Banner banner, IabSupportedAdSizes size, Position position,
			float floor) {
			// if the publisher intends for one of the full screen ad sizes, flip ensure the correct orientation is used
			// warning" this may break if the inverse placements aren't set up in the database
			if (size == IabSupportedAdSizes.FullScreenLandscape || size == IabSupportedAdSizes.FullScreenPortrait)
				size = Input.deviceOrientation == DeviceOrientation.Portrait
					? IabSupportedAdSizes.FullScreenPortrait
					: IabSupportedAdSizes.FullScreenLandscape;

			var (width, height) = size.ToWidthAndHeight();

			banner.BidFloor = floor;
			banner.W = width;
			banner.H = height;
			banner.Pos = position;
			banner.Api = DefaultApis;
			return banner;
		}


		public static Banner SetFloor(this Banner banner, float floor) {
			banner.BidFloor = floor;
			return banner;
		}
	}
}