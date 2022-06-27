#if UNITY_EDITOR
using Nimbus.Runtime.Scripts;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nimbus.Editor {
	public class BuildChecks: IPreprocessBuildWithReport {
		public int callbackOrder { get; }
		public void OnPreprocessBuild(BuildReport report) {
			if (UnityEditor.EditorUserBuildSettings.development) {
				return;
			}
			
			var nimbusManager = Object.FindObjectOfType<NimbusManager>();
			if (nimbusManager == null) {
				Debug.unityLogger.LogError("Nimbus", "The Scene is missing a NimbusManager object");
				return;
			}
			var config = nimbusManager.GetNimbusConfiguration();
			if (config.enableUnityLogs) {
				Debug.unityLogger.LogWarning("Nimbus", "creating a release build with logs enabled");
			}
			
			if (config.apiKey.Contains("DEV")) {
				throw new BuildFailedException("Release builds cannot be released with a development key, please reach out to your AM or SE");
			} 
			
			if (config.enableSDKInTestMode) {
				throw new BuildFailedException("Release builds cannot be released with test mode enabled, please reach out to your AM or SE");
			} 
		}
	}
}
#endif
