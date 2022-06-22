using System;
using System.Runtime.InteropServices;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using DeviceType = OpenRTB.Enumerations.DeviceType;

namespace Nimbus.Internal {
	public class IOS : NimbusAPI {
		private static void OnDestroyIOSAd(int adUnitInstanceId) {
			_destroyAd(adUnitInstanceId);
		}

		#region Declare external C interface

		[DllImport("__Internal")]
		private static extern void _initializeSDKWithPublisher(
			string publisher, 
			string apiKey, 
			bool enableUnityLogs);

		[DllImport("__Internal")]
		private static extern void _renderAd(int adUnitInstanceId, string bidResponse, bool isBlocking, double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);

		[DllImport("__Internal")]
		private static extern void _getSessionId(out string sessionId);

		[DllImport("__Internal")]
		private static extern void _getUserAgent(out string userAgent);

		[DllImport("__Internal")]
		private static extern void _getAdvertisingId(out string advertisingId);

		[DllImport("__Internal")]
		private static extern void _getConnectionType(out int connectionType);

		[DllImport("__Internal")]
		private static extern void _getDeviceModel(out string deviceModel);

		[DllImport("__Internal")]
		private static extern void _getSystemVersion(out string systemVersion);

		[DllImport("__Internal")]
		private static extern void _isLimitAdTrackingEnabled(out bool limitAdTracking);

		#endregion

		#region Wrapped methods and properties

		private readonly NimbusIOSAdManager _iOSAdManager;
		private Device _deviceCache;

		public IOS() {
			_iOSAdManager = NimbusIOSAdManager.Instance;
		}

		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");

			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableUnityLogs);

			Debug.unityLogger.Log("Ended initializing iOS SDK");
		}

		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log("Show Ad");

			var isBlocking = false;
			var closeButtonDelay = 0;
			if (nimbusAdUnit.AdType == AdUnitType.Interstitial || nimbusAdUnit.AdType == AdUnitType.Rewarded) {
				closeButtonDelay = 5;
				isBlocking = true;
				if (nimbusAdUnit.AdType == AdUnitType.Rewarded) closeButtonDelay = (int)TimeSpan.FromMinutes(60).TotalSeconds;
			}

			_renderAd(nimbusAdUnit.InstanceID, nimbusAdUnit.RawBidResponse, isBlocking, closeButtonDelay);
		}

		internal override string GetSessionID() {
			Debug.unityLogger.Log("Get Session ID");

			var sessionId = "SessionID";
			_getSessionId(out sessionId);
			return sessionId;
		}

		internal override Device GetDevice() {
			Debug.unityLogger.Log("Get Device");

			var deviceModel = "model";
			_getDeviceModel(out deviceModel);

			var systemVersion = "systemVersion";
			_getSystemVersion(out systemVersion);

			_deviceCache ??= new Device {
				DeviceType = DeviceType.MobileTablet,
				H = Screen.height,
				W = Screen.width,
				Os = "ios",
				Make = "apple",
            	Model = deviceModel,
				Osv = systemVersion
			};

			_deviceCache.Ua ??= "0000";

			var connectionType = 0;
			_getConnectionType(out connectionType);
			_deviceCache.ConnectionType = (ConnectionType)connectionType;

			var limitAdTracking = false;
			_isLimitAdTrackingEnabled(out limitAdTracking);
			_deviceCache.Lmt = limitAdTracking ? 1 : 0;

			var advertisingId = "AdvertisingId";
			_getAdvertisingId(out advertisingId);
			_deviceCache.Ifa = advertisingId;

			Debug.unityLogger.Log("Get Device end");

			return _deviceCache;
		}

		#endregion
	}
}