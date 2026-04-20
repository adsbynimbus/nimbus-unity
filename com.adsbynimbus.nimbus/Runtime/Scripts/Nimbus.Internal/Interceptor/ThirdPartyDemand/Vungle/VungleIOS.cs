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
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Vungle {
	#if UNITY_IOS && NIMBUS_ENABLE_VUNGLE
	internal class VungleIOS : IInterceptor, IProvider {
		private readonly string _appID;

		public VungleIOS(string appID) {
			_appID = appID;
		}
		
		public ThirdPartyDemandObj GetConfigObject()
		{
			return new ThirdPartyDemandObj(ThirdPartyDemandEnum.Vungle, _appID);
		}
	}
#endif
}