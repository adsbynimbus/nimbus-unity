using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using AdsByNimbus;
using AdsByNimbus.Internal.Extensions.AdMob;
using AdsByNimbus.Internal.Extensions.APS;
using Newtonsoft.Json;
using ScriptableObjects;
using UnityEngine;

namespace AdsByNimbus.Internal {
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
		private static extern void _bannerAd(int adUnitInstanceId, string position, int width, int height, string addFormats,
			int adPosition, float bidFloor, int refreshInterval, int bannerPosition, int xCoord, int yCoord, bool respectSafeArea, string demand, 
			string requestModifier, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _dynamicUnit(int adUnitInstanceId, string position, string addFormats, int orientation,
			int adPosition, float bidFloor, int refreshInterval, int width, int height, int bannerPosition, int xCoord, int yCoord, bool respectSafeArea, 
			string demand, string requestModifiers, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _fullscreenAd(int adUnitInstanceId, string position, 
			int orientation, string demand, string requestModifiers, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _interstitialAd(int adUnitInstanceId, string position, string addFormats, 
			int orientation, float bidFloor, string demand, string requestModifiers, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _rewardedAd(int adUnitInstanceId, string position, int orientation, float bidFloor, 
			string demand, string requestModifiers, bool showAd);
		
		[DllImport("__Internal")]
		private static extern void _showAd(int adUnitInstanceId, int width, int height, bool respectSafeArea, int bannerPosition, int xCoord, 
				int yCoord);

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
			
			#if NIMBUS_ENABLE_DIGITAL_TURBINE
				Debug.unityLogger.Log("Initializing iOS Digital Turbine SDK");
				extensions.digitalTurbine.appId = configuration.GetDigitalTurbineData();
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
				extensions.adMob.adUnitIds = nimbusAdUnit.AdMobAdUnitId == null ?  _adMobIOS.GetAdUnitId(nimbusAdUnit.AdType) :
					new [] {nimbusAdUnit.AdMobAdUnitId};
			#endif

			switch (nimbusAdUnit.AdType)
			{
				case AdType.Inline:
				{
					if (nimbusAdUnit is InlineAd inlineAd)
					{
						var size = inlineAd.AdSize.ToWidthAndHeight();
						#if NIMBUS_ENABLE_APS_IOS
							extensions.aps.slotData = nimbusAdUnit.ApsAds == null ? _apsIOS.GetAdUnitId(AdType.Inline, 
								size.Width, size.Height) : nimbusAdUnit.ApsAds;
						#endif
						if (inlineAd.DynamicUnit)
						{
							_dynamicUnit(inlineAd.InstanceID, inlineAd.position, 
								string.Join(",", inlineAd.AddFormats.Cast<byte>()), (int) inlineAd.Orientation, 
								(int) inlineAd.AdPosition,  inlineAd.BidFloor, 
								inlineAd.RefreshInterval, inlineAd.DynamicUnitWidth, inlineAd.DynamicUnitHeight, 
								(int)inlineAd.AdScreenPosition, inlineAd.XCoord, inlineAd.YCoord,
								inlineAd.RespectSafeArea, JsonConvert.SerializeObject(extensions)
								, JsonConvert.SerializeObject(inlineAd.GetRequestModifiers()), showAd);
						}
						else
						{
							_bannerAd(inlineAd.InstanceID, inlineAd.position, size.Width,
								size.Height, string.Join(",", inlineAd.AddFormats.Cast<byte>()), (int) inlineAd.AdPosition,  inlineAd.BidFloor, 
								inlineAd.RefreshInterval, (int)inlineAd.AdScreenPosition, inlineAd.XCoord, inlineAd.YCoord,
								inlineAd.RespectSafeArea, JsonConvert.SerializeObject(extensions)
								, JsonConvert.SerializeObject(inlineAd.GetRequestModifiers()), showAd);
						}
					}
					break;
				}
				case AdType.Fullscreen:
				{
					if (nimbusAdUnit is FullscreenAd fullscreenAd)
					{
						#if NIMBUS_ENABLE_APS_IOS
						extensions.aps.slotData = nimbusAdUnit.ApsAds == null ? _apsIOS.GetAdUnitId(AdType.Fullscreen, 0, 0) :
							nimbusAdUnit.ApsAds;
						#endif
						if (fullscreenAd.Interstitial)
						{
							_interstitialAd(fullscreenAd.InstanceID, fullscreenAd.position, string.Join(",", fullscreenAd.AddFormats.Cast<byte>()),
								(int) fullscreenAd.Orientation, fullscreenAd.BidFloor,
								JsonConvert.SerializeObject(extensions), 
								JsonConvert.SerializeObject(fullscreenAd.GetRequestModifiers()), showAd);
						}
						else
						{
							_fullscreenAd(fullscreenAd.InstanceID, fullscreenAd.position, 
								(int)fullscreenAd.Orientation, JsonConvert.SerializeObject(extensions), 
								JsonConvert.SerializeObject(fullscreenAd.GetRequestModifiers()), showAd);
						}
					}
					break;
				}
				case AdType.Rewarded:
				{
					if (nimbusAdUnit is RewardedAd rewardedAd)
					{
						#if NIMBUS_ENABLE_APS_IOS
							extensions.aps.slotData = nimbusAdUnit.ApsAds == null ? _apsIOS.GetAdUnitId(AdType.Rewarded, 0, 0) :
									nimbusAdUnit.ApsAds;
						#endif
						_rewardedAd(rewardedAd.InstanceID, rewardedAd.position, (int) rewardedAd.Orientation, rewardedAd.BidFloor, 
							JsonConvert.SerializeObject(extensions), 
							JsonConvert.SerializeObject(rewardedAd.GetRequestModifiers()), showAd);
					}

					break;
				}
			}
		}

		internal override void ShowAd(Ad nimbusAdUnit)
		{
			var respectSafeArea = false;
			var adScreenPosition = AdScreenPosition.BOTTOM_CENTER;
			var rect = new Rectangle(0, 0, 0, 0);
			if (nimbusAdUnit is InlineAd inlineAd)
			{
				if (inlineAd.DynamicUnit)
				{
					rect.X = inlineAd.XCoord;
					rect.Y = inlineAd.YCoord;
					rect.Width = inlineAd.DynamicUnitWidth;
					rect.Height = inlineAd.DynamicUnitHeight;
				}
				else
				{
					var wh = inlineAd.AdSize.ToWidthAndHeight();
					rect.X = inlineAd.XCoord;
					rect.Y = inlineAd.YCoord;
					rect.Width = wh.Width;
					rect.Height = wh.Height;
				}
				respectSafeArea = inlineAd.RespectSafeArea;
				adScreenPosition = inlineAd.AdScreenPosition;
			}
			_showAd(nimbusAdUnit.InstanceID, rect.Width, rect.Height, respectSafeArea, (int) adScreenPosition, rect.X, rect.Y);
		}
	}
#endif
}