using System;
using System.Drawing;
using System.Linq;
using AdsByNimbus;
using AdsByNimbus.Internal.Extensions.AdMob;
using AdsByNimbus.Internal.Extensions.APS;
using Newtonsoft.Json;
using ScriptableObjects;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
#if UNITY_ANDROID
namespace AdsByNimbus.Internal {
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
			var extensions = new Internal.Extensions.Extensions();
			
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
			var extensions = new Internal.Extensions.Extensions();
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
						var size = inlineAd.AdSize.ToWidthAndHeight();
						#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
						extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Inline, size.Width, size.Height);
						#endif
						if (inlineAd.DynamicUnit)
						{
							_manager.CallStatic("dynamicUnit", _currentActivity, 
								inlineAd.InstanceID, inlineAd.position, string.Join(",", inlineAd.AddFormats.Cast<byte>()), 
								(int) inlineAd.AdPosition, inlineAd.BidFloor, inlineAd.RefreshInterval, inlineAd.DynamicUnitWidth, inlineAd.DynamicUnitHeight,
								(int) inlineAd.AdScreenPosition, inlineAd.XCoord, inlineAd.YCoord, inlineAd.RespectSafeArea, JsonConvert.SerializeObject(extensions), 
								JsonConvert.SerializeObject(inlineAd.GetRequestModifiers()), showAd);
						}
						else
						{
							_manager.CallStatic("bannerAd", _currentActivity, 
								inlineAd.InstanceID, inlineAd.position, size.Width,
								size.Height, string.Join(",", inlineAd.AddFormats.Cast<byte>()), (int) inlineAd.AdPosition, 
								inlineAd.BidFloor, inlineAd.RefreshInterval, (int) inlineAd.AdScreenPosition, 
								inlineAd.XCoord, inlineAd.YCoord, 
								inlineAd.RespectSafeArea, JsonConvert.SerializeObject(extensions), 
								JsonConvert.SerializeObject(inlineAd.GetRequestModifiers()), showAd);
						}
					}

					break;
				}
				case AdType.Fullscreen:
				{
					if (nimbusAdUnit is FullscreenAd fullscreenAd)
					{
						#if NIMBUS_ENABLE_APS_ANDROID && UNITY_ANDROID
						extensions.apsSlotData = _apsAndroid.GetAdUnitId(AdType.Fullscreen, 0, 0);
						#endif
						if (fullscreenAd.Interstitial)
						{
							_manager.CallStatic("interstitialAd", _currentActivity,
								fullscreenAd.InstanceID, fullscreenAd.position, string.Join(",", fullscreenAd.AddFormats.Cast<byte>()), 
								(int) fullscreenAd.Orientation, fullscreenAd.BidFloor, JsonConvert.SerializeObject(extensions),
								JsonConvert.SerializeObject(fullscreenAd.GetRequestModifiers()), showAd);
						}
						else
						{
							_manager.CallStatic("fullscreenAd", _currentActivity,
								fullscreenAd.InstanceID, fullscreenAd.position, (int) fullscreenAd.Orientation, 
								JsonConvert.SerializeObject(extensions),
								JsonConvert.SerializeObject(fullscreenAd.GetRequestModifiers()), showAd);
						}

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
							rewardedAd.InstanceID, rewardedAd.position, (int) rewardedAd.Orientation, rewardedAd.BidFloor,
							JsonConvert.SerializeObject(extensions),
							JsonConvert.SerializeObject(rewardedAd.GetRequestModifiers()), showAd);
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
			var respectSafeArea = false;
			var adScreenPosition = AdScreenPosition.BOTTOM_CENTER;
			var rect = new Rectangle(-1, -1, 0, 0);
			if (nimbusAdUnit is InlineAd inlineAd)
			{
				if (inlineAd.DynamicUnit)
				{
					rect = new Rectangle(inlineAd.XCoord, inlineAd.YCoord, inlineAd.DynamicUnitWidth, inlineAd.DynamicUnitHeight);
				}
				else
				{
					var wh = inlineAd.AdSize.ToWidthAndHeight();
					rect = new Rectangle(inlineAd.XCoord, inlineAd.YCoord, wh.Width, wh.Height);
				}
				respectSafeArea = inlineAd.RespectSafeArea;
				adScreenPosition = inlineAd.AdScreenPosition;
			}
			_manager.CallStatic("showAd", _currentActivity, 
				nimbusAdUnit.InstanceID, rect.Width, rect.Height, 
				respectSafeArea, (int) adScreenPosition, rect.X, rect.Y);
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}
#endif
