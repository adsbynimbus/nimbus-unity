using System;
using System.Runtime.InteropServices;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
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

		#endregion

		#region Wrapped methods and properties

		private readonly NimbusIOSAdManager _iOSAdManager;
		private Device _deviceCache;
		private const string DntDeviceID = "00000000-0000-0000-0000-000000000000";

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
			return _getSessionId();
		}

		internal override Device GetDevice() {
			Debug.unityLogger.Log("Get Device");
			_deviceCache ??= new Device {
				DeviceType = DeviceType.MobileTablet,
				H = Screen.height,
				W = Screen.width,
				Os = "ios",
				Make = "apple",
				Model = _getDeviceModel(),
				Osv = _getSystemVersion(),
			};
			
			_deviceCache.Ua = _getUserAgent();
			_deviceCache.ConnectionType = (ConnectionType)_getConnectionType();
			_deviceCache.Lmt = _isLimitAdTrackingEnabled() ? 1 : 0;
			
			_deviceCache.Ifa = _getAdvertisingId();
			if (_deviceCache.Ifa.IsNullOrEmpty()) {
				_deviceCache.Ifa = DntDeviceID;
			}
			
			Debug.unityLogger.Log($"Get Device connection type {_deviceCache.ConnectionType}");
			Debug.unityLogger.Log($"Get Device UA {_deviceCache.Ua}");
			Debug.unityLogger.Log($"Get Device ifa {_deviceCache.Ifa}");
			Debug.unityLogger.Log($"Get Device lmt {_deviceCache.Lmt}");
			Debug.unityLogger.Log($"Get Device device type {_deviceCache.DeviceType}");
			Debug.unityLogger.Log($"Get Device height {_deviceCache.H}");
			Debug.unityLogger.Log($"Get Device width {_deviceCache.W}");
			Debug.unityLogger.Log($"Get Device os {_deviceCache.Os}");
			Debug.unityLogger.Log($"Get Device make {_deviceCache.Make}");
			Debug.unityLogger.Log($"Get Device model {_deviceCache.Model}");
			Debug.unityLogger.Log($"Get Device osv {_deviceCache.Osv}");
			
			var body = JsonConvert.SerializeObject(_deviceCache);
			Debug.unityLogger.Log($"Get Device end body {body}");
			Debug.unityLogger.Log("Get Device end");

			return _deviceCache;
		}

		#endregion
	}
}