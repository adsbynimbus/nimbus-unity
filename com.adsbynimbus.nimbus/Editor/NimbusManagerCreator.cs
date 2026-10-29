#if UNITY_EDITOR
using System.Collections.Generic;
using AdsByNimbus.Extensions;
using AdsByNimbus.Internal;
using AdsByNimbus.Internal.Extensions.AdMob;
using AdsByNimbus.Internal.Extensions.APS;
using AdsByNimbus.Internal.Utility;
using ScriptableObjects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

#region ThirdPartySDKImports

#endregion


namespace AdsByNimbus.Editor {
	public class NimbusManagerCreator : EditorWindow {
		private string _apiKey;
		private string _publisherKey;
		private bool _enableUnityLogs = true;
		private bool _enableSDKInTestMode;
		private bool _enableManualInitialization = false;
		private NimbusSDKConfiguration _asset = null;

		// APS
		private SerializedProperty _androidApsAppKey;
		private ReorderableList _androidApsSlotIdList = null;
		private SerializedProperty _androidApsSlots = null;

		private SerializedProperty _iosApsAppKey;
		private ReorderableList _iosApsSlotIdList = null;
		private SerializedProperty _iosApsSlots = null;
		
		// Vungle
		private SerializedProperty _androidVungleAppId;
		
		private SerializedProperty _iosVungleAppId;
		
		// Meta
		private SerializedProperty _androidMetaAppId;
		
		private SerializedProperty _iosMetaAppId;
		
		// AdMob
		private bool _adMobAutoInit;
		private SerializedProperty _androidAdMobAppId;
		private ReorderableList _androidAdMobAdUnitDataList = null;
		private SerializedProperty _androidAdMobAdUnitData = null;
		
		private SerializedProperty _iosAdMobAppId;
		private ReorderableList _iosAdMobAdUnitDataList = null;
		private SerializedProperty _iosAdMobAdUnitData = null;
		
		// Mintegral
		private SerializedProperty _androidMintegralAppId;
		private SerializedProperty _androidMintegralAppKey;
		
		private SerializedProperty _iosMintegralAppId;
		private SerializedProperty _iosMintegralAppKey;
		
		// Unity Ads
		private SerializedProperty _androidUnityAdsGameId;
		private SerializedProperty _iosUnityAdsGameId;
		
		// Moloco
		private SerializedProperty _androidMolocoAppKey;
		
		private SerializedProperty _iosMolocoAppKey;
		
		//InMobi
		private SerializedProperty _androidInMobiAccountId;
		
		private SerializedProperty _iosInMobiAccountId;
		
		//Digital Turbine
		private SerializedProperty _androidDigitalTurbineAppId;
		
		private SerializedProperty _iosDigitalTurbineAppId;
		
		//Display IO
		private SerializedProperty _androidDisplayIOAppId;
		private SerializedProperty _androidDisplayIOUserId;
		
		private SerializedProperty _iosDisplayIOAppId;
		private SerializedProperty _iosDisplayIOUserId;

		
		[MenuItem("Nimbus/Create New NimbusAdsManager")]
		public static void CreateNewNimbusGameManager() {
			GetWindow<NimbusManagerCreator>("NimbusAdsManager Creator");
		}

