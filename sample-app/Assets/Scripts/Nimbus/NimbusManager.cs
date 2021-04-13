using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nimbus.Internal;
using Nimbus.ScriptableObjects;
using UnityEngine;


namespace Nimbus {
	[DisallowMultipleComponent]
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
			configuration.ValidateMobileData();
			
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

		
		/// <summary>
		/// Calls to Nimbus for a 320x50 banner ad and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">This is a Nimbus specific field, it must not be empty and it represents a generic placement name can be used by the Nimbus dashboard to breakout data</param>
		public NimbusAdUnit LoadAndShowBannerAd(string position) {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, position, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		/// <summary>
		/// Calls to Nimbus for a full screen ad, this can either be a static or video ad depending on which wins the auction, and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">This is a Nimbus specific field, it must not be empty and it represents a generic placement name can be used by the Nimbus dashboard to breakout data</param>
		public NimbusAdUnit LoadAndShowInterstitialAd(string position) {
			var adUnit = new NimbusAdUnit(AdUnityType.Interstitial,   position, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		/// <summary>
		/// Calls to Nimbus for a video only ad and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">This is a Nimbus specific field, it must not be empty and it represents a generic placement name can be used by the Nimbus dashboard to breakout data</param>
		public NimbusAdUnit LoadAndShowRewardedVideoAd(string position) {
			var adUnit = new NimbusAdUnit(AdUnityType.Rewarded, position, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}
		
		/// <summary>
		/// Calls to Nimbus on a set timer for a 320x50 banner ad and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">This is a Nimbus specific field, it must not be empty and it represents a generic placement name can be used by the Nimbus dashboard to breakout data</param>
		/// <param name="refreshIntervalInSeconds">Specifies the rate at which banner ads should be called for. This defaults to the industry standard and best practice of 30 seconds</param>
		public IEnumerator LoadAndShowBannerAdWithRefresh(string position, float refreshIntervalInSeconds=30f) {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, position, in NimbusEvents);
			_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
			while (true) {
				yield return new WaitForSeconds(refreshIntervalInSeconds);
				adUnit?.Destroy();
				adUnit = new NimbusAdUnit(AdUnityType.Banner, position, in NimbusEvents);
				_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
			}
		}
	}
}