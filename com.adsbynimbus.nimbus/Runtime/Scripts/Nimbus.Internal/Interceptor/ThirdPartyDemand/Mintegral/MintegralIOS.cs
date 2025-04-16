using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

		public BidRequest ModifyRequest(BidRequest bidRequest, string data)
		{
			if (data.IsNullOrEmpty()) {
				return bidRequest;
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

			var mintegralObjectStr = _getMintegralRequestModifiers();
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			var mintegralObject = JsonConvert.DeserializeObject(mintegralObjectStr, typeof(MintegralObj)) as MintegralObj;
			if (mintegralObject != null)
			{
				bidRequest.User.Ext.MintegralSdkObj = mintegralObject;
			}
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen, int width=0, int height=0)
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

	}
#endif
}