		private void OnEnable() {
			_asset = CreateInstance<NimbusSDKConfiguration>();
			var serializedObject = new SerializedObject(_asset);

			// APS
			// Android APS UI
			_androidApsAppKey = serializedObject.FindProperty("androidApsAppKey");
			_androidApsSlots = serializedObject.FindProperty("androidApsSlotData");
			_androidApsSlotIdList = new ReorderableList(
				serializedObject, _androidApsSlots,
				true,
				false,
				true,
				true
			);
			_androidApsSlots.isExpanded = true;
			_androidApsSlotIdList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_androidApsSlotIdList.headerHeight = 0f;
			_androidApsSlotIdList.drawElementCallback += OnDrawElementApsAndroidSlotData;

			// IOS APS UI
			_iosApsAppKey = serializedObject.FindProperty("iosApsAppKey");
			_iosApsSlots = serializedObject.FindProperty("iosApsSlotData");
			_iosApsSlotIdList = new ReorderableList(
				serializedObject, _iosApsSlots,
				true,
				false,
				true,
				true
			);
			_iosApsSlots.isExpanded = true;
			_iosApsSlotIdList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosApsSlotIdList.headerHeight = 0f;
			_iosApsSlotIdList.drawElementCallback += OnDrawElementApsIOSSlotData;
			
			// Vungle
			// Android Vungle UI
			_androidVungleAppId = serializedObject.FindProperty("androidVungleAppID");
			
			// IOS Vungle UI
			_iosVungleAppId = serializedObject.FindProperty("iosVungleAppID");
			
			// Meta
			// Android Meta UI
			_androidMetaAppId = serializedObject.FindProperty("androidMetaAppID");
			
			// IOS Meta UI
			_iosMetaAppId = serializedObject.FindProperty("iosMetaAppID");
			
			// AdMob
			// Android AdMob UI
			_androidAdMobAppId = serializedObject.FindProperty("androidAdMobAppID");
			_androidAdMobAdUnitData = serializedObject.FindProperty("androidAdMobAdUnitData");
			_androidAdMobAdUnitDataList = new ReorderableList(
				serializedObject, _androidAdMobAdUnitData,
				true,
				false,
				true,
				true
			);
			_androidAdMobAdUnitData.isExpanded = true;
			_androidAdMobAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_androidAdMobAdUnitDataList.headerHeight = 0f;
			_androidAdMobAdUnitDataList.drawElementCallback += OnDrawElementAdMobAndroidAdUnitData;
			
			// IOS AdMob UI
			_iosAdMobAppId = serializedObject.FindProperty("iosAdMobAppID");
			_iosAdMobAdUnitData = serializedObject.FindProperty("iosAdMobAdUnitData");
			_iosAdMobAdUnitDataList = new ReorderableList(
				serializedObject, _iosAdMobAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosAdMobAdUnitData.isExpanded = true;
			_iosAdMobAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosAdMobAdUnitDataList.headerHeight = 0f;
			_iosAdMobAdUnitDataList.drawElementCallback += OnDrawElementAdMobIOSAdUnitData;
			
			//Mintegral
			// Android Mintegral UI
			_androidMintegralAppId = serializedObject.FindProperty("androidMintegralAppID");
			_androidMintegralAppKey = serializedObject.FindProperty("androidMintegralAppKey");
			
			// IOS Mintegral UI
			_iosMintegralAppId = serializedObject.FindProperty("iosMintegralAppID");
			_iosMintegralAppKey = serializedObject.FindProperty("iosMintegralAppKey");
			
			// Unity Ads
			// Android Unity Ads UI
			_androidUnityAdsGameId = serializedObject.FindProperty("androidUnityAdsGameID");
			
			// IOS Unity Ads UI
			_iosUnityAdsGameId = serializedObject.FindProperty("iosUnityAdsGameID");
			
			//Moloco
			// Android Moloco UI
			_androidMolocoAppKey = serializedObject.FindProperty("androidMolocoAppKey");
			
			// IOS Moloco UI
			_iosMolocoAppKey = serializedObject.FindProperty("iosMolocoAppKey");
			
			// InMobi
			// Android InMobi UI
			_androidInMobiAccountId = serializedObject.FindProperty("androidInMobiAccountId");
			
			// IOS InMobi UI
			_iosInMobiAccountId = serializedObject.FindProperty("iosInMobiAccountId");
			
			// Digital Turbine
			// Android Digital Turbine UI
			_androidDigitalTurbineAppId= serializedObject.FindProperty("androidDigitalTurbineAppId");
			
			// IOS Digital Turbine UI
			_iosDigitalTurbineAppId = serializedObject.FindProperty("iosDigitalTurbineAppId");
			
			// Display IO
			// Android Display IO UI
			_androidDisplayIOAppId = serializedObject.FindProperty("androidDisplayIOAppId");
			_androidDisplayIOUserId= serializedObject.FindProperty("androidDisplayIOUserId");
			
			// IOS Display IO UI
			_iosDisplayIOAppId = serializedObject.FindProperty("iosDisplayIOAppId");
			_iosDisplayIOUserId= serializedObject.FindProperty("iosDisplayIOUserId");
		}


		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementApsAndroidSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementApsIOSSlotData;
			_androidAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAndroidAdUnitData;
			_iosAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobIOSAdUnitData;
		}

