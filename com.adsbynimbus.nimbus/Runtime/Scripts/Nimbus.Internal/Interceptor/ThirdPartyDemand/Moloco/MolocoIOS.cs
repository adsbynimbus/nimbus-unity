using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using Newtonsoft.Json.Linq;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Moloco {
	#if UNITY_IOS && NIMBUS_ENABLE_MOLOCO
	internal class MolocoIOS : IInterceptor, IProvider {
		private readonly string _appKey;
		
		public MolocoIOS(string appKey) {
			_appKey = appKey;
		}
		
		public ThirdPartyDemandObj GetConfigObject()
		{
			return new ThirdPartyDemandObj(ThirdPartyDemandEnum.Moloco, _appKey);
		}
	}
#endif
}