using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.Meta {
	#if UNITY_IOS && NIMBUS_ENABLE_META
	internal class MetaIOS : IProvider {
		private readonly string _appID;
		private readonly bool _testMode;
		
		[DllImport("__Internal")]
		private static extern void _initializeMeta(string appKey, bool enableTestMode);

		public MetaIOS(string appID, bool enableTestMode) {
			_appID = appID;
			_testMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_initializeMeta(_appID, _testMode);
		}

	}
#endif
}