		private void OnDrawElementApsAndroidSlotData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidApsSlots.GetArrayElementAtIndex(index);
			item.isExpanded = true;
			var itr = item.Copy();

			itr.Next(true);
			fieldRect.y += 1.5f * fieldRect.height;
			EditorGUI.PropertyField(fieldRect, itr, false);

			var children = item.CountInProperty() - 1;
			for (var i = 0; i < children; i++) {
				EditorGUI.PropertyField(fieldRect, itr, false);
				itr.Next(false);
				fieldRect.y += fieldRect.height;
			}
		}
		private void OnDrawElementApsIOSSlotData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosApsSlots.GetArrayElementAtIndex(index);
			item.isExpanded = true;
			var itr = item.Copy();

			itr.Next(true);
			fieldRect.y += 1.5f * fieldRect.height;
			EditorGUI.PropertyField(fieldRect, itr, false);

			var children = item.CountInProperty() - 1;
			for (var i = 0; i < children; i++) {
				EditorGUI.PropertyField(fieldRect, itr, false);
				itr.Next(false);
				fieldRect.y += fieldRect.height;
			}
		}
		
		private void OnDrawElementAdMobAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidAdMobAdUnitData.GetArrayElementAtIndex(index);
			item.isExpanded = true;
			var itr = item.Copy();

			itr.Next(true);
			fieldRect.y += 1.5f * fieldRect.height;
			EditorGUI.PropertyField(fieldRect, itr, false);

			var children = item.CountInProperty() - 1;
			for (var i = 0; i < children; i++) {
				EditorGUI.PropertyField(fieldRect, itr, false);
				itr.Next(false);
				fieldRect.y += fieldRect.height;
			}
		}
		private void OnDrawElementAdMobIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosAdMobAdUnitData.GetArrayElementAtIndex(index);
			item.isExpanded = true;
			var itr = item.Copy();

			itr.Next(true);
			fieldRect.y += 1.5f * fieldRect.height;
			EditorGUI.PropertyField(fieldRect, itr, false);

			var children = item.CountInProperty() - 1;
			for (var i = 0; i < children; i++) {
				EditorGUI.PropertyField(fieldRect, itr, false);
				itr.Next(false);
				fieldRect.y += fieldRect.height;
			}
		}
		
		private void OnGUI() {
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			_publisherKey = EditorGUILayout.TextField("Publisher Key", _publisherKey);
			_apiKey = EditorGUILayout.TextField("ApiKey", _apiKey);
			_enableUnityLogs = EditorGUILayout.Toggle("Enable Unity Logger", _enableUnityLogs);
			_enableSDKInTestMode = EditorGUILayout.Toggle("Enable SDK In Test Mode", _enableSDKInTestMode);
			#if NIMBUS_ENABLE_KOTLIN_UPGRADE
				EditorGUILayout.LabelField("Android Kotlin Version upgraded to 2.2", EditorStyles.label);
			#endif
			#if NIMBUS_ENABLE_GRADLE_UPGRADE
				EditorGUILayout.LabelField("Android Gradle Version upgraded to 8.14.5", EditorStyles.label);
			#endif
			_enableManualInitialization = EditorGUILayout.Toggle("Enable Manual Initialization", _enableManualInitialization);
			
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			EditorGUIUtility.labelWidth = 200.0f; 
			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS || NIMBUS_ENABLE_MOBILEFUSE || NIMBUS_ENABLE_LIVERAMP || NIMBUS_ENABLE_MOLOCO || NIMBUS_ENABLE_INMOBI || NIMBUS_ENABLE_DIGITAL_TURBINE || NIMBUS_ENABLE_DISPLAY_IO
				EditorGUILayout.LabelField("Third Party SDK Support", headerStyle);
			#endif
			
			#if NIMBUS_ENABLE_LIVERAMP_ANDROID || NIMBUS_ENABLE_LIVERAMP_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("LiveRamp Configuration", headerStyle);
				#if NIMBUS_ENABLE_LIVERAMP_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.LabelField("LiveRamp is Enabled for Android", EditorStyles.label);
				#endif
							
				#if NIMBUS_ENABLE_LIVERAMP_IOS
					GUILayout.Space(10);
					EditorGUILayout.LabelField("LiveRamp is Enabled for iOS", EditorStyles.label);
				#endif
			#endif

			#if NIMBUS_ENABLE_APS_ANDROID || NIMBUS_ENABLE_APS_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("APS Configuration", headerStyle);
				#if NIMBUS_ENABLE_APS_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_androidApsAppKey);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidApsSlots, "APS Android Slot Id Data");
				#endif
				#if NIMBUS_ENABLE_APS_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_iosApsAppKey);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_iosApsSlots, "APS iOS Slot Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter APS data", MessageType.Warning);
				#endif
			#endif	
			
			#if NIMBUS_ENABLE_VUNGLE_ANDROID || NIMBUS_ENABLE_VUNGLE_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Vungle Configuration", headerStyle);
				#if NIMBUS_ENABLE_VUNGLE_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidVungleAppId));
				#endif
				#if NIMBUS_ENABLE_VUNGLE_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosVungleAppId));
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Vungle data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_META_ANDROID || NIMBUS_ENABLE_META_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Meta Configuration", headerStyle);
				#if NIMBUS_ENABLE_META_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidMetaAppId));
				#endif
				#if NIMBUS_ENABLE_META_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMetaAppId));
				#endif
				GUILayout.Space(10);
				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Meta data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_ADMOB_ANDROID || NIMBUS_ENABLE_ADMOB_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("AdMob Configuration", headerStyle);
				#if NIMBUS_ENABLE_ADMOB_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidAdMobAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidAdMobAdUnitData, "AdMob Android Ad Unit Id Data");
				#endif
				#if NIMBUS_ENABLE_ADMOB_IOS
					_adMobAutoInit = EditorGUILayout.Toggle("Auto Initialize", _adMobAutoInit);
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosAdMobAppId));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosAdMobAdUnitData, "AdMob iOS Ad Unit Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter AdMob data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MINTEGRAL_ANDROID || NIMBUS_ENABLE_MINTEGRAL_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Mintegral Configuration", headerStyle);
				#if NIMBUS_ENABLE_MINTEGRAL_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidMintegralAppId));
					EditorGUILayout.PropertyField((_androidMintegralAppKey));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMintegralAppId));
					EditorGUILayout.PropertyField((_iosMintegralAppKey));
					GUILayout.Space(10);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Mintegral data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_UNITY_ADS_ANDROID || NIMBUS_ENABLE_UNITY_ADS_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Unity Ads Configuration", headerStyle);
				#if NIMBUS_ENABLE_UNITY_ADS_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_androidUnityAdsGameId);
				#endif
				#if NIMBUS_ENABLE_UNITY_ADS_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_iosUnityAdsGameId);
				#endif
				GUILayout.Space(10);

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Unity Ads data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MOBILEFUSE_ANDROID || NIMBUS_ENABLE_MOBILEFUSE_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("MobileFuse Configuration", headerStyle);
				#if NIMBUS_ENABLE_MOBILEFUSE_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.LabelField("MobileFuse is Enabled for Android", EditorStyles.label);
				#endif

				#if NIMBUS_ENABLE_MOBILEFUSE_IOS
					GUILayout.Space(10);
					EditorGUILayout.LabelField("MobileFuse is Enabled for iOS", EditorStyles.label);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MOLOCO_ANDROID || NIMBUS_ENABLE_MOLOCO_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Moloco Configuration", headerStyle);
				#if NIMBUS_ENABLE_MOLOCO_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidMolocoAppKey));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif
					#if NIMBUS_ENABLE_MOLOCO_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMolocoAppKey));
					GUILayout.Space(10);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Moloco data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_INMOBI_ANDROID || NIMBUS_ENABLE_INMOBI_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("InMobi Configuration", headerStyle);
				#if NIMBUS_ENABLE_INMOBI_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidInMobiAccountId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif
					#if NIMBUS_ENABLE_INMOBI_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosInMobiAccountId));
					GUILayout.Space(10);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter InMobi data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_DIGITAL_TURBINE_ANDROID || NIMBUS_ENABLE_DIGITAL_TURBINE_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Digital Turbine Configuration", headerStyle);
				#if NIMBUS_ENABLE_DIGITAL_TURBINE_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_androidDigitalTurbineAppId);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif
					#if NIMBUS_ENABLE_DIGITAL_TURBINE_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_iosDigitalTurbineAppId);
					GUILayout.Space(10);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Digital Turbine data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_DISPLAY_IO_ANDROID || NIMBUS_ENABLE_DISPLAY_IO_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Display IO Configuration", headerStyle);
				#if NIMBUS_ENABLE_DISPLAY_IO_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_androidDisplayIOAppId);
					EditorGUILayout.PropertyField(_androidDisplayIOUserId);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif
				#if NIMBUS_ENABLE_DISPLAY_IO_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_iosDisplayIOAppId);
					EditorGUILayout.PropertyField(_iosDisplayIOUserId);
					GUILayout.Space(10);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Display IO data", MessageType.Warning);
				#endif
			#endif
			
			// ReSharper disable InvertIf
			if (GUILayout.Button("Create")) {
				_asset.publisherKey = _publisherKey;
				_asset.apiKey = _apiKey;
				_asset.enableUnityLogs = _enableUnityLogs;
				_asset.enableSDKInTestMode = _enableSDKInTestMode;
				_asset.enableManualInitialization = _enableManualInitialization;

				#if NIMBUS_ENABLE_APS_ANDROID
					HandleApsSlots(_androidApsSlots, out _asset.androidApsSlotData);
				#endif
				#if NIMBUS_ENABLE_APS_IOS
					HandleApsSlots(_iosApsSlots, out _asset.iosApsSlotData);
				#endif
				
				#if NIMBUS_ENABLE_ADMOB_ANDROID
					HandleAdMobAdUnitData(_androidAdMobAdUnitData, out _asset.androidAdMobAdUnitData);
				#endif
				#if NIMBUS_ENABLE_ADMOB_IOS
					HandleAdMobAdUnitData(_iosAdMobAdUnitData, out _asset.iosAdMobAdUnitData);
				#endif
				
				_asset.Sanitize();
				if (_asset.apiKey.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						"Apikey cannot be empty, object NimbusAdsManager not created");
					return;
				}

				if (_asset.publisherKey.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						"Publisher cannot be empty, object NimbusAdsManager not created");
					return;
				}

				#if NIMBUS_ENABLE_APS_ANDROID
					if (!ValidateApsData("Android", _androidApsAppKey, _asset.androidApsSlotData)) {
						return;
					}
					_asset.androidApsAppKey = _androidApsAppKey.stringValue;
				#endif
				#if NIMBUS_ENABLE_APS_IOS
					if (!ValidateApsData("iOS", _iosApsAppKey, _asset.iosApsSlotData)) {
						return;
					}
					_asset.iosApsAppKey = _iosApsAppKey.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_VUNGLE_ANDROID
					_asset.androidVungleAppID = _androidVungleAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_VUNGLE_IOS
					_asset.iosVungleAppID = _iosVungleAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_META_ANDROID
					_asset.androidMetaAppID = _androidMetaAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_META_IOS
					_asset.iosMetaAppID = _iosMetaAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_ADMOB_ANDROID
					if (!ValidateAdMobData("Android", _androidAdMobAppId, _asset.androidAdMobAdUnitData)) {
						return;
					}
					_asset.androidAdMobAppID = _androidAdMobAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_ADMOB_IOS
					if (!ValidateAdMobData("iOS", _iosAdMobAppId, _asset.iosAdMobAdUnitData)) {
						return;
					}
					_asset.iosAdMobAppID = _iosAdMobAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_MINTEGRAL_ANDROID
					_asset.androidMintegralAppID = _androidMintegralAppId.stringValue;
					_asset.androidMintegralAppKey = _androidMintegralAppKey.stringValue;
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL_IOS
					_asset.iosMintegralAppID = _iosMintegralAppId.stringValue;
					_asset.iosMintegralAppKey = _iosMintegralAppKey.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_UNITY_ADS_ANDROID
					_asset.androidUnityAdsGameID = _androidUnityAdsGameId.stringValue;
				#endif
				#if NIMBUS_ENABLE_UNITY_ADS_IOS
					_asset.iosUnityAdsGameID = _iosUnityAdsGameId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_MOLOCO_ANDROID
					_asset.androidMolocoAppKey = _androidMolocoAppKey.stringValue;
				#endif
				#if NIMBUS_ENABLE_MOLOCO_IOS
					_asset.iosMolocoAppKey = _iosMolocoAppKey.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_INMOBI_ANDROID
					_asset.androidInMobiAccountId = _androidInMobiAccountId.stringValue;
				#endif
				#if NIMBUS_ENABLE_INMOBI_IOS
					_asset.iosInMobiAccountId = _iosInMobiAccountId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_DIGITAL_TURBINE_ANDROID
					_asset.androidDigitalTurbineAppId = _androidDigitalTurbineAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_DIGITAL_TURBINE_IOS
					_asset.iosDigitalTurbineAppId = _iosDigitalTurbineAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_DISPLAY_IO_ANDROID
					_asset.androidDisplayIOAppId = _androidDisplayIOAppId.stringValue;
					_asset.androidDisplayIOUserId = _androidDisplayIOAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_DISPLAY_IO_IOS
					_asset.iosDisplayIOAppId = _iosDisplayIOAppId.stringValue;
					_asset.iosDisplayIOUserId = _iosDisplayIOUserId.stringValue;
				#endif

				AssetDatabase.CreateAsset(_asset,
					"Packages/com.adsbynimbus.nimbus/Runtime/Scripts/Nimbus.ScriptableObjects/NimbusSDKConfiguration.asset");
				AssetDatabase.SaveAssets();

				var go = new GameObject {
					name = "NimbusAdsManager"
				};
				var manager = go.AddComponent<NimbusManager>();
				manager.SetNimbusSDKConfiguration(_asset);

				Undo.RegisterCreatedObjectUndo(go, "NimbusManager created");
				Selection.activeObject = go;
				EditorUtility.FocusProjectWindow();
				Close();
			}
		}


		private void HandleApsSlots(SerializedProperty slotData, out apsAd[] platformSlots) {
			var apsSlotData = new List<apsAd>();
			for (var i = 0; i < slotData.arraySize; i++) {
				var item = slotData.GetArrayElementAtIndex(i);
				var slotId = item.FindPropertyRelative("slotId");

				var adUnitType = item.FindPropertyRelative("adUnitType");
				var apsAdUnitType = APSAdFormat.Display300X250;
				if (adUnitType != null) {
					apsAdUnitType = (APSAdFormat)adUnitType.enumValueIndex;
				}

				var apsData = new apsAd(slotId?.stringValue, apsAdUnitType);

				apsSlotData.Add(apsData);
			}
			platformSlots = apsSlotData.ToArray();
		}	

		private bool ValidateApsData(string platform, SerializedProperty appId, apsAd[] slotData) {

			var seenAdTypes = new Dictionary<APSAdFormat, bool>();
			foreach (var apsSlot in slotData) {
				if (apsSlot.slotId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, the APS slot id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(apsSlot.adUnitType)) {
					seenAdTypes.Add(apsSlot.adUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, APS cannot contain duplicate ad type {apsSlot.adUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}

		private void HandleAdMobAdUnitData(SerializedProperty adUnitData, out AdMobAdUnit[] adUnits)
		{
			var adUnitList = new List<AdMobAdUnit>();
			for (var i = 0; i < adUnitData.arraySize; i++) {
				var item = adUnitData.GetArrayElementAtIndex(i);
				var adUnitId = item.FindPropertyRelative("AdUnitId");

				var adMobData  = new AdMobAdUnit() {
					AdUnitId = adUnitId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("AdUnitType");
				if (adUnitType != null) {
					adMobData.AdUnitType = (AdType)adUnitType.enumValueIndex;
				}

				adUnitList.Add(adMobData);
			}
			adUnits = adUnitList.ToArray();
		}
		
		private bool ValidateAdMobData(string platform, SerializedProperty appId, AdMobAdUnit[] adUnitData) {

			var seenAdTypes = new Dictionary<AdType, bool>();
			foreach (var adUnit in adUnitData) {
				if (adUnit.AdUnitId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"AdMob SDK has been included, the Ad Unit id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(adUnit.AdUnitType)) {
					seenAdTypes.Add(adUnit.AdUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"AdMob SDK has been included, AdMob cannot contain duplicate ad type {adUnit.AdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
	}
}
#endif