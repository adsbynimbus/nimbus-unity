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
		private static extern void _showBannerAd(
			int adUnitInstanceId, 
			string position, 
			float bannerFloor);

		[DllImport("__Internal")]
		private static extern void _showInterstitialAd(
			int adUnitInstanceId, 
			string position, 
			float bannerFloor, 
			float videoFloor, 
			double closeButtonDelay);

		[DllImport("__Internal")]
		private static extern void _showRewardedVideoAd(
			int adUnitInstanceId, 
			string position, 
			float videoFloor,
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

		#endregion

		#region Wrapped methods and properties

		private readonly NimbusIOSAdManager _iOSAdManager;
		private Device _device;

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
			// TODO see the android implementation
			throw new Exception("iOS not supported yet");
		}

		internal override string GetSessionID() {
			Debug.unityLogger.Log("Get Session ID");
			return _getSessionId();
		}

		internal override Device GetDevice() {
			Debug.unityLogger.Log("Get Device");
			_device = new Device {
				DeviceType = DeviceType.MobileTablet,
				H = Screen.height,
				W = Screen.width,
				Os = "ios",
				Make = "apple",
            	Model = _getDeviceModel(),
				Osv = _getSystemVersion()
			};

			_device.Ua = _getUserAgent();
			_device.ConnectionType = (ConnectionType)_getConnectionType();
			_device.Lmt = _isLimitAdTrackingEnabled() ? 1 : 0;
			_device.Ifa = _getAdvertisingId();
			return _device;
		}

		#endregion
	}
}