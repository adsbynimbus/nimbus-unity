using System;
using System.Runtime.InteropServices;
using AdsByNimbus.Scripts;
using Internal.Extensions.AdMob;
using Internal.Extensions.APS;
using Newtonsoft.Json;
using ScriptableObjects;
using UnityEngine;

namespace Internal {
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
			var nimbusAdUnit = NimbusCallbackReceiver.Instance.AdUnitForInstanceID(adUnitInstanceId);
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
			float bidFloor, bool respectSafeArea, int bannerPosition, bool showAd, string demand, string requestModifiers);
		
		[DllImport("__Internal")]
		private static extern void _interstitialAd(int adUnitInstanceId, string position, 
			float bannerFloor, float videoFloor, bool showAd, string demand, string requestModifiers);
		
		[DllImport("__Internal")]
		private static extern void _rewardedAd(int adUnitInstanceId, string position, float bidFloor, bool showAd, 
			string demand, string requestModifiers);
		
		[DllImport("__Internal")]
		private static extern void _showAd(int adUnitInstanceId, bool respectSafeArea, int bannerPosition);

		[DllImport("__Internal")]
		private static extern void _destroyAd(int adUnitInstanceId);

		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing iOS SDK");
			var extensions = new Extensions.Extensions();
			
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

		internal override void GetAd(Ad nimbusAdUnit, bool showAd) {
			var extensions = new Extensions.Extensions();
			NimbusCallbackReceiver.Instance.AddAdUnit(nimbusAdUnit);
			nimbusAdUnit.OnDestroyIOSAd += OnDestroyIOSAd;
			#if NIMBUS_ENABLE_ADMOB_IOS
				extensions.adMob.adUnitIds = _adMobIOS.GetAdUnitId(nimbusAdUnit.AdType);
			#endif

			switch (nimbusAdUnit.AdType)
			{
				case AdType.Inline:
				{
					if (nimbusAdUnit is InlineAd inlineAd)
					{
						var size = inlineAd.BannerSize.ToWidthAndHeight();
						#if NIMBUS_ENABLE_APS_IOS
						extensions.aps.slotData = _apsIOS.GetAdUnitId(AdType.Inline, size.Item1, size.Item2);
						#endif
						_bannerAd(inlineAd.InstanceID, inlineAd.NimbusReportingPosition, size.Item1,
							size.Item2, inlineAd.BannerRefreshIntervalInSeconds, inlineAd.BannerBidFloor,
							inlineAd.RespectSafeArea, (int)inlineAd.AdPosition, showAd,
							JsonConvert.SerializeObject(extensions)
							, JsonConvert.SerializeObject(inlineAd.RequestModifiers));
					}
					break;
				}
				case AdType.Fullscreen:
				{
					if (nimbusAdUnit is FullscreenAd fullscreenAd)
					{
						#if NIMBUS_ENABLE_APS_IOS
						extensions.aps.slotData = _apsIOS.GetAdUnitId(AdType.Fullscreen, 0, 0);
						#endif
						_interstitialAd(fullscreenAd.InstanceID, fullscreenAd.NimbusReportingPosition, 
							fullscreenAd.BannerBidFloor, fullscreenAd.VideoBidFloor,
							showAd, JsonConvert.SerializeObject(extensions), 
							JsonConvert.SerializeObject(fullscreenAd.RequestModifiers));
					}
					break;
				}
				case AdType.Rewarded:
				{
					if (nimbusAdUnit is RewardedAd rewardedAd)
					{
						#if NIMBUS_ENABLE_APS_IOS && NIMBUS_ENABLE_IOS
						extensions.aps.slotData = _apsIOS.GetAdUnitId(AdType.Rewarded, 0, 0);
						#endif
						_rewardedAd(rewardedAd.InstanceID, rewardedAd.NimbusReportingPosition, rewardedAd.VideoBidFloor, 
							showAd, JsonConvert.SerializeObject(extensions), 
							JsonConvert.SerializeObject(rewardedAd.RequestModifiers));
					}

					break;
				}
			}
		}

		internal override void ShowAd(Ad nimbusAdUnit)
		{
			var respectSafeArea = false;
			if (nimbusAdUnit is InlineAd inlineAd)
			{
				respectSafeArea = inlineAd.RespectSafeArea;
			}
			_showAd(nimbusAdUnit.InstanceID, respectSafeArea, (int) nimbusAdUnit.AdPosition);
		}
	}
#endif
}