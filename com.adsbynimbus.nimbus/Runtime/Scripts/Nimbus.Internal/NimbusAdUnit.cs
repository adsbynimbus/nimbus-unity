using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Extensions;
using Nimbus.Internal.Utility;
using UnityEngine;

namespace Nimbus.Internal {
	internal delegate void DestroyAdDelegate(int adUnityInstanceId);

	public sealed class NimbusAdUnit {
		public readonly AdType AdType;
		public bool RespectSafeArea;
		public IabSupportedAdSizes BannerSize;
		public string ErrResponse;
		public string NimbusReportingPosition;
		public int BannerRefreshIntervalInSeconds;
		public NimbusAdUnitPosition AdPosition;
		public AdEventTypes CurrentAdState { get; private set; } = AdEventTypes.NOT_LOADED; 
		public readonly int InstanceID;
		private bool _adCompleted;
		private bool _adWasReturned;
		private readonly AdEvents _adEvents;
		internal bool AdWasRendered;
		public float BannerBidFloor;
		public float VideoBidFloor;
		public RequestModifiers? RequestModifiers;

		internal Task<string> Request = Task.FromResult("");
		
		public NimbusAdUnit(AdType adType, in AdEvents adEvents, string nimbusReportingPosition, IabSupportedAdSizes adSize = IabSupportedAdSizes.Banner, bool respectSafeArea = false, 
			NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, int bannerRefreshIntervalInSeconds = 30, float bannerBidFloor = 0f,
			float videoBidFloor = 0f, RequestModifiers? modifiers = null)
		{
			NimbusReportingPosition = nimbusReportingPosition;
			AdType = adType;
			InstanceID = GetHashCode();
			_adEvents = adEvents;
			BannerSize = adSize;
			RespectSafeArea = respectSafeArea;
			AdPosition = adPosition;
			BannerRefreshIntervalInSeconds = bannerRefreshIntervalInSeconds;
			BannerBidFloor = bannerBidFloor;
			VideoBidFloor = videoBidFloor;
			RequestModifiers = modifiers;
		}

		# region IOS specific

#pragma warning disable 67
		internal event DestroyAdDelegate OnDestroyIOSAd;
#pragma warning restore 67

		#endregion
		
		/// <summary>
		///     Destroys the ad at the mobile bridge level
		/// </summary>
		public void Destroy() {
			#if UNITY_ANDROID
			var managerClass = new AndroidJavaObject("com.adsbynimbus.unity.NimbusManager");
			var instance = managerClass.GetStatic<AndroidJavaObject> ("INSTANCE");
			//if (_androidController == null || _androidHelper == null) return;
			instance.CallStatic("destroyAd", InstanceID);
			_androidController = null;
			_androidHelper = null;
			# elif UNITY_IOS
			OnDestroyIOSAd?.Invoke(InstanceID);
			#endif
		}

		/// <summary>
		///     Returns returns true of the ad was rendered even if the ad has already been destroyed
		/// </summary>
		public bool WasAdRendered() {
			return AdWasRendered;
		}

		public bool WasAnAdReturned() {
			return _adWasReturned;
		}

		internal void FireMobileAdRenderedEvent() {
			_adEvents.FireOnAdRenderedEvent(this);
		}

		internal void FireMobileOnAdErrorEvent() {
			_adEvents.FireOnAdErrorEvent(this);
		}
		
		internal void FireMobileAdEvents(AdEventTypes e) {
			CurrentAdState = e;
			switch (e) {
				case AdEventTypes.LOADED:
					_adEvents.FireOnAdLoadedEvent(this);
					break;
				case AdEventTypes.IMPRESSION:
					_adEvents.FireOnAdImpressionEvent(this);
					break;
				case AdEventTypes.CLICKED:
					_adEvents.FireOnAdClickedEvent(this);
					break;
				case AdEventTypes.PAUSED:
					_adEvents.FireOnVideoAdPausedEvent(this);
					break;
				case AdEventTypes.RESUMED:
					_adEvents.FireOnVideoAdResumeEvent(this);
					break;
				case AdEventTypes.REWARDEARNED:
					_adEvents.FireOnAdRewardEarnedEvent(this);
					break;
				case AdEventTypes.COMPLETED:
					_adCompleted = true;
					// ensure that video ads auto close to avoid a black screen when the ad completes
					if (AdType == AdType.Interstitial) {
						Destroy();
					}
					break;
				case AdEventTypes.DESTROYED:
					// ReSharper disable once ConvertIfStatementToSwitchStatement
					if (AdType == AdType.Rewarded) {
						_adEvents.FireOnAdCompletedEvent(this, !_adCompleted);
					} else if (AdType == AdType.Interstitial) {
						// fired the completed event for interstitial ads force skipped to false everytime, since you
						// can skip after a set time
						_adEvents.FireOnAdCompletedEvent(this, false);
					}
					// always call destroyed the destroyed event
					_adEvents.FireOnAdDestroyedEvent(this);
					break;
				default:
					Debug.unityLogger.LogWarning("Nimbus",$"uncaught mobile event {e}");
					break;
			}
		}

		internal void SetAndroidController(AndroidJavaObject controller) {
			if (_androidController != null) return;
			_androidController = controller;
		}

		internal void SetAndroidHelper(AndroidJavaClass helper) {
			if (_androidHelper != null) return;
			_androidHelper = helper;
		}

		#region Android Specific

		private AndroidJavaObject _androidController;
		private AndroidJavaObject _androidHelper;

		#endregion
	}
	public enum NimbusAdUnitPosition
	{
		BOTTOM_CENTER = 0,
		TOP_CENTER = 1,
		CENTER = 2,
		BOTTOM_LEFT = 3,
		BOTTOM_RIGHT = 4,
		TOP_LEFT = 5,
		TOP_RIGHT = 6,
	}
	
	public enum IabSupportedAdSizes : byte {
		Banner,
		FullScreenPortrait,
		FullScreenLandscape,
		HalfScreen,
		Letterbox,
		LeaderBoard
	}

	public static class IabSupportedAdSizesExtension {
		public static Tuple<int, int> ToWidthAndHeight(this IabSupportedAdSizes isa) {
			switch (isa) {
				case IabSupportedAdSizes.Banner:
					return new Tuple<int, int>(320, 50);
				case IabSupportedAdSizes.FullScreenPortrait:
					return new Tuple<int, int>(320, 480);
				case IabSupportedAdSizes.FullScreenLandscape:
					return new Tuple<int, int>(480, 320);
				case IabSupportedAdSizes.HalfScreen:
					return new Tuple<int, int>(300, 600);
				case IabSupportedAdSizes.Letterbox:
					return new Tuple<int, int>(300, 250);
				case IabSupportedAdSizes.LeaderBoard:
					return new Tuple<int, int>(728, 90);
				default:
					return new Tuple<int, int>(0, 0);
			}
		}
	}
	public enum AdType : byte {
		Banner = 0,
		Interstitial = 1,
		Rewarded = 2
	}

}