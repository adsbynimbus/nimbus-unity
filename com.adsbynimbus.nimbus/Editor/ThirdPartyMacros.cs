#if UNITY_EDITOR
using Nimbus.Runtime.Scripts;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Nimbus.Editor {
	public class ThirdPartyMacros : EditorWindow
	{
		private bool _androidLiveRampIsEnabled;
		private bool _iosLiveRampIsEnabled;
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
		private bool _androidMobileFuseIsEnabled;
		private bool _iosMobileFuseIsEnabled;
		private bool _androidMolocoIsEnabled;
		private bool _iosMolocoIsEnabled;
		private bool _androidInMobiIsEnabled;
		private bool _iosInMobiIsEnabled;
		private const string LiveRampMacro = "NIMBUS_ENABLE_LIVERAMP";
		private const string ApsMacro = "NIMBUS_ENABLE_APS";
		private const string VungleMacro = "NIMBUS_ENABLE_VUNGLE";
		private const string MetaMacro = "NIMBUS_ENABLE_META";
		private const string AdMobMacro = "NIMBUS_ENABLE_ADMOB";
		private const string MintegralMacro = "NIMBUS_ENABLE_MINTEGRAL";
		private const string UnityAdsMacro = "NIMBUS_ENABLE_UNITY_ADS";
		private const string MobileFuseMacro = "NIMBUS_ENABLE_MOBILEFUSE";
		private const string MolocoMacro = "NIMBUS_ENABLE_MOLOCO";
		private const string InMobiMacro = "NIMBUS_ENABLE_INMOBI";
		// Android-specific Macros (for Unity Editor Configurations only)
		private const string LiveRampAndroidMacro = "NIMBUS_ENABLE_LIVERAMP_ANDROID";
		private const string ApsAndroidMacro = "NIMBUS_ENABLE_APS_ANDROID";
		private const string VungleAndroidMacro = "NIMBUS_ENABLE_VUNGLE_ANDROID";
		private const string MetaAndroidMacro = "NIMBUS_ENABLE_META_ANDROID";
		private const string AdMobAndroidMacro = "NIMBUS_ENABLE_ADMOB_ANDROID";
		private const string MintegralAndroidMacro = "NIMBUS_ENABLE_MINTEGRAL_ANDROID";
		private const string UnityAdsAndroidMacro = "NIMBUS_ENABLE_UNITY_ADS_ANDROID";
		private const string MobileFuseAndroidMacro = "NIMBUS_ENABLE_MOBILEFUSE_ANDROID";
		private const string MolocoAndroidMacro = "NIMBUS_ENABLE_MOLOCO_ANDROID";
		private const string InMobiAndroidMacro = "NIMBUS_ENABLE_INMOBI_ANDROID";
		// iOS-specific Macros (for Unity Editor Configurations only)
		private const string LiveRampIOSMacro = "NIMBUS_ENABLE_LIVERAMP_IOS";
		private const string ApsIOSMacro = "NIMBUS_ENABLE_APS_IOS";
		private const string VungleIOSMacro = "NIMBUS_ENABLE_VUNGLE_IOS";
		private const string MetaIOSMacro = "NIMBUS_ENABLE_META_IOS";
		private const string AdMobIOSMacro = "NIMBUS_ENABLE_ADMOB_IOS";
		private const string MintegralIOSMacro = "NIMBUS_ENABLE_MINTEGRAL_IOS";
		private const string UnityAdsIOSMacro = "NIMBUS_ENABLE_UNITY_ADS_IOS";
		private const string MobileFuseIOSMacro = "NIMBUS_ENABLE_MOBILEFUSE_IOS";
		private const string MolocoIOSMacro = "NIMBUS_ENABLE_MOLOCO_IOS";
		private const string InMobiIOSMacro = "NIMBUS_ENABLE_INMOBI_IOS";

		private const string Enabled = "Enabled";
		private const string Disabled = "Disabled";
		private const string ButtonMessageTemplate = @"{0} {1} Build Macro For {2}?";
		private const string LiveRampPartnerStr = "LiveRamp";
		private const string ApsPartnerStr = "APS";
		private const string VunglePartnerStr = "Vungle";
		private const string MetaPartnerStr = "Meta";
		private const string AdMobPartnerStr = "AdMob";
		private const string MintegralPartnerStr = "Mintegral";
		private const string UnityAdsPartnerStr = "Unity Ads";
		private const string MobileFusePartnerStr = "MobileFuse";
		private const string MolocoPartnerStr = "Moloco";
		private const string InMobiPartnerStr = "InMobi";
		
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
			
			// START OF LIVERAMP
			EditorGUILayout.LabelField("LiveRamp Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var liveRampAndroidStatus = _androidLiveRampIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {liveRampAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidLiveRampButtonText = _androidLiveRampIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "LiveRamp", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "LiveRamp", "Android");
			if (GUILayout.Button(androidLiveRampButtonText)) {
				if (_androidLiveRampIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, LiveRampMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(LiveRampAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, LiveRampMacro); 
					MacroHelpers.SetBuildMacroForBothPlatforms(LiveRampAndroidMacro);
					FocusOnGameManager(LiveRampPartnerStr);
				}
			}

			GUILayout.Space(5);

			var liveRampIosStatus = _iosLiveRampIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {liveRampIosStatus}", headerStyle);
			GUILayout.Space(2);
			var liveRampAndroidbuttonText = _iosLiveRampIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "LiveRamp", "Ios")
				: string.Format(ButtonMessageTemplate, "Enable", "LiveRamp", "Ios");
			if (GUILayout.Button(liveRampAndroidbuttonText)) {
				if (_iosLiveRampIsEnabled) { 
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, LiveRampMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(LiveRampIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, LiveRampMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(LiveRampIOSMacro);
					FocusOnGameManager(LiveRampPartnerStr);
				}
			}
			// END OF LIVERAMP
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);

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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(ApsAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, ApsMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(ApsAndroidMacro);

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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(ApsIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, ApsMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(ApsIOSMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(VungleAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, VungleMacro); 
					MacroHelpers.SetBuildMacroForBothPlatforms(VungleAndroidMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(VungleIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, VungleMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(VungleIOSMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro); 
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MetaAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, MetaMacro); 
					MacroHelpers.SetBuildMacroForBothPlatforms(MetaAndroidMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro); 
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MetaIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, MetaMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MetaIOSMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro); 
					MacroHelpers.RemoveBuildMacroForBothPlatforms(AdMobAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, AdMobMacro); 
					MacroHelpers.SetBuildMacroForBothPlatforms(AdMobAndroidMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(AdMobIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, AdMobMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(AdMobIOSMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MintegralAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, MintegralMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MintegralAndroidMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MintegralIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, MintegralMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MintegralIOSMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, UnityAdsMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(UnityAdsAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, UnityAdsMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(UnityAdsAndroidMacro);
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
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, UnityAdsMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(UnityAdsIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, UnityAdsMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(UnityAdsIOSMacro);
					EditorUtil.LogWithHelpBox(
						"Don't forget to add your iOS Unity Ads Game Id to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(UnityAdsPartnerStr);
				}
			}
			// END OF UNITY ADS
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			// START OF MOBILEFUSE
			EditorGUILayout.LabelField("MobileFuse Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var mobileFuseAndroidStatus = _androidMobileFuseIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {mobileFuseAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidMobileFusebuttonText = _androidMobileFuseIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "MobileFuse", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "MobileFuse", "Android");
			if (GUILayout.Button(androidMobileFusebuttonText)) {
				if (_androidMobileFuseIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, MobileFuseMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MobileFuseAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, MobileFuseMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MobileFuseAndroidMacro);
					FocusOnGameManager(MobileFusePartnerStr);
				}
			}

			GUILayout.Space(5);

			var mobileFuseIosStatus = _iosMobileFuseIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {mobileFuseIosStatus}", headerStyle);
			GUILayout.Space(2);
			var mobileFuseIosButtonText = _iosMobileFuseIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "MobileFuse", "iOS")
				: string.Format(ButtonMessageTemplate, "Enable", "MobileFuse", "iOS");
			if (GUILayout.Button(mobileFuseIosButtonText)) {
				if (_iosMobileFuseIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MobileFuseMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MobileFuseIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS, MobileFuseMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MobileFuseIOSMacro);
					FocusOnGameManager(MobileFusePartnerStr);
				}
			}
			// END OF MOBILEFUSE
			
			GUILayout.Space(10);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			
			// START OF MOLOCO
			EditorGUILayout.LabelField("Moloco Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var molocoAndroidStatus = _androidMolocoIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {molocoAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidMolocoButtonText = _androidMolocoIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Moloco", "Android")
				: string.Format(ButtonMessageTemplate, "Enable", "Moloco", "Android");
			if (GUILayout.Button(androidMolocoButtonText)) {
				if (_androidMolocoIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, MolocoMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MolocoAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, MolocoMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MolocoAndroidMacro);
					EditorUtil.LogWithHelpBox("Don't forget to add your Android Moloco App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(MolocoPartnerStr);
				}
			}

			GUILayout.Space(5);

			var molocoIosStatus = _iosMolocoIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {molocoIosStatus}", headerStyle);
			GUILayout.Space(2);
			var molocoIosButtonText = _iosMolocoIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", "Moloco", "iOS")
				: string.Format(ButtonMessageTemplate, "Enable", "Moloco", "iOS");
			if (GUILayout.Button(molocoIosButtonText)) {
				if (_iosMolocoIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, MolocoMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(MolocoIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS,MolocoMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(MolocoIOSMacro);
					EditorUtil.LogWithHelpBox(
						"Don't forget to add your iOS Moloco App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(MolocoPartnerStr);
				}
			}
			// END OF MOLOCO
			
			// START OF INMOBI
			EditorGUILayout.LabelField($"{InMobiPartnerStr} Build Macro Settings:", headerStyle);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
			GUILayout.Space(10);

			var inMobiAndroidStatus = _androidInMobiIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Android is: {inMobiAndroidStatus}", headerStyle);
			GUILayout.Space(2);
			var androidInMobiButtonText = _androidInMobiIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", InMobiPartnerStr, "Android")
				: string.Format(ButtonMessageTemplate, "Enable", InMobiPartnerStr, "Android");
			if (GUILayout.Button(androidInMobiButtonText)) {
				if (_androidInMobiIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.Android, InMobiMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(InMobiAndroidMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.Android, InMobiMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(InMobiAndroidMacro);
					EditorUtil.LogWithHelpBox($"Don't forget to add your Android {InMobiPartnerStr} Account Id to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(InMobiPartnerStr);
				}
			}

			GUILayout.Space(5);

			var inMobiIosStatus = _iosInMobiIsEnabled ? Enabled : Disabled;
			EditorGUILayout.LabelField($"Macro is set for Ios is: {inMobiIosStatus}", headerStyle);
			GUILayout.Space(2);
			var inMobiIosButtonText = _iosInMobiIsEnabled
				? string.Format(ButtonMessageTemplate, "Remove", InMobiPartnerStr, "iOS")
				: string.Format(ButtonMessageTemplate, "Enable", InMobiPartnerStr, "iOS");
			if (GUILayout.Button(inMobiIosButtonText)) {
				if (_iosInMobiIsEnabled) {
					MacroHelpers.RemoveBuildMacroForGroup(BuildTargetGroup.iOS, InMobiMacro);
					MacroHelpers.RemoveBuildMacroForBothPlatforms(InMobiIOSMacro);
				}
				else {
					MacroHelpers.SetBuildMacroForGroup(BuildTargetGroup.iOS,InMobiMacro);
					MacroHelpers.SetBuildMacroForBothPlatforms(InMobiIOSMacro);
					EditorUtil.LogWithHelpBox(
						$"Don't forget to add your iOS {InMobiPartnerStr} App Key to the NimbusSDKConfiguration scriptable object attached to your NimbusAdManager game object.", MessageType.Warning);
					FocusOnGameManager(InMobiPartnerStr);
				}
			}
			// END OF INMOBI
			
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}
		
		private void OnInspectorUpdate() {
			UpdateSettings();
			Repaint();
		}
		private void UpdateSettings() {
			_androidLiveRampIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, LiveRampAndroidMacro);
			_iosLiveRampIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, LiveRampIOSMacro);
			_androidApsIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, ApsAndroidMacro);
			_iosApsIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, ApsIOSMacro);
			_androidVungleIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, VungleAndroidMacro);
			_iosVungleIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, VungleIOSMacro);
			_androidMetaIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, MetaAndroidMacro);
			_iosMetaIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, MetaIOSMacro);
			_androidAdMobIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, AdMobAndroidMacro);
			_iosAdMobIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, AdMobIOSMacro);
			_androidMintegralIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, MintegralAndroidMacro);
			_iosMintegralIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, MintegralIOSMacro);
			_androidUnityAdsIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, UnityAdsAndroidMacro);
			_iosUnityAdsIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, UnityAdsIOSMacro);
			_androidMobileFuseIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, MobileFuseAndroidMacro);
			_iosMobileFuseIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, MobileFuseIOSMacro);
			_androidMolocoIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, MolocoAndroidMacro);
			_iosMolocoIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, MolocoIOSMacro);
			_androidInMobiIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.Android, InMobiAndroidMacro);
			_iosInMobiIsEnabled = MacroHelpers.IsBuildMacroSet(BuildTargetGroup.iOS, InMobiIOSMacro);
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