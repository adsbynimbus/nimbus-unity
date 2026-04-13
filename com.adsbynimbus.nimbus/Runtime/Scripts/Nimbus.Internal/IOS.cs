using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.APS;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.InMobi;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Moloco;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using Nimbus.Internal.Utility;
using Newtonsoft.Json.Linq;
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
			bool enableSDKInTestMode,
			string thirdPartyJson);

		[DllImport("__Internal")]
		private static extern void _bannerAd(int adUnitInstanceId, string position, int width, int height, int refreshInterval, 
			bool respectSafeArea, int bannerPosition, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _interstitialAd(int adUnitInstanceId, string position, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _rewardedAd(int adUnitInstanceId, string position, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _showAd(int adUnitInstanceId, bool respectSafeArea, int bannerPosition);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);
		
		[DllImport("__Internal")]
		private static extern string _getPlistJSON();

		private Device _deviceCache;
		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");
			
			var plist = GetPlistJson();
			if (StaticMethod.InitializeInterceptor() || !plist.IsNullOrEmpty()) {
				_interceptors = new List<IInterceptor>();		
			}

			if (!plist.IsNullOrEmpty()) {
				_interceptors.Add(new SkAdNetworkIOS(plist));
			}

			var interceptorConfigArr = new JArray();
			
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
				interceptorConfigArr.Add(vungle.GetConfigObject());
				_interceptors.Add(vungle);
			#endif
			#if NIMBUS_ENABLE_META
				Debug.unityLogger.Log("Initializing iOS Meta SDK");
				var metaAppID = configuration.GetMetaData();
				var meta = new MetaIOS(metaAppID, configuration.enableSDKInTestMode);
				interceptorConfigArr.Add(meta.GetConfigObject());
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
				var (mintegralAppID, mintegralAppKey) = configuration.GetMintegralData();
				var mintegral = new MintegralIOS(mintegralAppID, mintegralAppKey);
				interceptorConfigArr.Add(mintegral.GetConfigObject());
				_interceptors.Add(mintegral);
			#endif
			#if NIMBUS_ENABLE_UNITY_ADS
				Debug.unityLogger.Log("Initializing iOS Unity Ads SDK");
				var unityGameId = configuration.GetUnityAdsData();
				var unityAds = new UnityAdsIOS(unityGameId);
				interceptorConfigArr.Add(unityAds.GetConfigObject());
				_interceptors.Add(unityAds);
			#endif
			#if NIMBUS_ENABLE_MOBILEFUSE
				Debug.unityLogger.Log("Initializing iOS MobileFuse SDK");
				var mobileFuse = new MobileFuseIOS();
				interceptorConfigArr.Add(mobileFuse.GetConfigObject());
				_interceptors.Add(mobileFuse);
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				Debug.unityLogger.Log("Initializing iOS Moloco SDK");
				var molocoAppKey = configuration.GetMolocoData();
				var moloco = new MolocoIOS(molocoAppKey);
				interceptorConfigArr.Add(moloco.GetConfigObject());
				_interceptors.Add(moloco);
			#endif
			
			#if NIMBUS_ENABLE_INMOBI
				Debug.unityLogger.Log("Initializing iOS InMobi SDK");
				var inMobiAccountId = configuration.GetInMobiData();
				var inMobi = new InMobiIOS(inMobiAccountId);
				interceptorConfigArr.Add(inMobi.GetConfigObject());
				_interceptors.Add(inMobi);
			#endif
			
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableUnityLogs, configuration.enableSDKInTestMode, JsonConvert.SerializeObject(interceptorConfigArr));
		}

		internal override void getAd(NimbusAdUnit nimbusAdUnit, bool showAd) {
			NimbusIOSAdManager.Instance.AddAdUnit(nimbusAdUnit);
			nimbusAdUnit.OnDestroyIOSAd += OnDestroyIOSAd;
			
			switch (nimbusAdUnit.AdType)
			{
				case AdType.Banner:
				{
					var size = nimbusAdUnit.BannerSize.ToWidthAndHeight();
					_bannerAd(nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, size.Item1, size.Item2, nimbusAdUnit.BannerRefreshIntervalInSeconds, nimbusAdUnit.RespectSafeArea, (int) nimbusAdUnit.AdPosition, showAd);
					break;
				}
				case AdType.Interstitial:
				{
					_interstitialAd(nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, showAd);
					break;
				}
				case AdType.Rewarded:
				{
					_rewardedAd(nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, showAd);
					break;
				}
			}
		}

		internal override List<IInterceptor> Interceptors() {
			return _interceptors;
		}

		private static string GetPlistJson() {
			return  _getPlistJSON();
		}
	}
#endif
}