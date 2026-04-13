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
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	#if UNITY_IOS && NIMBUS_ENABLE_META
	internal class MetaIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _testMode;

		public MetaIOS(string appID, bool enableTestMode) {
			_appID = appID;
			_testMode = enableTestMode;
		}
		
		public JObject GetConfigObject()
		{
			var jObject = new JObject();
			jObject["demand"] = "Meta";
			jObject["appId"] = _appID;
			jObject["testMode"] = _testMode;
			return jObject;
		}

	}
#endif
}