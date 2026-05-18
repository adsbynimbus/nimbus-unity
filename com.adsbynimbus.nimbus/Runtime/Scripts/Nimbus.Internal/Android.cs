using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Nimbus.Internal.Extensions;
using Nimbus.Internal.Extensions.AdMob;
using Nimbus.Internal.Extensions.APS;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Nimbus.Internal {
	public class Android : NimbusAPI {
		// ThirdParty Providers
		#if NIMBUS_ENABLE_ADMOB_IOS
				private AdMobAndroid _adMobAndroid;
		#endif
		#if NIMBUS_ENABLE_APS_IOS
				private ApsAndroid _apsAndroid;
		#endif
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

		private AndroidJavaClass _helper;
		private AndroidJavaClass _nimbus;
		private AndroidJavaClass _unityPlayer;
		private string _sessionId;
		
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Initializing Android SDK");
			_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			_currentActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_nimbus = new AndroidJavaClass(NimbusPackage);
			_helper = new AndroidJavaClass(HelperClass);
			_connectionTypeHelper = new AndroidJavaClass(ConnectionHelper);
			_build = new AndroidJavaClass(AndroidBuild);
			_buildVersion = new AndroidJavaClass(AndroidBuildVersion);
			var applicationContext = _currentActivity.Call<AndroidJavaObject>("getApplicationContext");

			var extensions = new Nimbus.Internal.Extensions.Extensions();
			
			#if NIMBUS_ENABLE_APS
				var (apsAppID, slots) = configuration.GetApsData();
				_apsAndroid = new ApsAndroid(_currentActivity, apsAppID, slots, configuration.enableSDKInTestMode, 0);
				extensions.aps.appKey = apsAppID;
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				extensions.vungle.appId = configuration.GetVungleData();
			#endif
			#if NIMBUS_ENABLE_META
				extensions.meta.appId = configuration.GetMetaData();
				extensions.meta.forceTestAd = configuration.enableSDKInTestMode;
			#endif
			#if NIMBUS_ENABLE_ADMOB
				var adMobAdUnitIds = configuration.GetAdMobData();
				_adMobAndroid = new AdMobAndroid(adMobAdUnitIds);
			#endif
			#if NIMBUS_ENABLE_MINTEGRAL
				var (mintegralAppID, mintegralAppKey) = configuration.GetMintegralData();
				extensions.mintegral.appId = mintegralAppID;
				extensions.mintegral.appKey = mintegralAppKey;
			#endif
			#if NIMBUS_ENABLE_UNITY_ADS
				extensions.unityAds.gameId = configuration.GetUnityAdsData();
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				extensions.moloco.appKey = configuration.GetMolocoData();
			#endif
			#if NIMBUS_ENABLE_INMOBI
				extensions.inMobi.accountId = configuration.GetInMobiData();
			#endif
			
			_helper.CallStatic("initNimbusAndThirdParties", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim(), JsonConvert.SerializeObject(extensions));
		}


		internal override void getAd(NimbusAdUnit nimbusAdUnit, bool showAd) {
			const string functionCall = "render";
			var holdTime = 0;
			var shouldBlock = nimbusAdUnit.AdType != AdType.Banner;
			//var listener = new AdManagerListener(in _helper, ref nimbusAdUnit);

			/*f (nimbusAdUnit.AdType == AdUnitType.Interstitial || nimbusAdUnit.AdType == AdUnitType.Rewarded) {
				shouldBlock = true;
				holdTime = 5;
				if (nimbusAdUnit.AdType == AdUnitType.Rewarded) holdTime = (int)TimeSpan.FromMinutes(60).TotalSeconds;
			}*/
			_helper.CallStatic(functionCall, _currentActivity, shouldBlock, (nimbusAdUnit.AdType == AdType.Rewarded), holdTime,
				null,"", "","","", true, 0);
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}