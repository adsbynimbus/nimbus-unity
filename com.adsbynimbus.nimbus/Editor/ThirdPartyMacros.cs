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
		private bool _androidMetaIsEnabled;
		private bool _iosMetaIsEnabled;
		private bool _androidAdMobIsEnabled;
		private bool _iosAdMobIsEnabled;
		private bool _androidMintegralIsEnabled;
		private bool _iosMintegralIsEnabled;
		private const string ApsMacro = "NIMBUS_ENABLE_APS";
		private const string VungleMacro = "NIMBUS_ENABLE_VUNGLE";
		private const string MetaMacro = "NIMBUS_ENABLE_META";
		private const string AdMobMacro = "NIMBUS_ENABLE_ADMOB";
		private const string MintegralMacro = "NIMBUS_ENABLE_MINTEGRAL";
		private const string Enabled = "Enabled";
		private const string Disabled = "Disabled";
		private const string ButtonMessageTemplate = @"{0} {1} Build Macro For {2}?";
		private const string ApsPartnerStr = "APS";
		private const string VunglePartnerStr = "Vungle";
		private const string MetaPartnerStr = "Meta";
		private const string AdMobPartnerStr = "AdMob";
		private const string MintegralPartnerStr = "Mintegral";
		Vector2 scrollPos;

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
			EditorGUILayout.BeginVertical();
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(800), GUILayout.Height(600));
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
			EditorGUILayout.LabelField("Vungle Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var vungleAndroidStatus = _androidVungleIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {vungleAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidVunglebuttonText = _androidVungleIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Vungle", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "Vungle", "Android");
			if (GUILayout.Button(androidVunglebuttonText)) {
				if (_androidVungleIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Android Vungle App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(VunglePartnerStr);
				}
			}

			GUILayout.Space(5);

			var vungleIosStatus = _iosVungleIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {vungleIosStatus}", headerStyle);
			GUILayout.Space(2);
			var vungleAndroidButtonText = _iosVungleIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Vungle", "Ios")
				: string.Format(ButtonMessageTemplate, "Enable", "Vungle", "Ios");
			if (GUILayout.Button(vungleAndroidButtonText)) {
				if (_iosVungleIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your IOS Vungle App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(VunglePartnerStr);
				}
			}
			// END OF VUNGLE
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			// START OF META
			EditorGUILayout.LabelField("Meta Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var metaAndroidStatus = _androidMetaIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {metaAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidMetabuttonText = _androidMetaIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Meta", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "Meta", "Android");
			if (GUILayout.Button(androidMetabuttonText)) {
				if (_androidMetaIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Android Meta App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(MetaPartnerStr);
				}
			}

			GUILayout.Space(5);

			var metaIosStatus = _iosMetaIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {metaIosStatus}", headerStyle);
			GUILayout.Space(2);
			var metaAndroidButtonText = _iosMetaIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Meta", "Ios")
				: string.Format(ButtonMessageTemplate, "Enable", "Meta", "Ios");
			if (GUILayout.Button(metaAndroidButtonText)) {
				if (_iosMetaIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your IOS Meta App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(MetaPartnerStr);
				}
			}
			// END OF META
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			// START OF ADMOB
			EditorGUILayout.LabelField("AdMob Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var adMobAndroidStatus = _androidAdMobIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {adMobAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidAdMobbuttonText = _androidAdMobIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "AdMob", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "AdMob", "Android");
			if (GUILayout.Button(androidAdMobbuttonText)) {
				if (_androidAdMobIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Android AdMob App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(AdMobPartnerStr);
				}
			}

			GUILayout.Space(5);

			var adMobIosStatus = _iosAdMobIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {adMobIosStatus}", headerStyle);
			GUILayout.Space(2);
			var adMobIosButtonText = _iosAdMobIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "AdMob", "Ios")
				: string.Format(ButtonMessageTemplate, "Enable", "AdMob", "Ios");
			if (GUILayout.Button(adMobIosButtonText)) {
				if (_iosAdMobIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your IOS AdMob App Id to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(AdMobPartnerStr);
				}
			}
			// END OF ADMOB
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
						
			// START OF MINTEGRAL
			EditorGUILayout.LabelField("Mintegral Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var mintegralAndroidStatus = _androidMintegralIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {mintegralAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidMintegralbuttonText = _androidMintegralIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Mintegral", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "Mintegral", "Android");
			if (GUILayout.Button(androidMintegralbuttonText)) {
				if (_androidMintegralIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Android Mintegral App Id and App Key to the " +
					                          "NimbusSDKConfiguration Scriptable object attached to your NimbusAdManager game object", MessageType.Warning);
					FocusOnGameManager(MintegralPartnerStr);
				}
			}

			GUILayout.Space(5);

			var mintegralIosStatus = _iosMintegralIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {mintegralIosStatus}", headerStyle);
			GUILayout.Space(2);
			var mintegralIosButtonText = _iosMintegralIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Mintegral", "iOS")
				: string.Format(ButtonMessageTemplate, "Enable", "Mintegral", "iOS");
			if (GUILayout.Button(mintegralIosButtonText)) {
				if (_iosMintegralIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
					EditorUtil.LogWithHelpBox(
						"Don't forget to add your iOS Mintegral App Id and App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(MintegralPartnerStr);
				}
			}
			// END OF MINTEGRAL
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
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
			_androidMetaIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, MetaMacro);
			_iosMetaIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, MetaMacro);
			_androidAdMobIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, AdMobMacro);
			_iosAdMobIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, AdMobMacro);
			_androidMintegralIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, MintegralMacro);
			_iosMintegralIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, MintegralMacro);
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