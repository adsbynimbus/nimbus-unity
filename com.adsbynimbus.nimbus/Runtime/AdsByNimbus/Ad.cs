using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdsByNimbus.Extensions;
using AdsByNimbus.Internal;
using AdsByNimbus.Internal.Extensions;
using AdsByNimbus.Internal.Utility;
using AdsByNimbus.RTB;
using AdsByNimbus.RTB.Request;
using UnityEngine;

namespace AdsByNimbus {
	internal delegate void DestroyAdDelegate(int adUnityInstanceId);

	public class Ad {
		public readonly AdType AdType;
		public string position;
		public float BidFloor;
		public AdOrientation Orientation;
		public Format[] AddFormats;
		public AdEvent CurrentAdState { get; private set; } = AdEvent.NOT_LOADED; 
		public readonly int InstanceID;
		private bool _adCompleted;
		private bool _adWasReturned;
		//this boolean exists because the bridge isn't invoked until .load() or .show() is called
		private bool _adPassedToNative;
		private readonly AdEvents _adEvents;
		private Action<AdEvent> _onAdEvent;
		private Action<NimbusError> _onAdError;
		#if NIMBUS_ENABLE_APS
			internal apsAd[] ApsAds;
		#endif
		#if NIMBUS_ENABLE_ADMOB
			internal string AdMobAdUnitId;
		#endif
		
		public List<RequestComponent> components { get;} = new List<RequestComponent>();
		
		public List<DemandComponent> demand { get;} = new List<DemandComponent>();

		internal Ad(AdType adType, in AdEvents adEvents, string strPosition, Format[] addFormats = null,
			float bidFloor = 0f, AdOrientation orientation = AdOrientation.deviceOrientation, 
			List<RequestComponent> components = null, List<DemandComponent> demand = null)
		{
			AdType = adType;
			_adEvents = adEvents;
			position = strPosition;
			AddFormats = addFormats ?? new Format[]{};
			BidFloor = bidFloor;
			Orientation = orientation;
			InstanceID = GetHashCode();
			this.components = components;
			this.demand = demand;
			SetupDemand();
		}

		public Ad onEvent(Action<AdEvent> onEvent)
		{
			_onAdEvent = onEvent;
			return this;
		}

		public Ad onError(Action<NimbusError> onError)
		{
			_onAdError = onError;
			return this;
		}
		
		/// <summary>
		///   This method will preload and cache the ad to be shown later with the Show() method.
		///   It is not necessary to call this method before the Show() method.
		/// </summary>
		public Ad load()
		{
			_adPassedToNative = true;
			NimbusManager.Instance.StartCoroutine(LoadAd(showAd: false));
			return this;
		}
		
		/// <summary>
		///   This method will present the requested ad. The ad does not need the Load() method to be
		///   called before it will be shown.
		/// </summary>
		public Ad show()
		{
			if (_adPassedToNative)
			{
				NimbusManager.Instance.StartCoroutine(ShowAd());
			}
			else
			{
				// Ad needs to be passed over the bridge before show() is called
				NimbusManager.Instance.StartCoroutine(LoadAd(showAd: true));
			}

			return this;
		}
		
		/// <summary>
		///     Destroys the ad at the mobile bridge level
		/// </summary>
		public void Destroy() {
#if UNITY_ANDROID
			var managerClass = new AndroidJavaObject("com.adsbynimbus.unity.NimbusManager");
			var instance = managerClass.GetStatic<AndroidJavaObject> ("INSTANCE");
			instance.CallStatic("destroyAd", InstanceID);
			_androidController = null;
			_androidHelper = null;
# elif UNITY_IOS
			OnDestroyIOSAd?.Invoke(InstanceID);
#endif
		}
		# region IOS specific

#pragma warning disable 67
		internal event DestroyAdDelegate OnDestroyIOSAd;
#pragma warning restore 67

		#endregion


		private void SetupDemand()
		{
			if (demand != null)
			{
				foreach (var component in demand)
				{
					switch (component)
					{
#if NIMBUS_ENABLE_ADMOB
						case adMob adm:
							AdMobAdUnitId = adm.adUnitId;
							break;
#endif
#if NIMBUS_ENABLE_APS
						case aps aps:
							ApsAds = aps.apsAds;
							break;
#endif
						default:
							break;
					}
				}
			}

		}
		
		internal RequestModifiers GetRequestModifiers()
		{
			var rm = new RequestModifiers();
			if (components != null)
			{
				foreach (var component in components)
				{
					switch (component)
					{
						case app a:
							rm.app = a;
							break;
						case banner b:
							rm.banner = b;
							break;
						case content c:
							rm.content = c;
							break;
						case environment e:
							rm.environment = e;
							break;
						case location l:
							rm.location = l;
							break;
						case user u:
							rm.user = u;
							break;
						case video v:
							rm.video = v;
							break;
						case viewability vb:
							rm.viewability = vb;
							break;
						default:
							break;
					}
				}
			}
			return rm;
		}
		

		internal void FireMobileAdRenderedEvent() {
			_adEvents.FireOnAdRenderedEvent(this);
		}

		internal void FireMobileOnAdErrorEvent(NimbusError nimbusError) {
			_adEvents.FireOnAdErrorEvent(this, nimbusError);
			_onAdError?.Invoke(nimbusError);
		}
		
		internal void FireMobileAdEvents(AdEvent e) {
			CurrentAdState = e;
			switch (e) {
				case AdEvent.LOADED:
					_adEvents.FireOnAdLoadedEvent(this);
					break;
				case AdEvent.IMPRESSION:
					_adEvents.FireOnAdImpressionEvent(this);
					break;
				case AdEvent.CLICKED:
					_adEvents.FireOnAdClickedEvent(this);
					break;
				case AdEvent.PAUSED:
					_adEvents.FireOnVideoAdPausedEvent(this);
					break;
				case AdEvent.RESUMED:
					_adEvents.FireOnVideoAdResumeEvent(this);
					break;
				case AdEvent.REWARDEARNED:
					_adEvents.FireOnAdRewardEarnedEvent(this);
					break;
				case AdEvent.COMPLETED:
					_adCompleted = true;
					// ensure that video ads auto close to avoid a black screen when the ad completes
					if (AdType == AdType.Fullscreen) {
						Destroy();
					}
					break;
				case AdEvent.DESTROYED:
					// ReSharper disable once ConvertIfStatementToSwitchStatement
					if (AdType == AdType.Rewarded) {
						_adEvents.FireOnAdCompletedEvent(this, !_adCompleted);
					} else if (AdType == AdType.Fullscreen) {
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
			_onAdEvent?.Invoke(e);
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
		
		private IEnumerator LoadAd(bool showAd)
		{
			NimbusManager.Instance.NimbusPlatformAPI.GetAd(this, showAd);
			yield break;
		}
		
		private IEnumerator ShowAd()
		{
			NimbusManager.Instance.NimbusPlatformAPI.ShowAd(this);
			yield break;
		}
	}

	public enum AdType : byte {
		Inline = 0,
		Fullscreen = 1,
		Rewarded = 2
	}

	public enum AdOrientation : byte
	{
		portrait = 0,
		landscape = 1,
		deviceOrientation = 2
	}


}