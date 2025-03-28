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
		private bool _androidUnityAdsIsEnabled;
		private bool _iosUnityAdsIsEnabled;
		private const string ApsMacro = "NIMBUS_ENABLE_APS";
		private const string VungleMacro = "NIMBUS_ENABLE_VUNGLE";
		private const string MetaMacro = "NIMBUS_ENABLE_META";
		private const string AdMobMacro = "NIMBUS_ENABLE_ADMOB";
		private const string MintegralMacro = "NIMBUS_ENABLE_MINTEGRAL";
		private const string UnityAdsMacro = "NIMBUS_ENABLE_UNITY_ADS";
		// Android-specific Macros (for Unity Editor Configurations only)
		private const string ApsAndroidMacro = "NIMBUS_ENABLE_APS_ANDROID";
		private const string VungleAndroidMacro = "NIMBUS_ENABLE_VUNGLE_ANDROID";
		private const string MetaAndroidMacro = "NIMBUS_ENABLE_META_ANDROID";
		private const string AdMobAndroidMacro = "NIMBUS_ENABLE_ADMOB_ANDROID";
		private const string MintegralAndroidMacro = "NIMBUS_ENABLE_MINTEGRAL_ANDROID";
		private const string UnityAdsAndroidMacro = "NIMBUS_ENABLE_UNITY_ADS_ANDROID";
		// iOS-specific Macros (for Unity Editor Configurations only)
		private const string ApsIOSMacro = "NIMBUS_ENABLE_APS_IOS";
		private const string VungleIOSMacro = "NIMBUS_ENABLE_VUNGLE_IOS";
		private const string MetaIOSMacro = "NIMBUS_ENABLE_META_IOS";
		private const string AdMobIOSMacro = "NIMBUS_ENABLE_ADMOB_IOS";
		private const string MintegralIOSMacro = "NIMBUS_ENABLE_MINTEGRAL_IOS";
		private const string UnityAdsIOSMacro = "NIMBUS_ENABLE_UNITY_ADS_IOS";
		
		private const string Enabled = "Enabled";
		private const string Disabled = "Disabled";
		private const string ButtonMessageTemplate = @"{0} {1} Build Macro For {2}?";
		private const string ApsPartnerStr = "APS";
		private const string VunglePartnerStr = "Vungle";
		private const string MetaPartnerStr = "Meta";
		private const string AdMobPartnerStr = "AdMob";
		private const string MintegralPartnerStr = "Mintegral";
		private const string UnityAdsPartnerStr = "Unity Ads";
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
					RemoveBuildMacroForBothPlatforms(ApsAndroidMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
					SetBuildMacroForBothPlatforms(ApsAndroidMacro);

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
					RemoveBuildMacroForBothPlatforms(ApsIOSMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
					SetBuildMacroForBothPlatforms(ApsIOSMacro);
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
					RemoveBuildMacroForBothPlatforms(VungleAndroidMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro); 
					SetBuildMacroForBothPlatforms(VungleAndroidMacro);
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
					RemoveBuildMacroForBothPlatforms(VungleIOSMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
					SetBuildMacroForBothPlatforms(VungleIOSMacro);
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
					RemoveBuildMacroForBothPlatforms(MetaAndroidMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro); 
					SetBuildMacroForBothPlatforms(MetaAndroidMacro);
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
					RemoveBuildMacroForBothPlatforms(MetaIOSMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro);
					SetBuildMacroForBothPlatforms(MetaIOSMacro);
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
					RemoveBuildMacroForBothPlatforms(AdMobAndroidMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro); 
					SetBuildMacroForBothPlatforms(AdMobAndroidMacro);
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
					RemoveBuildMacroForBothPlatforms(AdMobIOSMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
					SetBuildMacroForBothPlatforms(AdMobIOSMacro);
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
					RemoveBuildMacroForBothPlatforms(MintegralAndroidMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
					SetBuildMacroForBothPlatforms(MintegralAndroidMacro);
					EditorUtil.LogWithHelpBox("Don't forget to add your Android Mintegral App Id and App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
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
					RemoveBuildMacroForBothPlatforms(MintegralIOSMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
					SetBuildMacroForBothPlatforms(MintegralIOSMacro);
					EditorUtil.LogWithHelpBox(
						"Don't forget to add your iOS Mintegral App Id and App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
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

			var unityAdsAndroidStatus = _androidUnityAdsIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {unityAdsAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidUnityAdsbuttonText = _androidUnityAdsIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Unity Ads", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "Unity Ads", "Android");
			if (GUILayout.Button(androidUnityAdsbuttonText)) {
				if (_androidUnityAdsIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.Android, UnityAdsMacro);
					RemoveBuildMacroForBothPlatforms(UnityAdsAndroidMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.Android, UnityAdsMacro);
					SetBuildMacroForBothPlatforms(UnityAdsAndroidMacro);
					EditorUtil.LogWithHelpBox("Don't forget to add your Android Unity Ads Game Id to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(UnityAdsPartnerStr);
				}
			}

			GUILayout.Space(5);

			var unityAdsIosStatus = _iosUnityAdsIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {unityAdsIosStatus}", headerStyle);
			GUILayout.Space(2);
			var unityAdsIosButtonText = _iosUnityAdsIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Unity Ads", "iOS")
				: string.Format(ButtonMessageTemplate, "Enable", "Unity Ads", "iOS");
			if (GUILayout.Button(unityAdsIosButtonText)) {
				if (_iosUnityAdsIsEnabled) {
					RemoveBuildMacroForGroup(BuildTargetGroup.iOS, UnityAdsMacro);
					RemoveBuildMacroForBothPlatforms(UnityAdsIOSMacro);
				}
				else {
					SetBuildMacroForGroup(BuildTargetGroup.iOS, UnityAdsMacro);
					SetBuildMacroForBothPlatforms(UnityAdsIOSMacro);
					EditorUtil.LogWithHelpBox(
						"Don't forget to add your iOS Unity Ads Game Id to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
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
			_androidApsIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, ApsAndroidMacro);
			_iosApsIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, ApsIOSMacro);
			_androidVungleIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, VungleAndroidMacro);
			_iosVungleIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, VungleIOSMacro);
			_androidMetaIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, MetaAndroidMacro);
			_iosMetaIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, MetaIOSMacro);
			_androidAdMobIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, AdMobAndroidMacro);
			_iosAdMobIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, AdMobIOSMacro);
			_androidMintegralIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, MintegralAndroidMacro);
			_iosMintegralIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, MintegralIOSMacro);
			_androidUnityAdsIsEnabled = IsBuildMacroSet(BuildTargetGroup.Android, UnityAdsAndroidMacro);
			_iosUnityAdsIsEnabled = IsBuildMacroSet(BuildTargetGroup.iOS, UnityAdsIOSMacro);
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

		private void RemoveBuildMacroForBothPlatforms(string buildMacro)
		{
			RemoveBuildMacroForGroup(BuildTargetGroup.iOS, buildMacro);
			RemoveBuildMacroForGroup(BuildTargetGroup.Android, buildMacro);
		}
		
		private void SetBuildMacroForBothPlatforms(string buildMacro)
		{
			SetBuildMacroForGroup(BuildTargetGroup.iOS, buildMacro);
			SetBuildMacroForGroup(BuildTargetGroup.Android, buildMacro);
		}
		
		
	}
}
#endif