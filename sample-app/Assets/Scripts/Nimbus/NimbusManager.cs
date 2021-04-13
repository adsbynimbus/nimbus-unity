using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nimbus.Internal;
using Nimbus.ScriptableObjects;
using UnityEngine;


namespace Nimbus {
	public class NimbusManager : MonoBehaviour {
		public NimbusSDKConfiguration configuration;
		// ReSharper disable once MemberCanBePrivate.Global
		public static NimbusManager Instance;
		public bool shouldSubscribeToIAdEvents;
		// ReSharper disable once MemberCanBePrivate.Global
		public AdEvents NimbusEvents;
		private NimbusAPI _nimbusPlatformAPI;
		
		private readonly Dictionary<string, Queue<NimbusAdUnit>> _refreshedBanners = new Dictionary<string, Queue<NimbusAdUnit>>();

		private void Awake() {
			if (configuration == null) {
				throw new Exception("The configuration object cannot be null");
			}
			
			if (Instance == null) {
				_nimbusPlatformAPI ??= new
#if UNITY_EDITOR
					Editor
#elif UNITY_ANDROID
                Android
#else
   //TODO swap for IOS
                Android
#endif
					();
				NimbusEvents = new AdEvents();
				Debug.unityLogger.logEnabled = configuration.enableUnityLogs;
				_nimbusPlatformAPI.InitializeSDK(Debug.unityLogger, configuration);
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this) {
				Destroy(gameObject);
			}
		}

		private IEnumerator Start() {
			if (!shouldSubscribeToIAdEvents) yield return null;
			yield return new WaitForEndOfFrame();
			// find any mono behaviours that implement IAdEvents
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance.NimbusEvents.OnAdRendered += iAdEvent.AdWasRendered;
				Instance.NimbusEvents.OnAdError += iAdEvent.AdError;
				Instance.NimbusEvents.OnAdEvent += iAdEvent.AdEvent;	
			}
			yield return null;
		}

		private void OnDestroy() {
			if (!shouldSubscribeToIAdEvents) return;
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance.NimbusEvents.OnAdRendered -= iAdEvent.AdWasRendered;
				Instance.NimbusEvents.OnAdError -= iAdEvent.AdError;
				Instance.NimbusEvents.OnAdEvent -= iAdEvent.AdEvent;	
			}
		}

		public NimbusAdUnit LoadAndShowBannerAd() {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		public NimbusAdUnit LoadAndShowInterstitialAd() {
			var adUnit = new NimbusAdUnit(AdUnityType.Interstitial, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		public NimbusAdUnit LoadAndShowRewardedVideoAd() {
			var adUnit = new NimbusAdUnit(AdUnityType.Rewarded, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}
		
		public IEnumerator LoadAndShowBannerAdWithRefresh(float refreshIntervalInSeconds) {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, in NimbusEvents);
			_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
			while (true) {
				yield return new WaitForSeconds(refreshIntervalInSeconds);
				adUnit?.Destroy();
				adUnit = new NimbusAdUnit(AdUnityType.Banner, in NimbusEvents);
				_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
			}
		}
	}
}