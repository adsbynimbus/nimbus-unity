using System;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.ThirdPartyDemandProviders {
	internal class ApsIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;

		private const double TimeoutInSeconds = 0.6f;

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

		public ApsIOS(string appID, ApsSlotData[] slotData, bool enableTestMode) {
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_initializeAPSRequestHelper(_appID, TimeoutInSeconds, _enableTestMode);

			foreach (var slot in _slotData) {
				var (w, h) = AdTypeToDim(slot.AdUnitType);
				if (slot.AdUnitType == AdUnitType.Rewarded) {
					_addAPSSlot(slot.SlotId, w, h, true);
					continue;
				}

				_addAPSSlot(slot.SlotId, w, h, false);
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
			// ReSharper disable once InvertIf
			if (type == AdUnitType.Rewarded) {
				w = 0;
				h = 0;
				isFullScreen = true;
			}

			return _fetchAPSParams(w, h, isFullScreen);
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
						Skadn = bidRequest.Imp[0].Ext.Skadn,
						Aps = apsObject,
					};
					bidRequest.Imp[0].Ext = ext;
				}
			}

			return bidRequest;
		}
	}
}