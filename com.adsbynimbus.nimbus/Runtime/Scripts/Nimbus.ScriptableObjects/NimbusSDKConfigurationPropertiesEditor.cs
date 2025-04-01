#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
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
		
		// APS
		private SerializedProperty _androidAppId;
		private ReorderableList _androidApsSlotIdList = null;
		private SerializedProperty _androidApsSlots = null;
		
		private SerializedProperty _iosAppId;
		private ReorderableList _iosApsSlotIdList = null;
		private SerializedProperty _iosApsSlots = null;
		
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

		private void OnEnable() {
			_publisherKey = serializedObject.FindProperty("publisherKey");
			_apiKey = serializedObject.FindProperty("apiKey");
			_enableSDKInTestMode = serializedObject.FindProperty("enableSDKInTestMode");
			_enableUnityLogs = serializedObject.FindProperty("enableUnityLogs");
			
			// APS
			// Android APS UI
			_androidAppId = serializedObject.FindProperty("androidAppID");
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
			_iosAppId = serializedObject.FindProperty("iosAppID");
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
		}

		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementApsAndroidSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementApsIOSSlotData;
			_androidAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAndroidAdUnitData;
			_iosAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobIOSAdUnitData;
			_androidMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAndroidAdUnitData;
			_iosMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralIOSAdUnitData;
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
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS
				EditorGUILayout.LabelField("Third Party SDK Support", headerStyle);
			#endif
			
			#if NIMBUS_ENABLE_APS_ANDROID || NIMBUS_ENABLE_APS_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("APS Configuration", headerStyle);
				#if NIMBUS_ENABLE_APS_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidApsSlots, "APS Android Slot Id Data");
				#endif
			
				#if NIMBUS_ENABLE_APS_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosAppId));
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
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif