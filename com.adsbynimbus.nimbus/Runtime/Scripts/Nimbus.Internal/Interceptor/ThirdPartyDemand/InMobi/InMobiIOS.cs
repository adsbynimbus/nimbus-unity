using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.InMobi {
	#if UNITY_IOS && NIMBUS_ENABLE_INMOBI
	internal class InMobiIOS : IInterceptor, IProvider {
		private readonly string _accountId;
		
		[DllImport("__Internal")]
		private static extern void _initializeInMobi(string accountId);

		[DllImport("__Internal")]
		private static extern string _fetchInMobiToken();
		
		public InMobiIOS(string accountId) {
			_accountId = accountId;
		}

		internal BidRequestDelta GetBidRequestDelta(string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			} 
			bidRequestDelta.SimpleUserExt = 
					new KeyValuePair<string, string> ("inmobi_buyeruid", data);			
			return bidRequestDelta;
		}

		internal string GetInMobiToken()
		{
			var inMobiToken = _fetchInMobiToken();
			if (inMobiToken != null)
			{
				return inMobiToken;
			}

			return "";
		}

		public void InitializeNativeSDK() {
			_initializeInMobi(_accountId);
		}

		public Task<BidRequestDelta> GetBidRequestDeltaAsync(AdUnitType type, bool isFullScreen, BidRequest bidRequest)
		{
			return Task<BidRequestDelta>.Run(() =>
			{
				try
				{
					return GetBidRequestDelta(GetInMobiToken());
				}
				catch (Exception e)
				{
					Debug.unityLogger.Log("InMobi ERROR", e.Message);
					return null;
				}
			});
		}
	}
#endif
}