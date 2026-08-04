using System.Collections;
using System.Threading.Tasks;
using Internal;
using Internal.Extensions;
using UnityEngine;

namespace AdsByNimbus {
	internal delegate void DestroyAdDelegate(int adUnityInstanceId);

	public class Ad {
		public readonly AdType AdType;
		public string ErrResponse;
		public string NimbusReportingPosition;
		public NimbusAdUnitPosition AdPosition;
		public AdEvent CurrentAdState { get; private set; } = AdEvent.NOT_LOADED; 
		public readonly int InstanceID;
		private bool _adCompleted;
		private bool _adWasReturned;
		//this boolean exists because the bridge isn't invoked until .load() or .show() is called
		private bool _adPassedToNative;
		private readonly AdEvents _adEvents;
		internal bool AdWasRendered;
		public RequestModifiers? RequestModifiers;

		internal Task<string> Request = Task.FromResult("");
		
		public Ad(AdType adType, in AdEvents adEvents, string nimbusReportingPosition, 
			NimbusAdUnitPosition adPosition = NimbusAdUnitPosition.BOTTOM_CENTER, RequestModifiers? modifiers = null)
		{
			NimbusReportingPosition = nimbusReportingPosition;
			AdType = adType;
			InstanceID = GetHashCode();
			_adEvents = adEvents;
			AdPosition = adPosition;
			RequestModifiers = modifiers;
		}
		
		/// <summary>
		///   This method will preload and cache the ad to be shown later with the Show() method.
		///   It is not necessary to call this method before the Show() method.
		/// </summary>
		public void Load()
		{
			_adPassedToNative = true;
			NimbusManager.Instance.StartCoroutine(LoadAd(false));
		}
		
		/// <summary>
		///   This method will present the requested ad. The ad does not need the Load() method to be
		///   called before it will be shown.
		/// </summary>
		public void Show()
		{
			if (_adPassedToNative)
			{
				NimbusManager.Instance.StartCoroutine(ShowAd());
			}
			else
			{
				// Ad needs to be passed over the bridge before show() is called
				NimbusManager.Instance.StartCoroutine(LoadAd(true));
			}
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

		internal void FireMobileAdRenderedEvent() {
			_adEvents.FireOnAdRenderedEvent(this);
		}

		internal void FireMobileOnAdErrorEvent() {
			_adEvents.FireOnAdErrorEvent(this);
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

}