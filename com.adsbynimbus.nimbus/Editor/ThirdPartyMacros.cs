#if UNITY_EDITOR
using System.Linq;
using Nimbus.Runtime.Scripts;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Nimbus.Editor {
	public class ThirdPartyMacros : EditorWindow {
		private bool _androidApsIsEnabled;
		private bool _iosApsIsEnabled;
		private bool _androidVungleIsEnabled;
		private bool _iosVungleIsEnabled;
		private const string ApsMacro = "NIMBUS_ENABLE_APS";
		private const string VungleMacro = "NIMBUS_ENABLE_VUNGLE";
		private const string Enabled = "Enabled";
		private const string Disabled = "Disabled";
		private const string ButtonMessageTemplate = @"{0} {1} Build Macro For {2}?";
		private const string ApsPartnerStr = "APS";
		private const string VunglePartnerStr = "Liftoff Monetize";

		private void OnEnable() {
			UpdateSettings();
		}


		[MenuItem("Nimbus/Third Party SDK Settings")]
		public static void ThirdPartySDKIntegrationMacros() {
			GetWindow<ThirdPartyMacros>("Third Party SDK Settings");
		}


		private void OnGUI() {
			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("Enable Third Party SDK Support", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 4);

			// START OF APS
			EditorGUILayout.LabelField("APS Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var status = _androidApsIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {status}", headerStyle);
			GUILayout.Space(2);
			var buttonText = _androidApsIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "APS", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "APS", "Android");
			if (GUILayout.Button(buttonText)) {
				if (_androidApsIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Android APS App Ids and APS Slot Ids to the " +
					                         "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(ApsPartnerStr);
				}
			}

			GUILayout.Space(5);

			status = _iosApsIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {status}", headerStyle);
			GUILayout.Space(2);
			buttonText = _iosApsIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "APS", "Ios")
				: string.Format(ButtonMessageTemplate, "Enable", "APS", "Ios");
			if (GUILayout.Button(buttonText)) {
				if (_iosApsIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your IOS APS App Ids and APS Slot Ids to the " +
					                         "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(ApsPartnerStr);
				}
			}
			// END OF APS
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			// START OF VUNGLE
			EditorGUILayout.LabelField("Liftoff Monetize Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var vungleAndroidStatus = _androidVungleIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {vungleAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidVunglebuttonText = _androidVungleIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Liftoff Monetize", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "Liftoff Monetize", "Android");
			if (GUILayout.Button(androidVunglebuttonText)) {
				if (_androidVungleIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Liftoff Monetize Android App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(VunglePartnerStr);
				}
			}

			GUILayout.Space(5);

			var vungleIosStatus = _iosVungleIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {vungleIosStatus}", headerStyle);
			GUILayout.Space(2);
			var vungleAndroidButtonText = _iosVungleIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Liftoff Monetize", "Ios")
				: string.Format(ButtonMessageTemplate, "Enable", "Liftoff Monetize", "Ios");
			if (GUILayout.Button(vungleAndroidButtonText)) {
				if (_iosVungleIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Liftoff Monetize IOS App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(VunglePartnerStr);
				}
			}
			// END OF VUNGLE
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
		}
		
		private void OnInspectorUpdate() {
			UpdateSettings();
			Repaint();
		}
		private void UpdateSettings() {
			_androidApsIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, ApsMacro);
			_iosApsIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, ApsMacro);
			_androidVungleIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, VungleMacro);
			_iosVungleIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, VungleMacro);
		}


		private static bool IsBuildMacroSet(BuildTargetGroup group, string buildMacro) {
			PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out var macros);
			return macros.Any(macro => macro == buildMacro);
		}

		private static void SetBuildMacroForGroup(BuildTargetGroup group, string buildMacro) {
			PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out var macros);
			if (macros.Any(macro => macro == buildMacro)) {
				return;
			}

			var enumerable = macros.Append(buildMacro);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, enumerable.ToArray());
		}

		private static void RemoveBuildMacroForGroup(BuildTargetGroup group, string buildMacro) {
			PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out var macros);
			macros = macros.Where((source, index) => source != buildMacro).ToArray();
			PlayerSettings.SetScriptingDefineSymbolsForGroup(group, macros.ToArray());
		}

		private static void FocusOnGameManager(string partner) {
			var manager = FindObjectOfType<NimbusManager>();
			if (manager != null) {
				Selection.activeGameObject = manager.gameObject;
			}
			else {
				EditorUtil.LogWithHelpBox($"{partner} was enabled however there is no NimbusAdManager located in your scene, " +
				                         "please add a NimbusGameManager to you scene. In the ToolBar Go to Nimbus -> Create New NimbusAdManager",
					MessageType.Error);
			}
		}
		
		
	}
}
#endif