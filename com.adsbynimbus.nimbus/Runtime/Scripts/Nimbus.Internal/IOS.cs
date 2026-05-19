using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Nimbus.ScriptableObjects;
using UnityEngine;
using Nimbus.Internal.Extensions.AdMob;
using Nimbus.Internal.Extensions.APS;

namespace Nimbus.Internal {
	#if UNITY_IOS
	public class IOS : NimbusAPI {
		// ThirdParty Providers
		#if NIMBUS_ENABLE_ADMOB
			private AdMobIOS _adMobIOS;
		#endif
		#if NIMBUS_ENABLE_APS
			private ApsIOS _apsIOS;
		#endif
		
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
			bool respectSafeArea, int bannerPosition, bool showAd, string demand);
		
		[DllImport("__Internal")]
		private static extern void _interstitialAd(int adUnitInstanceId, string position, bool showAd, 
			string demand);
		
		[DllImport("__Internal")]
		private static extern void _rewardedAd(int adUnitInstanceId, string position, bool showAd, 
			string demand);
		
		[DllImport("__Internal")]
		private static extern void _showAd(int adUnitInstanceId, bool respectSafeArea, int bannerPosition);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);

		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");
			var extensions = new Nimbus.Internal.Extensions.Extensions();
			
			#if NIMBUS_ENABLE_APS
				Debug.unityLogger.Log("Initializing iOS APS SDK");
				var (apsAppID, slots) = configuration.GetApsData();
				_apsIOS = new ApsIOS(apsAppID, slots, configuration.enableSDKInTestMode);
				extensions.aps.appKey = apsAppID;
			#endif
			#if NIMBUS_ENABLE_VUNGLE
				Debug.unityLogger.Log("Initializing iOS Vungle SDK");
				extensions.vungle.appId = configuration.GetVungleData();
			#endif
			#if NIMBUS_ENABLE_META
				Debug.unityLogger.Log("Initializing iOS Meta SDK");
				extensions.meta.appId = configuration.GetMetaData();
				extensions.meta.forceTestAd = configuration.enableSDKInTestMode;
			#endif
			#if NIMBUS_ENABLE_ADMOB
				Debug.unityLogger.Log("Initializing iOS AdMob SDK");
				var adMobAdUnitIds = configuration.GetAdMobData();
				_adMobIOS = new AdMobIOS(adMobAdUnitIds);
			#endif
			#if NIMBUS_ENABLE_MINTEGRAL
				Debug.unityLogger.Log("Initializing iOS Mintegral SDK");
				var (mintegralAppID, mintegralAppKey) = configuration.GetMintegralData();
				extensions.mintegral.appId = mintegralAppID;
				extensions.mintegral.appKey = mintegralAppKey;
			#endif
			#if NIMBUS_ENABLE_UNITY_ADS
				Debug.unityLogger.Log("Initializing iOS Unity Ads SDK");
				extensions.unityAds.gameId = configuration.GetUnityAdsData();
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				Debug.unityLogger.Log("Initializing iOS Moloco SDK");
				extensions.moloco.appKey = configuration.GetMolocoData();
			#endif
			
			#if NIMBUS_ENABLE_INMOBI
				Debug.unityLogger.Log("Initializing iOS InMobi SDK");
				extensions.inMobi.accountId = configuration.GetInMobiData();
			#endif
			
			_initializeSDKWithPublisher(configuration.publisherKey,
				configuration.apiKey,
				configuration.enableUnityLogs, configuration.enableSDKInTestMode, JsonConvert.SerializeObject(extensions));
		}

		internal override void getAd(NimbusAdUnit nimbusAdUnit, bool showAd) {
			var extensions = new Nimbus.Internal.Extensions.Extensions();
			NimbusIOSAdManager.Instance.AddAdUnit(nimbusAdUnit);
			nimbusAdUnit.OnDestroyIOSAd += OnDestroyIOSAd;
			#if NIMBUS_ENABLE_ADMOB_IOS
				extensions.adMob.adUnitIds = _adMobIOS.GetAdUnitId(nimbusAdUnit.AdType);
			#endif

			switch (nimbusAdUnit.AdType)
			{
				case AdType.Banner:
				{
					var size = nimbusAdUnit.BannerSize.ToWidthAndHeight();
					#if NIMBUS_ENABLE_APS_IOS
						extensions.aps.slotData = _apsIOS.GetAdUnitId(AdType.Banner, size.Item1, size.Item2);
					#endif
					_bannerAd(nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, size.Item1, 
						size.Item2, nimbusAdUnit.BannerRefreshIntervalInSeconds, 
						nimbusAdUnit.RespectSafeArea, (int) nimbusAdUnit.AdPosition, showAd, JsonConvert.SerializeObject(extensions));
					break;
				}
				case AdType.Interstitial:
				{
					#if NIMBUS_ENABLE_APS_IOS
						extensions.aps.slotData = _apsIOS.GetAdUnitId(AdType.Interstitial, 0, 0);
					#endif
					_interstitialAd(nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, 
						showAd, JsonConvert.SerializeObject(extensions));
					break;
				}
				case AdType.Rewarded:
				{
					#if NIMBUS_ENABLE_APS_IOS
						extensions.aps.slotData = _apsIOS.GetAdUnitId(AdType.Rewarded, 0, 0);
					#endif
					_rewardedAd(nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, showAd, 
						JsonConvert.SerializeObject(extensions));
					break;
				}
			}
		}
	}
#endif
}