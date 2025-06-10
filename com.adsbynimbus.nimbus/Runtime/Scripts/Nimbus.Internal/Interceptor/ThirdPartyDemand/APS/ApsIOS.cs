using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.APS {
	#if UNITY_IOS && NIMBUS_ENABLE_APS
	internal class ApsIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;

		private float _timeoutInSeconds = 1.0f;

		[DllImport("__Internal")]
		private static extern void _initializeAPSRequestHelper(string appKey, double timeoutInSeconds, bool enableTestMode);

		[DllImport("__Internal")]
		private static extern void _addAPSSlot(string slotUuid, int width, int height, bool isVideo);

		[DllImport("__Internal")]
		private static extern string _fetchAPSParams(int width, int height, bool includeVideo);

		public ApsIOS(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}

		public ApsIOS(string appID, ApsSlotData[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
			_timeoutInSeconds = Math.Clamp(timeoutInMilliseconds, 300, 2000)/1000.0f;
		}

		public void InitializeNativeSDK() {
			_initializeAPSRequestHelper(_appID, _timeoutInSeconds, _enableTestMode);

			foreach (var slot in _slotData) {
				var (w, h) = APSHelper.AdTypeToDim(slot.APSAdUnitType);
				if (slot.APSAdUnitType == APSAdUnitType.InterstitialVideo ||
				    slot.APSAdUnitType == APSAdUnitType.RewardedVideo) {
					_addAPSSlot(slot.SlotId, w, h, true);
					continue;
				}

				_addAPSSlot(slot.SlotId, w, h, false);
			}
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
				return null;
			}

			var w = width;
			var h = height;
			if (interstitialVideo || type == AdUnitType.Rewarded) {
				w = 0;
				h = 0;
				isFullScreen = true;
			}
			return _fetchAPSParams(w, h, isFullScreen);
		}

		internal BidRequestDelta GetBidRequestDelta(BidRequest bidRequest, string data) {
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			
			// ReSharper disable InvertIf
			if (!bidRequest.Imp.IsNullOrEmpty()) {
				if (bidRequest.Imp[0].Ext != null) {
					bidRequestDelta.impressionExtension = new ImpExt {
						Aps =  JsonConvert.DeserializeObject<JArray>(data)
					};;
				}
			}
			return bidRequestDelta;
		}
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(bidRequest, GetProviderRtbDataFromNativeSDK(type, bidRequest, isFullScreen));
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("APS ERROR", e.Message);
					return null;
				}
			});
		}
	}
#endif
}