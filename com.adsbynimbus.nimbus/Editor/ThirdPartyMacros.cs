#if UNITY_EDITOR
using System.Linq;
using Nimbus.Runtime.Scripts;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Nimbus.Editor {
	public class ThirdPartyMacros : EditorWindow {
		private bool _apsIsEnabled;
		private bool _vungleIsEnabled;
		private bool _metaIsEnabled;
		private bool _adMobIsEnabled;
		private bool _mintegralIsEnabled;
		private bool _unityAdsIsEnabled;
		private const string ApsMacro = "NIMBUS_ENABLE_APS";
		private const string VungleMacro = "NIMBUS_ENABLE_VUNGLE";
		private const string MetaMacro = "NIMBUS_ENABLE_META";
		private const string AdMobMacro = "NIMBUS_ENABLE_ADMOB";
		private const string MintegralMacro = "NIMBUS_ENABLE_MINTEGRAL";
		private const string UnityAdsMacro = "NIMBUS_ENABLE_UNITY_ADS";
		private const string Enabled = "Enabled";
		private const string Disabled = "Disabled";
		private const string ButtonMessageTemplate = @"{0} {1} Build Macro?";
		private const string ApsPartnerStr = "APS";
		private const string VunglePartnerStr = "Vungle";
		private const string MetaPartnerStr = "Meta";
		private const string AdMobPartnerStr = "AdMob";
		private const string MintegralPartnerStr = "Mintegral";
		private const string UnityAdsPartnerStr = "Unity Ads";
		Vector2 _scrollPos;

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
			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(800), GUILayout.Height(600));
			EditorGUILayout.LabelField("Enable Third Party SDK Support", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 4);

			// START OF APS
			EditorGUILayout.LabelField("APS Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var status = _apsIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for APS is: {status}", headerStyle);
			GUILayout.Space(2);
			var buttonText = _apsIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "APS")
				: string.Format(ButtonMessageTemplate, "Enable", "APS");
			if (GUILayout.Button(buttonText)) {
				if (_apsIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
					SetBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your APS App Ids and APS Slot Ids to the " +
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

			var vungleStatus = _vungleIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Vungle is: {vungleStatus}", headerStyle);
			GUILayout.Space(2);
			var vunglebuttonText = _vungleIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Vungle")
				: string.Format(ButtonMessageTemplate, "Enable", "Vungle");
			if (GUILayout.Button(vunglebuttonText)) {
				if (_vungleIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
					SetBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Vungle App Id to the " +
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

			var metaStatus = _metaIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Meta is: {metaStatus}", headerStyle);
			GUILayout.Space(2);
			var metabuttonText = _metaIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Meta")
				: string.Format(ButtonMessageTemplate, "Enable", "Meta");
			if (GUILayout.Button(metabuttonText)) {
				if (_metaIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro);
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro);
					SetBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your Meta App Id to the " +
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

			var adMobStatus = _adMobIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for AdMob is: {adMobStatus}", headerStyle);
			GUILayout.Space(2);
			var adMobbuttonText = _adMobIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "AdMob")
				: string.Format(ButtonMessageTemplate, "Enable", "AdMob");
			if (GUILayout.Button(adMobbuttonText)) {
				if (_adMobIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro);
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro);
					SetBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
					EditorUtil.LogWithHelpBox("Don't Forget To Add your AdMob App Id to the " +
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

			var mintegralStatus = _mintegralIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {mintegralStatus}", headerStyle);
			GUILayout.Space(2);
			var mintegralbuttonText = _mintegralIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Mintegral")
				: string.Format(ButtonMessageTemplate, "Enable", "Mintegral");
			if (GUILayout.Button(mintegralbuttonText)) {
				if (_mintegralIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
					SetBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
					EditorUtil.LogWithHelpBox("Don't forget to add your Mintegral App Id and App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(MintegralPartnerStr);
				}
			}
			// END OF MINTEGRAL
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			// START OF UNITY ADS
			EditorGUILayout.LabelField("Unity Ads Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var unityAdsStatus = _unityAdsIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {unityAdsStatus}", headerStyle);
			GUILayout.Space(2);
			var unityAdsButtonText = _unityAdsIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Unity Ads")
				: string.Format(ButtonMessageTemplate, "Enable", "Unity Ads");
			if (GUILayout.Button(unityAdsButtonText)) {
				if (_unityAdsIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, UnityAdsMacro);
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, UnityAdsMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, UnityAdsMacro);
					SetBuildMacroForGroup(BuildTargetGroup.iOS, UnityAdsMacro);
					EditorUtil.LogWithHelpBox("Don't forget to add your Unity Ads Game Id to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(UnityAdsPartnerStr);
				}
			}
			// END OF UNITY ADS
			
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
			_apsIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, ApsMacro) && IsBuildMacroSet(BuildTargetGroup.iOS, ApsMacro);
			_vungleIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, VungleMacro) && IsBuildMacroSet(BuildTargetGroup.iOS, VungleMacro);
			_metaIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, MetaMacro) && IsBuildMacroSet(BuildTargetGroup.iOS, MetaMacro);
			_adMobIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, AdMobMacro) && IsBuildMacroSet(BuildTargetGroup.iOS, AdMobMacro);
			_mintegralIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, MintegralMacro) && IsBuildMacroSet(BuildTargetGroup.iOS, MintegralMacro);
			_unityAdsIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, UnityAdsMacro) && IsBuildMacroSet(BuildTargetGroup.iOS, UnityAdsMacro);
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