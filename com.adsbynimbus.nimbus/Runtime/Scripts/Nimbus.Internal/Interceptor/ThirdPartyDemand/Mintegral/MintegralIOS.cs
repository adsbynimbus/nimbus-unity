using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Mintegral {
	#if UNITY_IOS && NIMBUS_ENABLE_MINTEGRAL
	internal class MintegralIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly string _appKey;
		private readonly bool _testMode;
		private readonly ThirdPartyAdUnit[] _adUnitIds;
		private AdUnitType _type;
		private string _adUnitId = "";
		private string _adUnitPlacementId = "";
		
		[DllImport("__Internal")]
		private static extern void _initializeMintegral(string appID, string appKey);

		[DllImport("__Internal")]
		private static extern string _getMintegralRequestModifiers();

		private BidRequestDelta ModifyRequest(BidRequest bidRequest, string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}

			var mintegralObjectStr = _getMintegralRequestModifiers();
			var mintegralObject = JsonConvert.DeserializeObject(mintegralObjectStr, typeof(JObject)) as JObject;
			if (mintegralObject != null)
			{
				bidRequestDelta.complexUserExt = 
					new KeyValuePair<string, JObject> ("mintegral_sdk", mintegralObject);			
			}
			return bidRequestDelta;
		}

		private string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
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

		public MintegralIOS(string appID, string appKey, ThirdPartyAdUnit[] adUnitIds, bool enableTestMode) {
			_appID = appID;
			_appKey = appKey;
			_adUnitIds = adUnitIds;
			_testMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_initializeMintegral(_appID, _appKey);
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