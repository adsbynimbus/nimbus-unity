using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using OpenRTB.Request;
using UnityEngine;

[assembly: InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Interceptor.ThirdPartyDemand.APS {
	#if UNITY_IOS && NIMBUS_ENABLE_APS
	internal class ApsIOS : IInterceptor, IProvider {
		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;

		private float _timeoutInSeconds = 3.0f;

		public ApsIOS(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}

		public ApsIOS(string appID, ApsSlotData[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
			_timeoutInSeconds = Math.Clamp(timeoutInMilliseconds, 300, 3000)/1000.0f;
		}
	}
#endif
}