using System;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using DeviceType = OpenRTB.Enumerations.DeviceType;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Nimbus.Internal {
	public class Android : NimbusAPI {
		private const string AndroidBuild = "android.os.Build";
		private const string AndroidBuildVersion = "android.os.Build$VERSION";
		private const string AndroidLogger = "com.adsbynimbus.Nimbus$Logger$Default";
		private const string ConnectionHelper = "com.adsbynimbus.request.ConnectionTypeKt";
		private const string HelperClass = "com.adsbynimbus.unity.UnityHelper";
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private AndroidJavaClass _build;
		private AndroidJavaClass _buildVersion;
		private AndroidJavaClass _connectionTypeHelper;

		private AndroidJavaObject _currentActivity;

		private Device _deviceCache;
		private AndroidJavaClass _helper;
		private AndroidJavaClass _nimbus;
		private AndroidJavaClass _unityPlayer;

		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_nimbus = new AndroidJavaClass(NimbusPackage);
			_helper = new AndroidJavaClass(HelperClass);
			_connectionTypeHelper = new AndroidJavaClass(ConnectionHelper);
			_build = new AndroidJavaClass(AndroidBuild);
			_buildVersion = new AndroidJavaClass(AndroidBuildVersion);

			var androidLogger = new AndroidJavaObject(AndroidLogger, 0);
			_nimbus.CallStatic("addLogger", androidLogger);
			_nimbus.CallStatic("initialize", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim());
		}


		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			const string functionCall = "render";
			var holdTime = 0;
			var shouldBlock = false;
			var listener = new AdManagerListener(in _helper, ref nimbusAdUnit);

			if (nimbusAdUnit.AdType == AdUnitType.Interstitial || nimbusAdUnit.AdType == AdUnitType.Rewarded) {
				shouldBlock = true;
				holdTime = 5;
				if (nimbusAdUnit.AdType == AdUnitType.Rewarded) holdTime = (int)TimeSpan.FromMinutes(60).TotalSeconds;
			}

			_helper.CallStatic(functionCall, _currentActivity, nimbusAdUnit.RawBidResponse, shouldBlock, holdTime,
				listener);
		}

		internal override string GetSessionID() {
			return _nimbus.CallStatic<string>("getSessionId");
		}


		internal override Device GetDevice() {
			_deviceCache ??= new Device {
				DeviceType = DeviceType.MobileTablet,
				H = Screen.height,
				W = Screen.width,
				Os = "android",
				Make = _build.GetStatic<string>("MANUFACTURER"),
				Model = _build.GetStatic<string>("MODEL"),
				Osv = _buildVersion.GetStatic<string>("RELEASE")
			};

			var ctx = CastToJavaObject(_currentActivity, "android.content.Context");
			_deviceCache.Ua = _nimbus.CallStatic<string>("getUserAgent", ctx);
			_deviceCache.ConnectionType =
				(ConnectionType)_connectionTypeHelper.CallStatic<byte>("getConnectionType", ctx);

			var _adInfo = _nimbus.CallStatic<AndroidJavaObject>("getAdIdInfo");
			_deviceCache.Lmt = _adInfo.Call<bool>("isLimitAdTrackingEnabled") ? 1 : 0;
			_deviceCache.Ifa = _adInfo.Call<string>("getId");
			return _deviceCache;
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}