using System;
using System.Runtime.InteropServices;
using Nimbus.ScriptableObjects;
using OpenRTB.Request;
using UnityEngine;

namespace Nimbus.Internal {
	public class IOS : NimbusAPI {
		private static void OnDestroyIOSAd(int adUnitInstanceId) {
			_destroyAd(adUnitInstanceId);
		}

		#region Declare external C interface

		[DllImport("__Internal")]
		private static extern void _initializeSDKWithPublisher(string publisher,
			string apiKey,
			bool enableSDKInTestMode, // TODO we can remove this parameter
			bool enableUnityLogs);

		[DllImport("__Internal")]
		private static extern void _showBannerAd(int adUnitInstanceId, string position, float bannerFloor);

		[DllImport("__Internal")]
		private static extern void _showInterstitialAd(int adUnitInstanceId, string position, float bannerFloor,
			float videoFloor,
			double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _showRewardedVideoAd(int adUnitInstanceId, string position, float videoFloor,
			double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _setGDPRConsentString(string consent);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);

		#endregion

		#region Wrapped methods and properties

		private readonly NimbusIOSAdManager _iOSAdManager;

		public IOS() {
			_iOSAdManager = NimbusIOSAdManager.Instance;
		}

		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing IOS SDK");
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableSDKInTestMode, // TODO can remove this, test mood is handled natively now
				configuration.enableUnityLogs);
		}

		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			// TODO see the android implementation
			throw new Exception("ios not supported yet");
		}


		internal override string GetSessionID() {
			throw new Exception("ios not supported yet");
		}

		internal override Device GetDevice() {
			// TODO this one is tricky see the android implementation, for efficiency we do not retrieve all the fields on every requests
			// we only retrieve fields continuously that can change or fail like UserAgent, LMT, DNT, Connection type, etc
			throw new Exception("ios not supported yet");
		}

		#endregion
	}
}