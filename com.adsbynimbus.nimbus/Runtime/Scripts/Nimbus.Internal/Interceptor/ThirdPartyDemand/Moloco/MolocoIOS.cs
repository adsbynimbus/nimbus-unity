using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Molocol {
	#if UNITY_IOS && NIMBUS_ENABLE_MOLOCO
	internal class MolocoIOS : IInterceptor, IProvider {
		private readonly string _appKey;
		private readonly bool _testMode;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _type;
		private string _adUnitId = "";
		private string _adUnitPlacementId = "";
		
		[DllImport("__Internal")]
		private static extern void _initializeMoloco(string appKey);

		[DllImport("__Internal")]
		private static extern string _fetchMolocoToken();
		
		public MolocoIOS(string appKey, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_appKey = appKey;
			_adUnitIds = adUnitIds;
			_testMode = enableTestMode;
		}

		internal BidRequestDelta ModifyRequest(BidRequest bidRequest, string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}

			var molocoToken = _fetchMolocoToken();
			if (molocoToken != null)
			{
				bidRequestDelta.simpleUserExt = 
					new KeyValuePair<string, string> ("moloco_buyeruid", molocoToken);			
			}
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					_adUnitId = adUnit.AdUnitId;
					_adUnitPlacementId = adUnit.AdUnitPlacementId;
					_type = type;
					return adUnit.AdUnitId;
				}
			}
			return "";
		}

		public void InitializeNativeSDK() {
			_initializeMoloco(_appKey);
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
					Debug.unityLogger.Log("Mintegral ERROR", e.Message);
					return null;
				}
			});
		}
	}
#endif
}