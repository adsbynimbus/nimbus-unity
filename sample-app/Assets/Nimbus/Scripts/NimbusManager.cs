using System;
using System.Collections;
using System.Linq;
using Nimbus.Scripts.Internal;
using Nimbus.Scripts.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Scripts {
	[DisallowMultipleComponent]
	public class NimbusManager : MonoBehaviour {
		#region Editor Values
		public NimbusSDKConfiguration configuration;
		public bool shouldSubscribeToIAdEvents;
		#endregion
		
		public delegate void SetAdUnitFromCoroutine(NimbusAdUnit adUnit);

		public static NimbusManager Instance;

		private AdEvents _nimbusEvents;
		private NimbusAPI _nimbusPlatformAPI;

		private void Awake() {
			if (configuration == null) throw new Exception("The configuration object cannot be null");
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
				_nimbusEvents = new AdEvents();
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
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance._nimbusEvents.OnAdRendered += iAdEvent.AdWasRendered;
				Instance._nimbusEvents.OnAdError += iAdEvent.AdError;
				Instance._nimbusEvents.OnAdEvent += iAdEvent.AdEvent;
			}

			yield return null;
		}

		private void OnDestroy() {
			if (!shouldSubscribeToIAdEvents) return;
			var iAdEvents = FindObjectsOfType<MonoBehaviour>().OfType<IAdEvents>();
			foreach (var iAdEvent in iAdEvents) {
				Instance._nimbusEvents.OnAdRendered -= iAdEvent.AdWasRendered;
				Instance._nimbusEvents.OnAdError -= iAdEvent.AdError;
				Instance._nimbusEvents.OnAdEvent -= iAdEvent.AdEvent;
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
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, position, bannerBidFloor, 0f, in _nimbusEvents);
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
			var adUnit = new NimbusAdUnit(AdUnityType.Interstitial, position, bannerBidFloor, videoBidFloor, in _nimbusEvents);
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
			var adUnit = new NimbusAdUnit(AdUnityType.Rewarded, position, 0, videoBidFloor, in _nimbusEvents);
			return _nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
		}

		/// <summary>
		///     Calls to Nimbus on a set timer for a 320x50 banner ad and loads the ad if an ad is returned from the auction
		/// </summary>
		/// <param name="position">
		///     This is a Nimbus specific field, it must not be empty and it represents a generic placement name
		///     can be used by the Nimbus dashboard to breakout data
		/// </param>
		/// <param name="bannerBidFloor">
		///     Represents your asking price for banner ads during the auction
		/// </param>
		/// <param name="currentAdUnit">Provides the ability to pass the current ad unit back out of the coroutine</param>
		/// <param name="refreshIntervalInSeconds">
		///     Specifies the rate at which banner ads should be called for. This defaults to
		///     the industry standard and best practice of 30 seconds
		/// </param>
		public IEnumerator LoadAndShowBannerAdWithRefresh(string position, float bannerBidFloor, SetAdUnitFromCoroutine currentAdUnit,
			float refreshIntervalInSeconds = 30f) {
			var adUnit = new NimbusAdUnit(AdUnityType.Banner, position, bannerBidFloor, 0, in _nimbusEvents);
			_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
			while (true) {
				yield return new WaitForSeconds(refreshIntervalInSeconds);
				adUnit?.Destroy();
				adUnit = new NimbusAdUnit(AdUnityType.Banner, position, bannerBidFloor, 0, in _nimbusEvents);
				currentAdUnit(adUnit);
				_nimbusPlatformAPI.LoadAndShowAd(Debug.unityLogger, ref adUnit);
			}
		}

	
	}
}