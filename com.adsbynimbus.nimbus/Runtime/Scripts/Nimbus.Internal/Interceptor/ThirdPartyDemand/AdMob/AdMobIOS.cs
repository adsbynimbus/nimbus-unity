using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
		private readonly AdMobAdUnit[] _adUnitIds;
		private AdUnitType _type;
		private string _adUnitId;
		
		[DllImport("__Internal")]
		private static extern void _initializeAdMob();

		[DllImport("__Internal")]
		private static extern string _getAdMobRequestModifiers(int adUnitType, string adUnitId, int width, int height);

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
			var adMobSignals = _getAdMobRequestModifiers((int) _type, data, width, height);
			if (bidRequest.User.Ext == null) {
				bidRequest.User.Ext = new UserExt();
			}
			bidRequest.User.Ext.AdMobSignals = adMobSignals;
			Debug.unityLogger.Log("AdMob Signals", adMobSignals);
			
			return bidRequest;
		}

		public string GetProviderRtbDataFromNativeSDK(AdUnitType type, bool isFullScreen)
		{
			foreach (AdMobAdUnit adUnit in _adUnitIds)
			{
				if (adUnit.AdUnitType == type)
				{
					_adUnitId = adUnit.AdUnitId;
					_type = type;
					Debug.unityLogger.Log("AdMob AdUnitId", adUnit.AdUnitId);
					return adUnit.AdUnitId;
				}
			}
			return "";
		}

		public AdMobIOS(string appID, AdMobAdUnit[] adUnitIds, bool enableTestMode) {
			_appID = appID;
			_adUnitIds = adUnitIds;
			_testMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_initializeAdMob();
		}

	}
#endif
}