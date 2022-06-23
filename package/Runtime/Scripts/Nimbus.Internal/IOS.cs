using System;
using System.Runtime.InteropServices;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using Nimbus.Internal.Utility;
using DeviceType = OpenRTB.Enumerations.DeviceType;

namespace Nimbus.Internal {
	public class IOS : NimbusAPI {
		private static void OnDestroyIOSAd(int adUnitInstanceId) {
			_destroyAd(adUnitInstanceId);
		}

		[DllImport("__Internal")]
		private static extern void _initializeSDKWithPublisher(
			string publisher,
			string apiKey,
			bool enableUnityLogs);

		[DllImport("__Internal")]
		private static extern void _renderAd(int adUnitInstanceId, string bidResponse, bool isBlocking,
			double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);

		[DllImport("__Internal")]
		private static extern string _getSessionId();

		[DllImport("__Internal")]
		private static extern string _getUserAgent();

		[DllImport("__Internal")]
		private static extern string _getAdvertisingId();

		[DllImport("__Internal")]
		private static extern int _getConnectionType();

		[DllImport("__Internal")]
		private static extern string _getDeviceModel();

		[DllImport("__Internal")]
		private static extern string _getSystemVersion();

		[DllImport("__Internal")]
		private static extern bool _isLimitAdTrackingEnabled();


		private readonly NimbusIOSAdManager _iOSAdManager;

		private Device _deviceCache;
		private string _sessionId;
		private const string DntDeviceID = "00000000-0000-0000-0000-000000000000";

		public IOS() {
			_iOSAdManager = NimbusIOSAdManager.Instance;
		}

		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableUnityLogs);
		}

		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			var isBlocking = false;
			var closeButtonDelay = 0;
			if (nimbusAdUnit.AdType == AdUnitType.Interstitial || nimbusAdUnit.AdType == AdUnitType.Rewarded) {
				closeButtonDelay = 5;
				isBlocking = true;
				if (nimbusAdUnit.AdType == AdUnitType.Rewarded)
					closeButtonDelay = (int)TimeSpan.FromMinutes(60).TotalSeconds;
			}

			_renderAd(nimbusAdUnit.InstanceID, nimbusAdUnit.RawBidResponse, isBlocking, closeButtonDelay);
		}

		internal override string GetSessionID() {
			if (_sessionId.IsNullOrEmpty()) {
				_sessionId = _getSessionId();
			}
			return _sessionId;
		}

		internal override Device GetDevice() {
			_deviceCache ??= new Device {
				DeviceType = DeviceType.MobileTablet,
				H = Screen.height,
				W = Screen.width,
				Os = "ios",
				Make = "apple",
				Model = _getDeviceModel(),
				Osv = _getSystemVersion(),
			};

			_deviceCache.ConnectionType = (ConnectionType)_getConnectionType();
			_deviceCache.Lmt = _isLimitAdTrackingEnabled() ? 1 : 0;
			_deviceCache.Ifa = _getAdvertisingId();
			_deviceCache.Ua = _getUserAgent();
			if (_deviceCache.Ifa.IsNullOrEmpty()) {
				_deviceCache.Ifa = DntDeviceID;
			}

			return _deviceCache;
		}
	}
}