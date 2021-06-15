using System;
using System.Collections;
using System.Linq;
using Nimbus.Runtime.Scripts.Internal;
using Nimbus.Runtime.Scripts.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Runtime.Scripts {
	[DisallowMultipleComponent]
	public class NimbusManager : MonoBehaviour {
		#region Editor Values
		public NimbusSDKConfiguration configuration;
		public bool shouldSubscribeToIAdEvents;
		#endregion
		
		// ReSharper disable once MemberCanBePrivate.Global
		public AdEvents NimbusEvents;
		
		public static NimbusManager Instance;
		private NimbusAPI _nimbusPlatformAPI;
		private NimbusAdUnit _refreshAd;
		private bool _canStartBannerRefreshing =  true;
		
		private void Awake() {
			if (configuration == null) throw new Exception("The configuration object cannot be null");
			
			if (Instance == null) {
				_nimbusPlatformAPI = _nimbusPlatformAPI ?? new
#if UNITY_EDITOR
					Editor
#elif UNITY_ANDROID
                Android
#else   
                IOS
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
			if (!shouldSubscribeToIAdEvents) yield break;
			yield return new WaitForEndOfFrame();
			var eventsFound = false;
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance.NimbusEvents.OnAdRendered += iAdEvent.OnAdWasRendered;
				Instance.NimbusEvents.OnAdError += iAdEvent.OnAdError;
				Instance.NimbusEvents.OnAdLoaded += iAdEvent.OnAdLoaded;
				Instance.NimbusEvents.OnAdImpression += iAdEvent.OnAdImpression;
				Instance.NimbusEvents.OnAdClicked += iAdEvent.OnAdClicked;
				Instance.NimbusEvents.OnAdDestroyed += iAdEvent.OnAdDestroyed;
				Instance.NimbusEvents.OnVideoAdPaused += iAdEvent.OnVideoAdPaused;
				Instance.NimbusEvents.OnVideoAdResume += iAdEvent.OnVideoAdResume;
				Instance.NimbusEvents.OnVideoAdCompleted += iAdEvent.OnVideoAdCompleted;
				eventsFound = true;
			}

			if (!eventsFound)
				Debug.unityLogger.LogWarning(
					"Manager is set to auto subscribe to listeners, however there are no instances of IAdEvents in the scene",
					this);

			yield return null;
		}

		private void OnDestroy() {
			if (!shouldSubscribeToIAdEvents) return;
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance.NimbusEvents.OnAdRendered -= iAdEvent.OnAdWasRendered;
				Instance.NimbusEvents.OnAdError -= iAdEvent.OnAdError;
				Instance.NimbusEvents.OnAdLoaded += iAdEvent.OnAdLoaded;
				Instance.NimbusEvents.OnAdImpression += iAdEvent.OnAdImpression;
				Instance.NimbusEvents.OnAdClicked += iAdEvent.OnAdClicked;
				Instance.NimbusEvents.OnAdDestroyed += iAdEvent.OnAdDestroyed;
				Instance.NimbusEvents.OnVideoAdPaused += iAdEvent.OnVideoAdPaused;
				Instance.NimbusEvents.OnVideoAdResume += iAdEvent.OnVideoAdResume;
				Instance.NimbusEvents.OnVideoAdCompleted += iAdEvent.OnVideoAdCompleted;
			}
		}


		/// <summary>
		///     Calls to Nimbus for a 320x50 banner ad and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">
		///     This is a Nimbus specific field, it must not be empty and it represents a generic placement name
		///     can be used by the Nimbus dashboard to breakout data
		/// </param>
		/// <param name="bannerBidFloor">
		///     Represents your asking price for banner ads during the auction
		/// </param>
		public NimbusAdUnit LoadAndShowBannerAd(string position, float bannerBidFloor) {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, position, bannerBidFloor, 0f, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		/// <summary>
		///     Calls to Nimbus for a full screen ad, this can either be a static or video ad depending on which wins the auction,
		///     and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">
		///     This is a Nimbus specific field, it must not be empty and it represents a generic placement name
		///     can be used by the Nimbus dashboard to breakout data
		/// </param>
		/// <param name="bannerBidFloor">
		///     Represents your asking price for banner ads during the auction
		/// </param>
		/// <param name="videoBidFloor">
		///     Represents your asking price for video ads during the auction
		/// </param>
		public NimbusAdUnit LoadAndShowFullScreenAd(string position, float bannerBidFloor, float videoBidFloor) {
			var adUnit = new NimbusAdUnit(AdUnityType.Interstitial, position, bannerBidFloor, videoBidFloor,
				in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		/// <summary>
		///     Calls to Nimbus for a video only ad and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">
		///     This is a Nimbus specific field, it must not be empty and it represents a generic placement name
		///     can be used by the Nimbus dashboard to breakout data
		/// </param>
		/// <param name="videoBidFloor">
		///     Represents your asking price for video ads during the auction
		/// </param>
		public NimbusAdUnit LoadAndShowRewardedVideoAd(string position, float videoBidFloor) {
			var adUnit = new NimbusAdUnit(AdUnityType.Rewarded, position, 0, videoBidFloor, in NimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		/// <summary>
		///     Calls to Nimbus on a set timer for a 320x50 banner ad and loads the ad if an ad is returned from the auction.
		///     Note: there can only be 1 refreshing banner ad per application session
		/// </summary>
		/// <param name="position">
		///     This is a Nimbus specific field, it must not be empty and it represents a generic placement name
		///     can be used by the Nimbus dashboard to breakout data
		/// </param>
		/// <param name="bannerBidFloor">
		///     Represents your asking price for banner ads during the auction
		/// </param>
		/// <param name="refreshIntervalInSeconds">
		///     Specifies the rate at which banner ads should be called for. This defaults to
		///     the industry standard and best practice of 30 seconds
		/// </param>
		public IEnumerator LoadAndShowBannerAdWithRefresh(string position, float bannerBidFloor, float refreshIntervalInSeconds = 30f) {
			if (!_canStartBannerRefreshing) throw new Exception("LoadAndShowBannerAdWithRefresh() is currently running a coroutine, only 1 running coroutine can be called at a time");
			_canStartBannerRefreshing = false;
			_refreshAd = new NimbusAdUnit(AdUnityType.Banner, position, bannerBidFloor, 0, in NimbusEvents);
			_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref _refreshAd);
			while (true) {
				yield return new WaitForSeconds(refreshIntervalInSeconds);
				_refreshAd?.Destroy();
				_refreshAd = new NimbusAdUnit(AdUnityType.Banner, position, bannerBidFloor, 0, in NimbusEvents);
				_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref _refreshAd);
			}
			// ReSharper disable once IteratorNeverReturns
		}
		
		/// <summary>
		///     Cleans up any ads that were created as a result of refreshing banner ads on a timer.
		///     This also resets the internal state and allows the LoadAndShowBannerAdWithRefresh to called again
		///     without throwing an exception
		/// </summary>
		/// <param name="refreshBannerCoroutine">
		///     The coroutine that was returned from calling LoadAndShowBannerAdWithRefresh
		/// </param>
		public void StopRefreshBannerAd(IEnumerator refreshBannerCoroutine) {
			StopCoroutine(refreshBannerCoroutine);
			_refreshAd?.Destroy();
			_canStartBannerRefreshing = true;
		}

		// ReSharper disable once InconsistentNaming
		/// <summary>
		///     Allows the TCF GDPR consent string to be set globally on all request to Nimbus
		/// </summary>
		/// <param name="consent">
		///     This is the TCF GDPR consent string
		/// </param>
		public void SetGDPRConsentString(string consent) {
			_nimbusPlatformAPI.SetGDPRConsentString(consent);
		}
	}
}