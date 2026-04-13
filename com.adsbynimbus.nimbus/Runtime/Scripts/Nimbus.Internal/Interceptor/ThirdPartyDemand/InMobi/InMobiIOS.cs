using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using Nimbus.Runtime.Scripts;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.InMobi {
	#if UNITY_IOS && NIMBUS_ENABLE_INMOBI
	internal class InMobiIOS : IInterceptor, IProvider {
		private readonly string _accountId;
		
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

		public void InitializeNativeSDK() {
		}
		
		public JObject GetConfigObject()
		{
			var jObject = new JObject();
			jObject["demand"] = "InMobi";
			jObject["accountId"] = _accountId;
			return jObject;
		}
	}
#endif
}