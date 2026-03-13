using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using UnityEngine;

namespace Nimbus.Internal {
	internal delegate void DestroyAdDelegate(int adUnityInstanceId);

	public sealed class NimbusAdUnit {
		public readonly AdUnitType AdType;
		public string adUnitId;
		public bool RespectSafeArea;
		public string ErrResponse;
		public NimbusAdUnitPosition AdPosition;
		public AdEventTypes CurrentAdState { get; private set; } = AdEventTypes.NOT_LOADED; 
		public readonly int InstanceID;
		private bool _adCompleted;
		private bool _adWasReturned;
		private readonly AdEvents _adEvents;

		internal bool AdWasRendered;
		internal string RawBidResponse;

		internal Task<string> Request = Task.FromResult("");
		
		public NimbusAdUnit(AdUnitType adType, in AdEvents adEvents, bool respectSafeArea = false, 
			NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER)
		{
			AdType = adType;
			InstanceID = GetHashCode();
			_adEvents = adEvents;
			RespectSafeArea = respectSafeArea;
			AdPosition = adPosition;
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
			if (_androidController == null || _androidHelper == null) return;
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			_androidHelper.CallStatic("destroyController", currentActivity, _androidController);
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
				case AdEventTypes.COMPLETED:
					_adCompleted = true;
					// ensure that video ads auto close to avoid a black screen when the ad completes
					if (AdType == AdUnitType.Interstitial) {
						Destroy();
					}
					break;
				case AdEventTypes.DESTROYED:
					// ReSharper disable once ConvertIfStatementToSwitchStatement
					if (AdType == AdUnitType.Rewarded) {
						_adEvents.FireOnAdCompletedEvent(this, !_adCompleted);
					} else if (AdType == AdUnitType.Interstitial) {
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

		
		internal async void LoadJsonResponseAsync(Task<string> jsonBody) {
			Request = Task.Run(async () => {
				var response = "";
				try
				{
					response = await jsonBody;
					_adWasReturned = true;
					RawBidResponse = response;
					Debug.unityLogger.Log("Nimbus", $"BID RESPONSE: {RawBidResponse}");
					_adEvents.FireOnAdLoadedEvent(this);
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log($"Bid Response Error: {e.Message}");
					_adEvents.FireOnAdErrorEvent(this);
				}
				return response;
			});
			await Request;
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
		private AndroidJavaClass _androidHelper;

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
		Banner300X50,
		Banner320X50,
		FullScreenPortrait,
		FullScreenLandscape,
		HalfScreen,
		Letterbox,
		LeaderBoard
	}

	public static class IabSupportedAdSizesExtension {
		public static Tuple<int, int> ToWidthAndHeight(this IabSupportedAdSizes isa) {
			switch (isa) {
				case IabSupportedAdSizes.Banner300X50:
					return new Tuple<int, int>(300, 50);
				case IabSupportedAdSizes.Banner320X50:
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

}