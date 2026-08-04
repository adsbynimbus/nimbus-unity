#if UNITY_EDITOR
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace AdsByNimbus.Editor {
	public class AndroidSpecificSettings : EditorWindow
	{
		private bool _kotlinUpgradeEnabled;
		private bool _gradleUpgradeEnabled;
		private const string KotlinUpgradeMacro = "NIMBUS_ENABLE_KOTLIN_UPGRADE";
		private const string GradleUpgradeMacro = "NIMBUS_ENABLE_GRADLE_UPGRADE";
		private const string Enabled = "Enabled";
		private const string Disabled = "Disabled";
		private const string ButtonMessageTemplate = @"{0} {1}?";

		private void OnEnable()
		{
			UpdateSettings();
		}

		[MenuItem("Nimbus/Android Settings")]
		public static void ThirdPartySDKIntegrationMacros() {
			GetWindow<AndroidSpecificSettings>("Android Settings");
		}

		private void OnGUI() {
			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("Enable Android Specific Settings", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 4);
			GUILayout.Space(10);

			EditorGUILayout.LabelField($"Upgrade Kotlin Version to 2.2", headerStyle);
			GUILayout.Space(2);
			var androidKotlinButtonText = _kotlinUpgradeEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Kotlin Upgrade")
				: string.Format(ButtonMessageTemplate, "Enable", "Kotlin Upgrade");
			if (GUILayout.Button(androidKotlinButtonText)) {
				if (_kotlinUpgradeEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, KotlinUpgradeMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, KotlinUpgradeMacro); 
				}
			}
			GUILayout.Space(10);
			EditorGUILayout.LabelField($"Upgrade Gradle Version to 8.14.5", headerStyle);
			GUILayout.Space(2);
			var androidGradleButtonText = _gradleUpgradeEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Gradle Upgrade")
				: string.Format(ButtonMessageTemplate, "Enable", "Gradle Upgrade");
			if (GUILayout.Button(androidGradleButtonText)) {
				if (_gradleUpgradeEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, GradleUpgradeMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, GradleUpgradeMacro); 
				}
			}
		}
		
		private void OnInspectorUpdate() {
			UpdateSettings();
			Repaint();
		}
		
		private void UpdateSettings() {
			_kotlinUpgradeEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, KotlinUpgradeMacro);
			_gradleUpgradeEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, GradleUpgradeMacro);
		}
	}
}
#endif