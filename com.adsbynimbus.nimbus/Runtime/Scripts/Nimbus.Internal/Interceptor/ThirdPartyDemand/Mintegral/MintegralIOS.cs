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
		

		internal BidRequestDelta GetBidRequestDelta(BidRequest bidRequest, string data)
		{
			var bidRequestDelta = new BidRequestDelta();
			if (data.IsNullOrEmpty()) {
				return bidRequestDelta;
			}
			
			var mintegralObject = JsonConvert.DeserializeObject(data, typeof(JObject)) as JObject;
			if (mintegralObject != null)
			{
				bidRequestDelta.ComplexUserExt = 
					new KeyValuePair<string, JObject> ("mintegral_sdk", mintegralObject);			
			}
			return bidRequestDelta;
		}

		public MintegralIOS(string appID, string appKey) {
			_appID = appID;
			_appKey = appKey;
		}
		
		public ThirdPartyDemandObj GetConfigObject()
		{
			return new ThirdPartyDemandObj(ThirdPartyDemandEnum.Mintegral, _appID, _appKey);
		}
	}
#endif
}