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
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral {
	#if UNITY_IOS && NIMBUS_ENABLE_MINTEGRAL
	internal class MintegralIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly string _appKey;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		
		[DllImport("__Internal")]
		private static extern void _initializeMintegral(string appID, string appKey);

		[DllImport("__Internal")]
		private static extern string _getMintegralRequestModifiers();

		internal BidRequestDelta GetBidRequestDelta(BidRequest bidRequest, string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			
			var mintegralObject = JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject;
			if (mintegralObject != null)
			{
				bidRequestDelta.complexUserExt = 
					new KeyValuePair<string, JObject> ("mintegral_sdk", mintegralObject);			
			}
			return bidRequestDelta;
		}

		internal string GetProviderRtbDataFromNativeSDK(AdUnitType type)
		{
			var adUnitId = "";
			foreach (ThirdPartyAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					adUnitId = adUnit.AdUnitId;
					break;
				}
			}
			return adUnitId.IsNullOrEmpty() ? "" : _getMintegralRequestModifiers();
		}

		public MintegralIOS(string appID, string appKey, ThirdPartyAdUnit[] adUnitIds) {
			_appID = appID;
			_appKey = appKey;
			_adUnitIds = adUnitIds;
		}

		public void InitializeNativeSDK() {
			_initializeMintegral(_appID, _appKey);
		}
		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(bidRequest, GetProviderRtbDataFromNativeSDK(type));
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