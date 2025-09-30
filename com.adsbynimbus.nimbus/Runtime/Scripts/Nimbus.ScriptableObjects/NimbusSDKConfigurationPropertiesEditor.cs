#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Nimbus.ScriptableObjects {
	[CustomEditor(typeof(NimbusSDKConfiguration))]
	public class NimbusSDKConfigurationPropertiesEditor : Editor {
		// Initialization
		private SerializedProperty _publisherKey;
		private SerializedProperty _apiKey;
		private SerializedProperty _enableSDKInTestMode;
		private SerializedProperty _enableUnityLogs;
		private SerializedProperty _enableManualInitialization;
		
		// APS
		private SerializedProperty _androidAppId;
		private ReorderableList _androidApsSlotIdList = null;
		private SerializedProperty _androidApsSlots = null;
		private SerializedProperty _androidApsTimeoutInMilliseconds;
		
		private SerializedProperty _iosAppId;
		private ReorderableList _iosApsSlotIdList = null;
		private SerializedProperty _iosApsSlots = null;
		private SerializedProperty _iosApsTimeoutInMilliseconds;
		
		// Vungle
		private SerializedProperty _androidVungleAppId;
		
		private SerializedProperty _iosVungleAppId;
		
		// Meta
		private SerializedProperty _androidMetaAppId;

		private SerializedProperty _iosMetaAppId;
		
		// AdMob
		private SerializedProperty _androidAdMobAppId;
		private ReorderableList _androidAdMobAdUnitDataList = null;
		private SerializedProperty _androidAdMobAdUnitData = null;
		
		private SerializedProperty _iosAdMobAppId;
		private ReorderableList _iosAdMobAdUnitDataList = null;
		private SerializedProperty _iosAdMobAdUnitData = null;
		
		// Mintegral
		private SerializedProperty _androidMintegralAppId;
		private SerializedProperty _androidMintegralAppKey;
		private ReorderableList _androidMintegralAdUnitDataList = null;
		private SerializedProperty _androidMintegralAdUnitData = null;
		
		private SerializedProperty _iosMintegralAppId;
		private SerializedProperty _iosMintegralAppKey;
		private ReorderableList _iosMintegralAdUnitDataList = null;
		private SerializedProperty _iosMintegralAdUnitData = null;

		// Unity Ads
		private SerializedProperty _androidUnityAdsGameId;

		private SerializedProperty _iosUnityAdsGameId;
		
		// Moloco
		private SerializedProperty _androidMolocoAppKey;
		private ReorderableList _androidMolocoAdUnitDataList = null;
		private SerializedProperty _androidMolocoAdUnitData = null;
		
		private SerializedProperty _iosMolocoAppKey;
		private ReorderableList _iosMolocoAdUnitDataList = null;
		private SerializedProperty _iosMolocoAdUnitData = null;
		
		// InMobi
		private SerializedProperty _androidInMobiAccountId;
		private ReorderableList _androidInMobiAdUnitDataList = null;
		private SerializedProperty _androidInMobiAdUnitData = null;
		
		private SerializedProperty _iosInMobiAccountId;
		private ReorderableList _iosInMobiAdUnitDataList = null;
		private SerializedProperty _iosInMobiAdUnitData = null;

		// Needed so error messages aren't spammed
		private bool _errorLogged;
		
		private void OnEnable() {
			_publisherKey = serializedObject.FindProperty("publisherKey");
			_apiKey = serializedObject.FindProperty("apiKey");
			_enableSDKInTestMode = serializedObject.FindProperty("enableSDKInTestMode");
			_enableUnityLogs = serializedObject.FindProperty("enableUnityLogs");
			_enableManualInitialization = serializedObject.FindProperty("enableManualInitialization");
			
			// APS
			// Android APS UI
			_androidAppId = serializedObject.FindProperty("androidAppID");
			_androidApsSlots = serializedObject.FindProperty("androidApsSlotData");
			_androidApsTimeoutInMilliseconds = serializedObject.FindProperty("androidApsTimeoutInMilliseconds");
			_androidApsTimeoutInMilliseconds.intValue = serializedObject.FindProperty("androidApsTimeoutInMilliseconds").intValue 
			                                            == 0 ? NimbusSDKConfiguration.ApsDefaultTimeout : serializedObject.FindProperty("androidApsTimeoutInMilliseconds").intValue;
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
			_iosAppId = serializedObject.FindProperty("iosAppID");
			_iosApsSlots = serializedObject.FindProperty("iosApsSlotData");
			_iosApsTimeoutInMilliseconds = serializedObject.FindProperty("iosApsTimeoutInMilliseconds");
			_iosApsTimeoutInMilliseconds.intValue = serializedObject.FindProperty("iosApsTimeoutInMilliseconds").intValue 
			                                        == 0 ? NimbusSDKConfiguration.ApsDefaultTimeout : serializedObject.FindProperty("iosApsTimeoutInMilliseconds").intValue;
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
			
			// Mintegral
			// Android Mintegral UI
			_androidMintegralAppId = serializedObject.FindProperty("androidMintegralAppID");
			_androidMintegralAppKey = serializedObject.FindProperty("androidMintegralAppKey");
			_androidMintegralAdUnitData = serializedObject.FindProperty("androidMintegralAdUnitData");
			_androidMintegralAdUnitDataList = new ReorderableList(
				serializedObject, _androidMintegralAdUnitData,
				true,
				false,
				true,
				true
			);
			_androidMintegralAdUnitData.isExpanded = true;
			_androidMintegralAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_androidMintegralAdUnitDataList.headerHeight = 0f;
			_androidMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralAndroidAdUnitData;
			
			// IOS Mintegral UI
			_iosMintegralAppId = serializedObject.FindProperty("iosMintegralAppID");
			_iosMintegralAppKey = serializedObject.FindProperty("iosMintegralAppKey");
			_iosMintegralAdUnitData = serializedObject.FindProperty("iosMintegralAdUnitData");
			_iosMintegralAdUnitDataList = new ReorderableList(
				serializedObject, _iosMintegralAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosMintegralAdUnitData.isExpanded = true;
			_iosMintegralAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosMintegralAdUnitDataList.headerHeight = 0f;
			_iosMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralIOSAdUnitData;
			
			// Unity Ads
			// Android Unity Ads UI
			_androidUnityAdsGameId = serializedObject.FindProperty("androidUnityAdsGameID");
			
			// IOS Unity Ads UI
			_iosUnityAdsGameId = serializedObject.FindProperty("iosUnityAdsGameID");
			
			// Moloco
			// Android Moloco UI
			_androidMolocoAppKey = serializedObject.FindProperty("androidMolocoAppKey");
			_androidMolocoAdUnitData = serializedObject.FindProperty("androidMolocoAdUnitData");
			_androidMolocoAdUnitDataList = new ReorderableList(
				serializedObject, _androidMolocoAdUnitData,
				true,
				false,
				true,
				true
			);
			_androidMolocoAdUnitData.isExpanded = true;
			_androidMolocoAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_androidMolocoAdUnitDataList.headerHeight = 0f;
			_androidMolocoAdUnitDataList.drawElementCallback += OnDrawElementMolocoAndroidAdUnitData;
			
			// IOS Moloco UI
			_iosMolocoAppKey = serializedObject.FindProperty("iosMolocoAppKey");
			_iosMolocoAdUnitData = serializedObject.FindProperty("iosMolocoAdUnitData");
			_iosMolocoAdUnitDataList = new ReorderableList(
				serializedObject, _iosMolocoAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosMolocoAdUnitData.isExpanded = true;
			_iosMolocoAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosMolocoAdUnitDataList.headerHeight = 0f;
			_iosMolocoAdUnitDataList.drawElementCallback += OnDrawElementMolocoIOSAdUnitData;
			
			//InMobi
			// Android InMobi UI
			_androidInMobiAccountId = serializedObject.FindProperty("androidInMobiAccountId");
			_androidInMobiAdUnitData = serializedObject.FindProperty("androidInMobiAdUnitData");
			_androidInMobiAdUnitDataList = new ReorderableList(
				serializedObject, _androidInMobiAdUnitData,
				true,
				false,
				true,
				true
			);
			_androidInMobiAdUnitData.isExpanded = true;
			_androidInMobiAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_androidInMobiAdUnitDataList.headerHeight = 0f;
			_androidInMobiAdUnitDataList.drawElementCallback += OnDrawElementInMobiAndroidAdUnitData;
			
			// IOS InMobi UI
			_iosInMobiAccountId = serializedObject.FindProperty("iosInMobiAccountId");
			_iosInMobiAdUnitData = serializedObject.FindProperty("iosInMobiAdUnitData");
			_iosInMobiAdUnitDataList = new ReorderableList(
				serializedObject, _iosInMobiAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosInMobiAdUnitData.isExpanded = true;
			_iosInMobiAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosInMobiAdUnitDataList.headerHeight = 0f;
			_iosInMobiAdUnitDataList.drawElementCallback += OnDrawElementInMobiIOSAdUnitData;
		}

		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementApsAndroidSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementApsIOSSlotData;
			_androidAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAndroidAdUnitData;
			_iosAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobIOSAdUnitData;
			_androidMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAndroidAdUnitData;
			_iosMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralIOSAdUnitData;
			_androidMolocoAdUnitDataList.drawElementCallback -= OnDrawElementMolocoAndroidAdUnitData;
			_iosMolocoAdUnitDataList.drawElementCallback -= OnDrawElementMolocoIOSAdUnitData;
			_androidInMobiAdUnitDataList.drawElementCallback -= OnDrawElementInMobiAndroidAdUnitData;
			_iosInMobiAdUnitDataList.drawElementCallback -= OnDrawElementInMobiIOSAdUnitData;
			var config = target as NimbusSDKConfiguration;
			if (config == null) return;
			config.Sanitize();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
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
		
		private void OnDrawElementMintegralAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidMintegralAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMintegralIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosMintegralAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMolocoAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidMolocoAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMolocoIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosMolocoAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementInMobiAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidInMobiAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementInMobiIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosInMobiAdUnitData.GetArrayElementAtIndex(index);
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

		public override void OnInspectorGUI() {
			serializedObject.Update();
			var config = target as NimbusSDKConfiguration;
			if (config == null) return;
			EditorUtility.SetDirty(target);

			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;

			EditorDrawUtility.DrawScriptableObjectField((NimbusSDKConfiguration)target);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);

			EditorGUILayout.LabelField("Publisher Credentials", headerStyle);
			EditorGUILayout.PropertyField(_publisherKey, true);
			EditorGUILayout.PropertyField(_apiKey, true);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("SDK Flags", headerStyle);
			EditorGUILayout.PropertyField((_enableSDKInTestMode));
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Enable Unity Logs", headerStyle);
			EditorGUILayout.PropertyField((_enableUnityLogs));
			GUILayout.Space(10);
			
			EditorGUILayout.LabelField("Enable Manual Initialization", headerStyle);
			EditorGUILayout.PropertyField(_enableManualInitialization);
			GUILayout.Space(10);
			
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS || NIMBUS_ENABLE_MOBILEFUSE || NIMBUS_ENABLE_LIVERAMP || NIMBUS_ENABLE_MOLOCO || NIMBUS_ENABLE_INMOBI
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
					ValidateApsSlots("Android", _androidApsSlots);
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidAppId));
					_androidApsTimeoutInMilliseconds.intValue = EditorGUILayout.IntField("Timeout in Milliseconds", value: _androidApsTimeoutInMilliseconds.intValue);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidApsSlots, "APS Android Slot Id Data");
				#endif
			
				#if NIMBUS_ENABLE_APS_IOS
					ValidateApsSlots("iOS", _androidApsSlots);
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_iosAppId);
					_iosApsTimeoutInMilliseconds.intValue = EditorGUILayout.IntField("Timeout in Milliseconds", value: _iosApsTimeoutInMilliseconds.intValue);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_iosApsSlots, "APS iOS Slot Id Data");
				#endif	

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter APS data", MessageType.Warning);
				#endif
				GUILayout.Space(10);
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
				GUILayout.Space(10);
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
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_androidMintegralAdUnitData, "Mintegral Android Ad Unit Id Data");
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMintegralAppId));
					EditorGUILayout.PropertyField((_iosMintegralAppKey));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosMintegralAdUnitData, "Mintegral iOS Ad Unit Id Data");
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
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_androidMolocoAdUnitData, "Moloco Android Ad Unit Id Data");
				#endif
				#if NIMBUS_ENABLE_MOLOCO_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMolocoAppKey));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosMolocoAdUnitData, "Moloco iOS Ad Unit Id Data");
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
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_androidInMobiAdUnitData, "InMobi Android Ad Unit Id Data");
				#endif
				#if NIMBUS_ENABLE_INMOBI_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosInMobiAccountId));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosInMobiAdUnitData, "InMobi iOS Ad Unit Id Data");
				#endif
				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter InMobi data", MessageType.Warning);
				#endif
			#endif
			
			serializedObject.ApplyModifiedProperties();
		}
		private void ValidateApsSlots(string platform, SerializedProperty slotData) {
			var apsSlotData = new List<ApsSlotData>();
			for (var i = 0; i < slotData.arraySize; i++) {
				var item = slotData.GetArrayElementAtIndex(i);
				var slotId = item.FindPropertyRelative("SlotId");

				var apsData = new ApsSlotData {
					SlotId = slotId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("APSAdUnitType");
				if (adUnitType != null) {
					apsData.APSAdUnitType = (APSAdUnitType)adUnitType.enumValueIndex;
				}

				apsSlotData.Add(apsData);
			}
			var platformSlots = apsSlotData.ToArray();
			var seenAdTypes = new Dictionary<APSAdUnitType, bool>();
			foreach (var apsSlot in platformSlots) {
				if (!seenAdTypes.ContainsKey(apsSlot.APSAdUnitType)) {
					seenAdTypes.Add(apsSlot.APSAdUnitType, true);
				}
				else {
					if (!_errorLogged)
					{
						Debug.unityLogger.LogError("Nimbus", 
							$"APS SDK has been included, APS cannot contain duplicate ad type {apsSlot.APSAdUnitType} for {platform}, object NimbusAdsManager not created");
						_errorLogged = true;
					}
					return;
				}
			}
			_errorLogged = false;
		}
	}
}
#endif