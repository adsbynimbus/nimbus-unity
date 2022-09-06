using System;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.ThirdPartyDemandProviders {
	internal class Aps : IInterceptor, IProvider {
		private const string AndroidApsPackage = "com.adsbynimbus.request.ApsDemandProvider";

		private readonly string _appID;
		private readonly ApsSlotData[] _slotData;
		private readonly AndroidJavaObject _currentActivity;
		private AndroidJavaObject _aps;
		
		public Aps(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public Aps(AndroidJavaObject currentActivity, string appID, ApsSlotData[] slotData) {
			_currentActivity = currentActivity;
			_appID = appID;
			_slotData = slotData;
		}

		public void InitializeNativeSDK() {
			_aps = new AndroidJavaObject(AndroidApsPackage);
			_aps.CallStatic("initialize", _currentActivity, _appID);
			foreach (var slot in _slotData) {
				if (slot.AdUnitType == AdUnitType.Rewarded) {
					_aps.CallStatic("addApsSlot", slot.SlotId, Screen.width, Screen.height, true);
					continue;
				}
				var (w, h) = AdTypeToDim(slot.AdUnitType);
				_aps.CallStatic("addApsSlot", slot.SlotId, w, h, false);
			}
		}

		private static Tuple<int, int> AdTypeToDim(AdUnitType type) {
			switch (type) {
				case AdUnitType.Undefined:
					return new Tuple<int, int>(0, 0);
				case AdUnitType.Banner:
					return new Tuple<int, int>(320, 50);
				case AdUnitType.Interstitial:
					return new Tuple<int, int>(320, 480);
				case AdUnitType.Rewarded:
					return new Tuple<int, int>(Screen.width, Screen.height);
				default:
					return new Tuple<int, int>(0, 0);
			}
		}

		
		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var slot in _slotData) {
				if (slot.AdUnitType == type) {
					break;
				}
				return null;
			}
			
			var (w, h) = AdTypeToDim(type);
			if (type == AdUnitType.Rewarded) {
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
				bidRequest.Imp[0].Ext ??= new ThirdPartyProviderImpExt();
				// pass in the APS data
				if (bidRequest.Imp[0].Ext is ThirdPartyProviderImpExt apsData) {
					apsData.Aps = data;
					bidRequest.Imp[0].Ext = apsData;
				}
			}
			return bidRequest;
		}

		internal void SetApsTimeout(int timeInSeconds) {
			var timeout = (int)TimeSpan.FromSeconds(timeInSeconds).TotalMilliseconds;
			_aps.CallStatic("setApsRequestTimeout", timeout);
		}
	}
	
}