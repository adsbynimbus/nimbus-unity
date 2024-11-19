using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using Nimbus.Internal.Utility;
using DeviceType = OpenRTB.Enumerations.DeviceType;

namespace Nimbus.Internal {
	#if UNITY_IOS
	public class IOS : NimbusAPI {
		// ThirdParty Providers
		private List<IInterceptor> _interceptors;
		
		private static void OnDestroyIOSAd(int adUnitInstanceId) {
			var nimbusAdUnit = NimbusIOSAdManager.Instance.AdUnitForInstanceID(adUnitInstanceId);
			if (nimbusAdUnit != null) {
				nimbusAdUnit.OnDestroyIOSAd -= OnDestroyIOSAd;
			}
			_destroyAd(adUnitInstanceId);
		}

		[DllImport("__Internal")]
		private static extern void _initializeSDKWithPublisher(
			string publisher,
			string apiKey,
			bool enableUnityLogs);

		[DllImport("__Internal")]
		private static extern void _renderAd(int adUnitInstanceId, string bidResponse, bool isBlocking, bool isRewarded,
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
		private static extern string _getDeviceLanguage();

		[DllImport("__Internal")]
		private static extern string _getSystemVersion();
		
		[DllImport("__Internal")]
		private static extern void _setCoppa(bool flag);

		[DllImport("__Internal")]
		private static extern bool _isLimitAdTrackingEnabled();
		
		[DllImport("__Internal")]
		private static extern string _getPlistJSON();

		[DllImport("__Internal")]
		private static extern int _getAtts();

		[DllImport("__Internal")]
		private static extern string _getVendorId();

		[DllImport("__Internal")]
		private static extern string _getVersion();

		private Device _deviceCache;
		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");
			
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableUnityLogs);
			
			var plist = GetPlistJson();
			if (StaticMethod.InitializeInterceptor() || !plist.IsNullOrEmpty()) {
				_interceptors = new List<IInterceptor>();		
			}

			if (!plist.IsNullOrEmpty()) {
				_interceptors.Add(new SkAdNetworkIOS(plist));
			}
			
			#if NIMBUS_ENABLE_APS
				Debug.unityLogger.Log("Initializing iOS APS SDK");
				var (appID, slots) = configuration.GetApsData();
				var aps = new ApsIOS(appID, slots, configuration.enableSDKInTestMode);
				aps.InitializeNativeSDK();
				_interceptors.Add(aps);
			#endif
			#if NIMBUS_ENABLE_VUNGLE
				Debug.unityLogger.Log("Initializing iOS Liftoff Monetize SDK");
				var appID = configuration.GetVungleData();
				var vungle = new VungleIOS(appID);
				vungle.InitializeNativeSDK();
				_interceptors.Add(vungle);
			#endif
		}

		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			NimbusIOSAdManager.Instance.AddAdUnit(nimbusAdUnit);
			nimbusAdUnit.OnDestroyIOSAd += OnDestroyIOSAd;

			var isBlocking = false;
			var isRewarded = false;
			var closeButtonDelay = 0;
			if (nimbusAdUnit.AdType == AdUnitType.Interstitial || nimbusAdUnit.AdType == AdUnitType.Rewarded) {
				isBlocking = true;
				closeButtonDelay = 5;
				if (nimbusAdUnit.AdType == AdUnitType.Rewarded)
				{
					isRewarded = true;
					closeButtonDelay = (int)TimeSpan.FromMinutes(60).TotalSeconds;
				}
			}

			_renderAd(nimbusAdUnit.InstanceID, nimbusAdUnit.RawBidResponse, isBlocking, isRewarded, closeButtonDelay);
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
				Language = _getDeviceLanguage(),
				Ext = new DeviceExt {
					Ifv = _getVendorId()
				},
			};

			_deviceCache.ConnectionType = (ConnectionType)_getConnectionType();
			_deviceCache.Lmt = _isLimitAdTrackingEnabled() ? 1 : 0;
			_deviceCache.Ifa = _getAdvertisingId();
			_deviceCache.Ua = _getUserAgent();
			var atts = _getAtts();
			if (atts > -1) { 
				_deviceCache.Ext.Atts = atts;
			}

			return _deviceCache;
		}

		internal override List<IInterceptor> Interceptors() {
			return _interceptors;
		}
		
		internal override void SetCoppaFlag(bool flag) {
			_setCoppa(flag);
		}

		internal override string GetVersion() {
			return _getVersion();
		}

		private static string GetPlistJson() {
			return  _getPlistJSON();
		}
	}
#endif
}