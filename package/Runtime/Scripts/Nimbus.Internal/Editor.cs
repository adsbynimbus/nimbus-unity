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

		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log("In Editor mode, ShowAd was called, however ads cannot be shown in the editor");
		}

		internal override string GetSessionID() {
			Debug.unityLogger.Log("Mock Session ID the SDK is not initialized");
			return Guid.NewGuid().ToString();
		}

		internal override Device GetDevice() {
			return new Device {
				Ua = "UnityPlayer/2020.3.34f1 personal (UnityWebRequest/1.0, libcurl/7.52.0-DEV)",
				DeviceType = DeviceType.PersonalComputer,
				Os = "Unity Editor",
				Osv = Application.version,
				H = Screen.height,
				W = Screen.width,
				ConnectionType = ConnectionType.Unknown,
				Ifa = "00000000-0000-0000-0000-000000000000"
			};
		}
		
		internal override List<IInterceptor> Interceptors() {
			return null;
		}
		
		internal override void SetCoppaFlag(bool flag) {
			Debug.unityLogger.Log($"Mock Coppa flag set {flag}");
		}
	}
}