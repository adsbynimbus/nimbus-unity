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
		
		public ThirdPartyDemandObj GetConfigObject()
		{
			return new ThirdPartyDemandObj(ThirdPartyDemandEnum.InMobi, _accountId);
		}
	}
#endif
}