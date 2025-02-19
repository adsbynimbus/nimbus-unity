using System;
using System.Collections.Generic;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
using Nimbus.Internal.Utility;
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
		private string _sessionId;
		
		// ThirdParty Providers
		private List<IInterceptor> _interceptors;

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

			if (StaticMethod.InitializeInterceptor()) {
				_interceptors = new List<IInterceptor>();		
			}

			#if NIMBUS_ENABLE_APS
				var (apsAppID, slots) = configuration.GetApsData();
				var aps = new ApsAndroid(_currentActivity, apsAppID, slots, configuration.enableSDKInTestMode);
				aps.InitializeNativeSDK();
				_interceptors.Add(aps);
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				var vungleAppId = configuration.GetVungleData();
				var applicationContext = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");
				Debug.unityLogger.Log(vungleAppId);
				var vungle = new VungleAndroid(applicationContext, vungleAppId);
				vungle.InitializeNativeSDK();
				_interceptors.Add(vungle);
			#endif
			#if NIMBUS_ENABLE_META
				var metaAppId = configuration.GetMetaData();
				var meta = new MetaAndroid(_currentActivity, configuration.enableSDKInTestMode, metaAppId);
				meta.InitializeNativeSDK();
				_interceptors.Add(meta);
			#endif
			#if NIMBUS_ENABLE_ADMOB
				var (adMobAppID, adUnitIds) = configuration.GetAdMobData();
				var admob = new AdMobAndroid(_currentActivity, adMobAppID, adUnitIds, configuration.enableSDKInTestMode);
				_interceptors.Add(admob);
			#endif
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
			if (_sessionId.IsNullOrEmpty()) {
				_sessionId = _nimbus.CallStatic<string>("getSessionId");
			}
			return _sessionId;
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
				(ConnectionType)_connectionTypeHelper.CallStatic<sbyte>("getConnectionType", ctx);

			var _adInfo = _nimbus.CallStatic<AndroidJavaObject>("getAdIdInfo");
			var cls = new AndroidJavaClass("java.util.Locale");
			var locale = cls.CallStatic<AndroidJavaObject>("getDefault");
			_deviceCache.Language = locale.Call<string>("getLanguage"); 
			_deviceCache.Lmt = _adInfo.Call<bool>("isLimitAdTrackingEnabled") ? 1 : 0;
			_deviceCache.Ifa = _adInfo.Call<string>("getId");
			return _deviceCache;
		}

		internal override List<IInterceptor> Interceptors() {
			return _interceptors;
		}
		
		internal override void SetCoppaFlag(bool flag) {
			_nimbus.SetStatic("COPPA", flag);
		}

		internal override string GetVersion() {
			return _nimbus.GetStatic<string>("version");
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}