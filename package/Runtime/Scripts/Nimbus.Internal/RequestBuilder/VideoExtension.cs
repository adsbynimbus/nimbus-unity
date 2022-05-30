using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal.RequestBuilder {
	public static class VideoExtension {
		private static readonly Protocols[] DefaultProtocols = {
			Protocols.Vast2, Protocols.Vast3, Protocols.Vast2Wrapper, Protocols.Vast3Wrapper
		};

#if UNITY_EDITOR || UNITY_ANDROID
		private static readonly string[] DefaultMimes =
			{ "application/x-mpegurl", "video/mp4", "video/3gpp", "video/x-flv" };
#else
		private static readonly string[] Mimes = {"video/mp4", "video/3gpp", "application/x-mpegurl"};
#endif

		public static Video Interstitial(this Video video) {
			video.W = Screen.width;
			video.H = Screen.height;
			video.Pos = Position.Fullscreen;
			video.Protocols = DefaultProtocols;
			video.Mimes = DefaultMimes;
			return video;
		}


		public static Video SetupDefaults(this Video video, Position position, float floor) {
			video.BidFloor = floor;
			video.W = Screen.width;
			video.H = Screen.height;
			video.Pos = position;
			video.Protocols = DefaultProtocols;
			video.Mimes = DefaultMimes;
			return video;
		}

		public static Video SetFloor(this Video video, float floor) {
			video.BidFloor = floor;
			return video;
		}
	}
}