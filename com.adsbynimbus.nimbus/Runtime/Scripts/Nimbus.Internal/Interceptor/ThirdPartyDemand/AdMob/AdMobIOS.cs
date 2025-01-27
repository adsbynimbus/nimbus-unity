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
		
		[DllImport("__Internal")]
		private static extern void _initializeAdMob();

		[DllImport("__Internal")]
		private static extern string _getAdMobRequestModifiers(int adUnitType, string adUnitId);

		public BidRequest ModifyRequest(BidRequest bidRequest, string data)
		{
			if (data.IsNullOrEmpty()) {
				return bidRequest;
			}
			//maybe pass w/h to initialize ad format? NimbusAdFormat
			var adMobSignals = _getAdMobRequestModifiers((int) _type, data);
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