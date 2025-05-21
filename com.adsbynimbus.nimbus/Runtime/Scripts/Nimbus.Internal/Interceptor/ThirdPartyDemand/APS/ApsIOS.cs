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

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen) {
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
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
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