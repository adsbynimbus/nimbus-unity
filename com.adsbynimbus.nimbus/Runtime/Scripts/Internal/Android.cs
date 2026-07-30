using System;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
#if UNITY_ANDROID
namespace Nimbus.Internal {
	public class Android : NimbusAPI {
		// ThirdParty Providers
		#if NIMBUS_ENABLE_ADMOB
			private AdMobAndroid _adMobAndroid;
		#endif
		#if NIMBUS_ENABLE_APS
			private ApsAndroid _apsAndroid;
		#endif
		private const string AndroidBuild = "android.os.Build";
		private const string AndroidBuildVersion = "android.os.Build$VERSION";
		private const string AndroidLogger = "com.adsbynimbus.Nimbus$Logger$Default";
		private const string ManagerClass = "com.adsbynimbus.unity.NimbusManager";
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private AndroidJavaClass _build;
		private AndroidJavaClass _buildVersion;
		private AndroidJavaObject _currentActivity;


		private AndroidJavaObject _manager;
		private AndroidJavaClass _unityPlayer;
		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var managerClass = new AndroidJavaObject(ManagerClass);
			_manager = managerClass.GetStatic<AndroidJavaObject> ("INSTANCE");
			var extensions = new Nimbus.Internal.Extensions.Extensions();
			
			#if NIMBUS_ENABLE_APS
				var (apsAppID, slots) = configuration.GetApsData();
				_apsAndroid = new ApsAndroid(_currentActivity, apsAppID, slots, configuration.enableSDKInTestMode, 0);
				extensions.apsAppKey = apsAppID;
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				extensions.vungleAppId = configuration.GetVungleData();
			#endif
			#if NIMBUS_ENABLE_META
				extensions.metaAppId = configuration.GetMetaData();
				extensions.metaForceTestAd = configuration.enableSDKInTestMode;
			#endif
			#if NIMBUS_ENABLE_ADMOB
				var adMobAdUnitIds = configuration.GetAdMobData();
				_adMobAndroid = new AdMobAndroid(adMobAdUnitIds);
			#endif
			#if NIMBUS_ENABLE_MINTEGRAL
				var (mintegralAppID, mintegralAppKey) = configuration.GetMintegralData();
				extensions.mintegralAppId = mintegralAppID;
				extensions.mintegralAppKey = mintegralAppKey;
			#endif
			#if NIMBUS_ENABLE_UNITY_ADS
				extensions.unityAdsGameId = configuration.GetUnityAdsData();
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				extensions.molocoAppKey = configuration.GetMolocoData();
			#endif
			#if NIMBUS_ENABLE_INMOBI
				extensions.inMobiAccountId = configuration.GetInMobiData();
			#endif
			
			_manager.CallStatic("initNimbusAndThirdParties", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim(), configuration.enableSDKInTestMode, JsonConvert.SerializeObject(extensions));
		}


		internal override void GetAd(Ad nimbusAdUnit, bool showAd) {
			var extensions = new Nimbus.Internal.Extensions.Extensions();
			NimbusCallbackReceiver.Instance.AddAdUnit(nimbusAdUnit);
			#if NIMBUS_ENABLE_ADMOB_ANDROID && UNITY_ANDROID
				extensions.adMobAdUnitIds = _adMobAndroid.GetAdUnitId(nimbusAdUnit.AdType);
			#endif
			switch (nimbusAdUnit.AdType)
			{
				case AdType.Inline:
				{
					if (nimbusAdUnit is InlineAd inlineAd)
					{
						var size = inlineAd.BannerSize.ToWidthAndHeight();
						#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
						extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Inline, size.Item1, size.Item2);
						#endif
						_manager.CallStatic("bannerAd", _currentActivity, 
							inlineAd.InstanceID, inlineAd.NimbusReportingPosition, size.Item1,
							size.Item2, inlineAd.BannerRefreshIntervalInSeconds, inlineAd.BannerBidFloor, inlineAd.RespectSafeArea, 
							(int) inlineAd.AdPosition, showAd, JsonConvert.SerializeObject(extensions), 
							JsonConvert.SerializeObject(inlineAd.RequestModifiers));
					}

					break;
				}
				case AdType.Fullscreen:
				{
					if (nimbusAdUnit is FullscreenAd interstitialAd)
					{
						#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
						extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Fullscreen, 0, 0);
						#endif
						_manager.CallStatic("interstitialAd", _currentActivity,
							interstitialAd.InstanceID, interstitialAd.NimbusReportingPosition, interstitialAd.BannerBidFloor,
							interstitialAd.VideoBidFloor, showAd, JsonConvert.SerializeObject(extensions),
							JsonConvert.SerializeObject(interstitialAd.RequestModifiers));
					}

					break;
				}
				case AdType.Rewarded:
				{
					if (nimbusAdUnit is RewardedAd rewardedAd)
					{
					#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
					extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Rewarded, 0, 0);
					#endif
						_manager.CallStatic("rewardedAd", _currentActivity,
							rewardedAd.InstanceID, rewardedAd.NimbusReportingPosition, rewardedAd.VideoBidFloor,
							showAd, JsonConvert.SerializeObject(extensions),
							JsonConvert.SerializeObject(rewardedAd.RequestModifiers));
					}

					break;
				}
			}
		}

		internal override void ShowAd(Ad nimbusAdUnit)
		{
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var managerClass = new AndroidJavaObject(ManagerClass);
			_manager = managerClass.GetStatic<AndroidJavaObject> ("INSTANCE");
			var size = IabSupportedAdSizes.Banner.ToWidthAndHeight();
			var respectSafeArea = false;
			if (nimbusAdUnit is InlineAd inlineAd)
			{  
				size = inlineAd.BannerSize.ToWidthAndHeight();
				respectSafeArea = inlineAd.RespectSafeArea;
			}
			_manager.CallStatic("showAd", _currentActivity, 
				nimbusAdUnit.InstanceID, size.Item1, size.Item2,
				respectSafeArea, (int) nimbusAdUnit.AdPosition);
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}
#endif
