using System;
using System.Collections.Generic;
using System.Linq;
using Nimbus.Internal.Interceptor;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.InMobi;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.Moloco;
using Nimbus.Internal.Interceptor.ThirdPartyDemand.UnityAds;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using DeviceType = OpenRTB.Enumerations.DeviceType;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace Nimbus.Internal {
	public class Android : NimbusAPI {
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

		private Device _deviceCache;
		private AndroidJavaClass _helper;
		private AndroidJavaClass _nimbus;
		private AndroidJavaClass _unityPlayer;
		private string _sessionId;
		
		// ThirdParty Providers
		private List<IInterceptor> _interceptors;

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

			var androidLogger = new AndroidJavaObject(AndroidLogger, 0);
			//_nimbus.CallStatic("addLogger", androidLogger);
			/*_nimbus.CallStatic("initialize", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim());*/
			_helper.CallStatic("initNimbusAndThirdParties", _currentActivity, configuration.publisherKey.Trim(),
				configuration.apiKey.Trim());
			if (StaticMethod.InitializeInterceptor()) {
				_interceptors = new List<IInterceptor>();		
			}

			#if NIMBUS_ENABLE_APS
				var (apsAppID, slots, timeout) = configuration.GetApsData();
				var aps = new ApsAndroid(_currentActivity, apsAppID, slots, configuration.enableSDKInTestMode, timeout);
				aps.InitializeNativeSDK();
				_interceptors.Add(aps);
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				var vungleAppId = configuration.GetVungleData();
				Debug.unityLogger.Log(vungleAppId);
				var vungle = new VungleAndroid(applicationContext, vungleAppId);
				vungle.InitializeNativeSDK();
				_interceptors.Add(vungle);
			#endif
			#if NIMBUS_ENABLE_META
				var metaAppId = configuration.GetMetaData();
				var meta = new MetaAndroid(_currentActivity, configuration.enableSDKInTestMode, metaAppId);
				meta.InitializeNativeSDK();
				_interceptors.Add(meta);
			#endif
			#if NIMBUS_ENABLE_ADMOB
				var adMobAdUnitIds = configuration.GetAdMobData();
				var admob = new AdMobAndroid(adMobAdUnitIds);
				_interceptors.Add(admob);
			#endif
			#if NIMBUS_ENABLE_MINTEGRAL
				var (mintegralAppID, mintegralAppKey) = configuration.GetMintegralData();
				var mintegral = new MintegralAndroid(applicationContext, mintegralAppID, mintegralAppKey);
				mintegral.InitializeNativeSDK();
				_interceptors.Add(mintegral);
			#endif
			#if NIMBUS_ENABLE_UNITY_ADS
				var unityAdsGameId = configuration.GetUnityAdsData();
				var unityAds = new UnityAdsAndroid(applicationContext, unityAdsGameId);
				unityAds.InitializeNativeSDK();
				_interceptors.Add(unityAds);
			#endif
			#if NIMBUS_ENABLE_MOBILEFUSE
				var mobileFuse = new MobileFuseAndroid();
				// No Initialization Needed
				_interceptors.Add(mobileFuse);
			#endif
			#if NIMBUS_ENABLE_MOLOCO
				Debug.unityLogger.Log("Initializing Android Moloco SDK");
				var molocoAppKey = configuration.GetMolocoData();
				var moloco = new MolocoAndroid(applicationContext, molocoAppKey);
				moloco.InitializeNativeSDK();
				_interceptors.Add(moloco);
			#endif
			#if NIMBUS_ENABLE_INMOBI
				Debug.unityLogger.Log("Initializing Android InMobi SDK");
				var inMobiAccountId = configuration.GetInMobiData();
				var inMobi = new InMobiAndroid(applicationContext, inMobiAccountId);
				inMobi.InitializeNativeSDK();
				_interceptors.Add(inMobi);
			#endif
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

		internal override List<IInterceptor> Interceptors() {
			return _interceptors;
		}

		private static AndroidJavaObject CastToJavaObject(AndroidJavaObject source, string className) {
			var clazz = new AndroidJavaClass("java.lang.Class");
			var destClass = clazz.CallStatic<AndroidJavaObject>("forName", className);
			return destClass.Call<AndroidJavaObject>("cast", source);
		}
	}
}