using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenRTB.Response;
using UnityEngine;

namespace Nimbus.Internal {
	internal delegate void DestroyAdDelegate(int adUnityInstanceId);

	public sealed class NimbusAdUnit {
		public readonly AdUnitType AdType;
		public BidResponse BidResponse;
		public AdEventTypes CurrentAdState { get; private set; } = AdEventTypes.NOT_LOADED;
		public ErrResponse ErrResponse;
		public readonly int InstanceID;
		
		private bool _adCompleted;
		private bool _adWasReturned;
		private readonly AdEvents _adEvents;

		internal bool AdWasRendered;
		internal string RawBidResponse;
		
		public NimbusAdUnit(AdUnitType adType, in AdEvents adEvents) {
			AdType = adType;
			InstanceID = GetHashCode();
			_adEvents = adEvents;
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
					if (AdType == AdUnitType.Interstitial && BidResponse.Type == "video") {
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
			await Task.Run(async () => {
				var response = "";
				try {
					response = await jsonBody;
				} catch (Exception e) { }
				if (response.Contains("message")) {
					Debug.unityLogger.Log("Nimbus",$"RESPONSE ERROR: {response}");
					ErrResponse = JsonConvert.DeserializeObject<ErrResponse>(response);
					_adEvents.FireOnAdErrorEvent(this);
					return;
				}
				Debug.unityLogger.Log("Nimbus",$"BID RESPONSE: {response}");
				_adWasReturned = true;
				RawBidResponse = response;
				BidResponse = JsonConvert.DeserializeObject<BidResponse>(response);
				_adEvents.FireOnAdLoadedEvent(this);
			});
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
}