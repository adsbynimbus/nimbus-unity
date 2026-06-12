using System;
using Newtonsoft.Json;
using Nimbus.Internal.Extensions.AdMob;
using Nimbus.Internal.Extensions.APS;
using Nimbus.ScriptableObjects;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

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
		private const string HelperClass = "com.adsbynimbus.unity.UnityHelper";
		private const string NimbusPackage = "com.adsbynimbus.Nimbus";
		private AndroidJavaClass _build;
		private AndroidJavaClass _buildVersion;
		private AndroidJavaObject _currentActivity;


		private AndroidJavaObject _helper;
		private AndroidJavaClass _unityPlayer;
		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var helperClass = new AndroidJavaObject(HelperClass);
			_helper = helperClass.GetStatic<AndroidJavaObject> ("INSTANCE");
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
			
			_helper.CallStatic("initNimbusAndThirdParties", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim(), configuration.enableSDKInTestMode, JsonConvert.SerializeObject(extensions));
		}


		internal override void GetAd(NimbusAdUnit nimbusAdUnit, bool showAd) {
			var extensions = new Nimbus.Internal.Extensions.Extensions();
			NimbusCallbackReceiver.Instance.AddAdUnit(nimbusAdUnit);
			#if NIMBUS_ENABLE_ADMOB_ANDROID && UNITY_ANDROID
				extensions.adMobAdUnitIds = _adMobAndroid.GetAdUnitId(nimbusAdUnit.AdType);
			#endif
			switch (nimbusAdUnit.AdType)
			{
				case AdType.Banner:
				{
					var size = nimbusAdUnit.BannerSize.ToWidthAndHeight();
					#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
						extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Banner, size.Item1, size.Item2);
					#endif
					_helper.CallStatic("bannerAd", _currentActivity, 
						nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, size.Item1,
						size.Item2, nimbusAdUnit.BannerRefreshIntervalInSeconds, nimbusAdUnit.BannerBidFloor, nimbusAdUnit.RespectSafeArea, 
						(int) nimbusAdUnit.AdPosition, showAd, JsonConvert.SerializeObject(extensions));
					break;
				}
				case AdType.Interstitial:
				{
					#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
						extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Interstitial, 0, 0);
					#endif
					_helper.CallStatic("interstitialAd", _currentActivity, 
						nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, nimbusAdUnit.BannerBidFloor, 
						nimbusAdUnit.VideoBidFloor, showAd, JsonConvert.SerializeObject(extensions));
					break;
				}
				case AdType.Rewarded:
				{
					#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
					extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Rewarded, 0, 0);
					#endif
					_helper.CallStatic("rewardedAd", _currentActivity, 
						nimbusAdUnit.InstanceID, nimbusAdUnit.NimbusReportingPosition, nimbusAdUnit.VideoBidFloor,
						showAd, JsonConvert.SerializeObject(extensions));
					break;
				}
			}
		}

		internal override void ShowAd(NimbusAdUnit nimbusAdUnit)
		{
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var helperClass = new AndroidJavaObject(HelperClass);
			_helper = helperClass.GetStatic<AndroidJavaObject> ("INSTANCE");
			var size = nimbusAdUnit.BannerSize.ToWidthAndHeight();
			_helper.CallStatic("showAd", _currentActivity, 
				nimbusAdUnit.InstanceID, size.Item1, size.Item2,
				nimbusAdUnit.RespectSafeArea, (int) nimbusAdUnit.AdPosition);
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}
