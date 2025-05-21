using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand {
	internal class ApsAndroid : IInterceptor, IProvider {
		private const string AndroidApsPackage = "com.adsbynimbus.request.ApsDemandProvider";

		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;
		private readonly AndroidJavaObject _currentActivity;
		private AndroidJavaClass _aps;
		private int _timeoutInMilliseconds = 1000;
		
		public ApsAndroid(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public ApsAndroid(AndroidJavaObject currentActivity, string appID, ApsSlotData[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_currentActivity = currentActivity;
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
			if (timeoutInMilliseconds > 0)
			{
				_timeoutInMilliseconds = timeoutInMilliseconds;
			}
		}

		public void InitializeNativeSDK() {
			_aps = new AndroidJavaClass(AndroidApsPackage);
			_aps.CallStatic("initialize", _currentActivity, _appID);
			foreach (var slot in _slotData) {
				var (w, h) = APSHelper.AdTypeToDim(slot.APSAdUnitType);
				if (slot.APSAdUnitType == APSAdUnitType.InterstitialVideo || 
				    slot.APSAdUnitType == APSAdUnitType.RewardedVideo) {
					_aps.CallStatic<bool>("addApsSlot", slot.SlotId, w, h, true);
					continue;
				}
				_aps.CallStatic<bool>("addApsSlot", slot.SlotId, w, h, false);
			}

			if (!_enableTestMode) return;
			
			using var adRegistration = new AndroidJavaClass("com.amazon.device.ads.AdRegistration");
			adRegistration.CallStatic("enableTesting", true);
		}
		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, BidRequest bidRequest, bool isFullScreen) {
			var found = false;
			var interstitialVideo = false;
			var width = 0;
			var height = 0;
			if (!bidRequest.Imp.IsNullOrEmpty())
			{
				if (bidRequest.Imp[0].Banner != null)
				{
					width = bidRequest.Imp[0].Banner.W ?? 0;
					height = bidRequest.Imp[0].Banner.H ?? 0;
				}
			}
			foreach (ApsSlotData slot in _slotData){
				if (type == AdUnitType.Banner)
				{
					if (width == 320 && height == 50 && slot.APSAdUnitType == APSAdUnitType.Display320X50)
					{
						found = true;
						break;
					}
					if (width == 300 && height == 250 && slot.APSAdUnitType == APSAdUnitType.Display300X250)
					{
						found = true;
						break;
					}
					if (width == 728 && height == 90 && slot.APSAdUnitType == APSAdUnitType.Display728X90)
					{
						found = true;
						break;
					}
				}
				if (type == AdUnitType.Interstitial)
				{
					if (slot.APSAdUnitType == APSAdUnitType.InterstitialDisplay ||
					    slot.APSAdUnitType == APSAdUnitType.InterstitialVideo)
					{
						found = true;
						interstitialVideo = (slot.APSAdUnitType == APSAdUnitType.InterstitialVideo);
						break;
					}
				}
				if (type == AdUnitType.Rewarded)
				{
					if (slot.APSAdUnitType == APSAdUnitType.RewardedVideo)
					{
						found = true;
						break;
					}
				}
			}
			
			if (!found) {
				Debug.unityLogger.LogError("Nimbus", 
					"APS NOT FOUND");
				return null;
			}

			var w = width;
			var h = height;
			if (interstitialVideo || type == AdUnitType.Rewarded) {
				w = 0;
				h = 0;
				isFullScreen = true;
			}
			SetApsTimeout(_timeoutInMilliseconds);
			var response = _aps.CallStatic<string>("fetchApsParams", w, h, isFullScreen);
			return response;
		}
		
		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			
			// ReSharper disable InvertIf
			if (!bidRequest.Imp.IsNullOrEmpty()) {
				if (bidRequest.Imp[0].Ext != null) {
					bidRequestDelta.impressionExtension = new ImpExt {
						Aps =  JsonConvert.DeserializeObject<JObject[]>(data)
					};;
				}
			}
			return bidRequestDelta;
		}

		private void SetApsTimeout(int timeoutInMilliseconds) {
			if (timeoutInMilliseconds != 1000)
			{
				_aps.CallStatic("setApsRequestTimeout", timeoutInMilliseconds);
			}
		}
		
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, bidRequest, isFullScreen));
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("APS ERROR", e.Message);
					return null;
				}
			});
		}
	}
}