using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Moloco {
	#if UNITY_IOS && NIMBUS_ENABLE_MOLOCO
	internal class MolocoIOS : IInterceptor, IProvider {
		private readonly string _appKey;
		
		[DllImport("__Internal")]
		private static extern void _initializeMoloco(string appKey);

		[DllImport("__Internal")]
		private static extern string _fetchMolocoToken();
		
		public MolocoIOS(string appKey) {
			_appKey = appKey;
		}

		internal BidRequestDelta GetBidRequestDelta(string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			} 
			bidRequestDelta.SimpleUserExt = 
					new KeyValuePair<string, string> ("moloco_buyeruid", data);			
			return bidRequestDelta;
		}

		internal string GetMolocoToken()
		{
			var molocoToken = _fetchMolocoToken();
			if (molocoToken != null)
			{
				return molocoToken;
			}

			return "";
		}

		public void InitializeNativeSDK() {
			_initializeMoloco(_appKey);
		}

		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetMolocoToken());
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