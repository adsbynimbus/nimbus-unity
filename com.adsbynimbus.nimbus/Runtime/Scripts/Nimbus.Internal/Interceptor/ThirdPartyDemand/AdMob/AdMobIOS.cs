using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.AdMob {
	#if UNITY_IOS && NIMBUS_ENABLE_ADMOB
	internal class AdMobIOS : IInterceptor, IProvider {
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _type;
		private string _adUnitId;
		
		[DllImport("__Internal")]
		private static extern void _initializeAdMob();

		[DllImport("__Internal")]
		private static extern string _getAdMobRequestModifiers(int adUnitType, string adUnitId, int width, int height);

		private String GetProviderRtbDataFromNativeSDK(BidRequest bidRequest, AdUnitType type)
		{
			var adUnitId = GetAdUnitId(type);
			if (adUnitId.IsNullOrEmpty())
			{
				return "";
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
			
			return _getAdMobRequestModifiers((int) _type, adUnitId, width, height);
		}
		
		internal BidRequestDelta GetBidRequestDelta(string data)
		{
			return data.IsNullOrEmpty() ? new BidRequestDelta() : new BidRequestDelta()
			{
				SimpleUserExt = new KeyValuePair<string, string>("admob_gde_signals", data)
			};
		}

		private string GetAdUnitId(AdUnitType type)
		{
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					return adUnit.AdUnitId;
				}
			}
			return "";
		}

		public AdMobIOS(ThirdPartyAdUnit[] adUnitIds) {
			_adUnitIds = adUnitIds;
		}

		public void InitializeNativeSDK() {
			_initializeAdMob();
		}
		
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
	            {
	               return GetBidRequestDelta(GetProviderRtbDataFromNativeSDK(bidRequest, type));
	            }
	            catch (Exception e)
	            {
	               Debug.unityLogger.Log("AdMob AdUnitSignal ERROR", e.Message);
	               return null;
	            }
			});
		}

	}
#endif
}