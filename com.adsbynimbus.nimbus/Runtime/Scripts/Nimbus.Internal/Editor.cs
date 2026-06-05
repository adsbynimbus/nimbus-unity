using System;
using System.Collections.Generic;
using Nimbus.ScriptableObjects;
using UnityEngine;

namespace Nimbus.Internal {
	public class Editor : NimbusAPI {
		internal override void InitializeSDK(NimbusSDKConfiguration configuration) {
			Debug.unityLogger.Log("Mock SDK initialized for editor");
		}

		internal override void GetAd(NimbusAdUnit nimbusAdUnit, bool showAd) {
			Debug.unityLogger.Log("In Editor mode, GetAd was called, however ads cannot be accessed in the editor");
		}
		
		internal override void ShowAd(NimbusAdUnit nimbusAdUnit) {
			Debug.unityLogger.Log("In Editor mode, ShowAd was called, however ads cannot be shown in the editor");
		}
	}
}