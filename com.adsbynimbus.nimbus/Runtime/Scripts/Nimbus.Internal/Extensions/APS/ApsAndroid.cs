using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nimbus.Internal.Utility;
using UnityEngine;

[assembly:InternalsVisibleTo("nimbus.test")]
namespace Nimbus.Internal.Extensions.APS {
	internal class ApsAndroid {
		private const string AndroidApsPackage = "com.adsbynimbus.request.ApsDemandProvider";

		private readonly string _appID;
		private readonly bool _enableTestMode;
		private readonly ApsSlotData[] _slotData;
		private readonly AndroidJavaObject _currentActivity;
		private AndroidJavaClass _aps;
		
		public ApsAndroid(string appID, ApsSlotData[] slotData) {
			_appID = appID;
			_slotData = slotData;
		}
		
		public ApsAndroid(AndroidJavaObject currentActivity, string appID, ApsSlotData[] slotData, bool enableTestMode, int timeoutInMilliseconds) {
			_currentActivity = currentActivity;
			_appID = appID;
			_slotData = slotData;
			_enableTestMode = enableTestMode;
		}

		public void InitializeNativeSDK() {
			_aps = new AndroidJavaClass(AndroidApsPackage);
			_aps.CallStatic("initialize", _currentActivity, _appID);
		}
	}
}