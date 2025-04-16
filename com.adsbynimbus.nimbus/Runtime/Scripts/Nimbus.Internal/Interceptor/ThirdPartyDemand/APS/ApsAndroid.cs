using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
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
		
		public ApsAndroid(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public ApsAndroid(AndroidJavaObject currentActivity, string appID, ApsSlotData[] slotData, bool enableTestMode) {
			_currentActivity = currentActivity;
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_aps = new AndroidJavaClass(AndroidApsPackage);
			_aps.CallStatic("initialize", _currentActivity, _appID);
			foreach (var slot in _slotData) {
				var (w, h) = AdTypeToDim(slot.APSAdUnitType);
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

		private static Tuple<int, int> AdTypeToDim(APSAdUnitType type) {
			switch (type) {
				case APSAdUnitType.Display320X50:
					return new Tuple<int, int>(320, 50);
				case APSAdUnitType.Display300X250:
					return new Tuple<int, int>(300, 250);
				case APSAdUnitType.Display728X90:
					return new Tuple<int, int>(728, 90);
				case APSAdUnitType.InterstitialDisplay:
					return new Tuple<int, int>(Screen.width, Screen.height);
				case APSAdUnitType.InterstitialVideo:
					return new Tuple<int, int>(Screen.width, Screen.height);
				case APSAdUnitType.RewardedVideo:
					return new Tuple<int, int>(Screen.width, Screen.height);
				default:
					return new Tuple<int, int>(0, 0);
			}
		}

		
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen, int width=0, int height=0) {
			var found = false;
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
			if (type == AdUnitType.Interstitial ||
			    type == AdUnitType.Rewarded) {
				w = 0;
				h = 0;
				isFullScreen = true;
			}
		
			var response = _aps.CallStatic<string>("fetchApsParams", w, h, isFullScreen);
			return response;
		}
		
		public BidRequest ModifyRequest(BidRequest bidRequest, string data) {
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			
			// ReSharper disable InvertIf
			if (!bidRequest.Imp.IsNullOrEmpty()) {
				if (bidRequest.Imp[0].Ext != null) {
					var apsObject = JsonConvert.DeserializeObject<ApsResponse[]>(data);
					// ThirdPartyProviderImpExt has already been initialized by another IProvider
					if (bidRequest.Imp[0].Ext is ThirdPartyProviderImpExt apsData) {
						apsData.Aps = apsObject;
						bidRequest.Imp[0].Ext = apsData;
						return bidRequest;
					}
					
					var ext = new ThirdPartyProviderImpExt {
						Position = bidRequest.Imp[0].Ext.Position,
						Skadn =  bidRequest.Imp[0].Ext.Skadn,
						Aps =  apsObject,
					};
					bidRequest.Imp[0].Ext = ext;
				}
			}
			return bidRequest;
		}

		internal void SetApsTimeout(int timeInSeconds) {
			var timeout = (long)TimeSpan.FromSeconds(timeInSeconds).TotalMilliseconds;
			_aps.CallStatic("setApsRequestTimeout", timeout);
		}
	}
	
}