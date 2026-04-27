using System;
using System.Collections.Generic;
using Nimbus.Internal.Interceptor;
using Nimbus.ScriptableObjects;
using OpenRTB.Enumerations;
using OpenRTB.Request;
using UnityEngine;
using DeviceType = OpenRTB.Enumerations.DeviceType;

namespace Nimbus.Internal {
	public class Editor : NimbusAPI {
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Mock SDK initialized for editor");
		}

		internal override void getAd(NimbusAdUnit nimbusAdUnit, bool showAd) {
			Debug.unityLogger.Log("In Editor mode, ShowAd was called, however ads cannot be shown in the editor");
		}
		
		internal override List<IInterceptor> Interceptors() {
			return null;
		}
	}
}