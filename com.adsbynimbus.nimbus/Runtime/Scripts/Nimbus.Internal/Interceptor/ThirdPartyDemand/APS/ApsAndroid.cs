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
				var (w, h) = AdTypeToDim(slot.AdUnitType);
				if (slot.AdUnitType == AdUnitType.InterstitialVideo ||
				    slot.AdUnitType == AdUnitType.Rewarded) {
					_aps.CallStatic<bool>("addApsSlot", slot.SlotId, w, h, true);
					continue;
				}
				_aps.CallStatic<bool>("addApsSlot", slot.SlotId, w, h, false);
			}

			if (!_enableTestMode) return;
			
			using var adRegistration = new AndroidJavaClass("com.amazon.device.ads.AdRegistration");
			adRegistration.CallStatic("enableTesting", true);
		}

		private static Tuple<int, int> AdTypeToDim(AdUnitType type) {
			switch (type) {
				case AdUnitType.Banner320X50:
					return new Tuple<int, int>(320, 50);
				case AdUnitType.Banner300X250:
					return new Tuple<int, int>(300, 250);
				case AdUnitType.Banner728X90:
					return new Tuple<int, int>(728, 90);
				case AdUnitType.InterstitialDisplay:
					return new Tuple<int, int>(Screen.width, Screen.height);
				case AdUnitType.InterstitialVideo:
					return new Tuple<int, int>(Screen.width, Screen.height);
				case AdUnitType.Rewarded:
					return new Tuple<int, int>(Screen.width, Screen.height);
				default:
					return new Tuple<int, int>(0, 0);
			}
		}

		
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
			var found = false;
			// ReSharper disable once ForCanBeConvertedToForeach
			// ReSharper disable once LoopCanBeConvertedToQuery
			for (var i = 0; i < _slotData.Length; i++) {
				if (_slotData[i].AdUnitType != type) continue;
				found = true;
				break;
			}
			
			if (!found) {
				return null;
			}
			
			var (w, h) = AdTypeToDim(type);
			if (type == AdUnitType.InterstitialDisplay || type == AdUnitType.InterstitialVideo ||
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