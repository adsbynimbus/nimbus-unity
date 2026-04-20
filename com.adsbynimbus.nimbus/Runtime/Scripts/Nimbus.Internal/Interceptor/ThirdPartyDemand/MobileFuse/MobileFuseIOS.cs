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
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.MobileFuse {
	#if UNITY_IOS && NIMBUS_ENABLE_MOBILEFUSE
	internal class MobileFuseIOS : IInterceptor, IProvider {
		
		public ThirdPartyDemandObj GetConfigObject()
		{
			return new ThirdPartyDemandObj(ThirdPartyDemandEnum.MobileFuse);
		}
	}
#endif
}