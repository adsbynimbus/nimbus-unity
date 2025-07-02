using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.APS;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Molocol;
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
			bool enableUnityLogs,
			bool enableSDKInTestMode);

		[DllImport("__Internal")]
		private static extern void _renderAd(int adUnitInstanceId, string bidResponse, bool isBlocking, bool isRewarded,
			double closeButtonDelay, string mintegralAdUnitId, string mintegralAdUnitPlacementId, string molocoAdUnitId);

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
		
		private ThirdPartyAdUnit[] mintegralAdUnits;
		
		private ThirdPartyAdUnit[] molocoAdUnits;

		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");
			
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableUnityLogs, configuration.enableSDKInTestMode);
			
			var plist = GetPlistJson();
			if (StaticMethod.InitializeInterceptor() || !plist.IsNullOrEmpty()) {
				_interceptors = new List<IInterceptor>();		
			}

			if (!plist.IsNullOrEmpty()) {
				_interceptors.Add(new SkAdNetworkIOS(plist));
			}
			
			#if NIMBUS_ENABLE_APS
				Debug.unityLogger.Log("Initializing iOS APS SDK");
				var (apsAppID, slots, timeout) = configuration.GetApsData();
				var aps = new ApsIOS(apsAppID, slots, configuration.enableSDKInTestMode, timeout);
				aps.InitializeNativeSDK();
				_interceptors.Add(aps);
			#endif
			#if NIMBUS_ENABLE_VUNGLE
				Debug.unityLogger.Log("Initializing iOS Vungle SDK");
				var vungleAppID = configuration.GetVungleData();
				var vungle = new VungleIOS(vungleAppID);
				vungle.InitializeNativeSDK();
				_interceptors.Add(vungle);
			#endif
			#if NIMBUS_ENABLE_META
				Debug.unityLogger.Log("Initializing iOS Meta SDK");
				var metaAppID = configuration.GetMetaData();
				var meta = new MetaIOS(metaAppID, configuration.enableSDKInTestMode);
				meta.InitializeNativeSDK();
				_interceptors.Add(meta);
			#endif
			#if NIMBUS_ENABLE_ADMOB
				Debug.unityLogger.Log("Initializing iOS AdMob SDK");
				var (adMobAppID, adMobAdUnitIds) = configuration.GetAdMobData();
				var admob = new AdMobIOS(adMobAdUnitIds);
				admob.InitializeNativeSDK();
				_interceptors.Add(admob);
			#endif
			#if NIMBUS_ENABLE_MINTEGRAL
				Debug.unityLogger.Log("Initializing iOS Mintegral SDK");
				var (mintegralAppID, mintegralAppKey, mintegralAdUnitIds) = configuration.GetMintegralData();
				mintegralAdUnits = mintegralAdUnitIds;
				var mintegral = new MintegralIOS(mintegralAppID, mintegralAppKey, mintegralAdUnitIds);
				mintegral.InitializeNativeSDK();
				_interceptors.Add(mintegral);
			#endif
			#if NIMBUS_ENABLE_UNITY_ADS
				Debug.unityLogger.Log("Initializing iOS Unity Ads SDK");
				var unityGameId = configuration.GetUnityAdsData();
				var unityAds = new UnityAdsIOS(unityGameId);
				unityAds.InitializeNativeSDK();
				_interceptors.Add(unityAds);
			#endif
			#if NIMBUS_ENABLE_MOBILEFUSE
				Debug.unityLogger.Log("Initializing iOS MobileFuse SDK");
				var mobileFuse = new MobileFuseIOS();
				mobileFuse.InitializeNativeSDK();
				_interceptors.Add(mobileFuse);
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				Debug.unityLogger.Log("Initializing iOS Moloco SDK");
				var (molocoAppKey, molocoAdUnitIds) = configuration.GetMolocoData();
				molocoAdUnits = molocoAdUnitIds;
				var moloco = new MolocoIOS(molocoAppKey, molocoAdUnitIds, configuration.enableSDKInTestMode);
				moloco.InitializeNativeSDK();
				_interceptors.Add(moloco);
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
			var mintegralAdUnitId = "";
			var mintegralAdUnitPlacementId = "";
			var molocoAdUnitId = "";
			#if NIMBUS_ENABLE_MINTEGRAL
				try
				{
					var minteralAdUnit =
						mintegralAdUnits.SingleOrDefault(adUnit => adUnit.AdUnitType == nimbusAdUnit.AdType);
					mintegralAdUnitId = minteralAdUnit.AdUnitId;
					mintegralAdUnitPlacementId = minteralAdUnit.AdUnitPlacementId;
				}
				catch (Exception e)
				{
					Debug.unityLogger.LogException(e);
				}
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				try
				{
					var molocoAdUnit =
						molocoAdUnits.SingleOrDefault(adUnit => adUnit.AdUnitType == nimbusAdUnit.AdType);
					molocoAdUnitId = molocoAdUnit.AdUnitId;
				}
				catch (Exception e)
				{
					Debug.unityLogger.LogException(e);
				}
			#endif
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(nimbusAdUnit.RawBidResponse);
			_renderAd(nimbusAdUnit.InstanceID, System.Convert.ToBase64String(plainTextBytes), isBlocking, isRewarded, closeButtonDelay, mintegralAdUnitId, mintegralAdUnitPlacementId, molocoAdUnitId);
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