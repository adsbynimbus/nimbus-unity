using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob {
	#if UNITY_IOS && NIMBUS_ENABLE_ADMOB
	internal class AdMobIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _testMode;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _type;
		private string _adUnitId;
		
		[DllImport("__Internal")]
		private static extern void _initializeAdMob();

		[DllImport("__Internal")]
		private static extern string _getAdMobRequestModifiers(int adUnitType, string adUnitId, int width, int height);

		private BidRequestDelta ModifyRequest(BidRequest bidRequest, string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
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

			var adMobSignals = _getAdMobRequestModifiers((int) _type, data, width, height);
			bidRequestDelta.simpleUserExt = new KeyValuePair<string, string>("admob_gde_signals", adMobSignals);
			
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					_adUnitId = adUnit.AdUnitId;
					_type = type;
					return adUnit.AdUnitId;
				}
			}
			return "";
		}

		public AdMobIOS(string appID, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_appID = appID;
			_adUnitIds = adUnitIds;
			_testMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_initializeAdMob();
		}
		
		public Task<BidRequestDelta> ModifyRequestAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				return ModifyRequest(bidRequest, GetProviderRtbDataFromNativeSDK(type, isFullScreen));
			});
		}

	}
#endif
}