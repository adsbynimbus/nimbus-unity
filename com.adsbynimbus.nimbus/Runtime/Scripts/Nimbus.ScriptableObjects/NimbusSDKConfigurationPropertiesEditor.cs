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
			_androidApsSlotIdList.drawElementCallback += OnDrawElementApsSlotData;
			
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
			_iosApsSlotIdList.drawElementCallback += OnDrawElementApsSlotData;
			
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
			_androidAdMobAdUnitDataList.drawElementCallback += OnDrawElementAdMobAdUnitData;
			
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
			_iosAdMobAdUnitDataList.drawElementCallback += OnDrawElementAdMobAdUnitData;
			
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
			_androidMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralAdUnitData;
			
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
			_iosMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralAdUnitData;
		}

		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementApsSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementApsSlotData;
			_androidAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAdUnitData;
			_iosAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAdUnitData;
			_androidMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAdUnitData;
			_iosMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAdUnitData;
			var config = target as NimbusSDKConfiguration;
			if (config == null) return;
			config.Sanitize();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		private void OnDrawElementApsSlotData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			#if UNITY_ANDROID
				var item = _androidApsSlots.GetArrayElementAtIndex(index);
			#endif
			#if UNITY_IOS
				var item = _iosApsSlots.GetArrayElementAtIndex(index);
			#endif
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
		
		private void OnDrawElementAdMobAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			#if UNITY_ANDROID
				var item = _androidAdMobAdUnitData.GetArrayElementAtIndex(index);
			#endif
			#if UNITY_IOS
				var item = _iosAdMobAdUnitData.GetArrayElementAtIndex(index);
			#endif
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
		
		private void OnDrawElementMintegralAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			#if UNITY_ANDROID
				var item = _androidMintegralAdUnitData.GetArrayElementAtIndex(index);
			#endif
			#if UNITY_IOS
				var item = _iosMintegralAdUnitData.GetArrayElementAtIndex(index);
			#endif
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
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL
				EditorGUILayout.LabelField("Third Party SDK Support", headerStyle);
			#endif
			
			#if NIMBUS_ENABLE_APS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("APS Configuration", headerStyle);
				#if UNITY_ANDROID
					EditorGUILayout.PropertyField((_androidAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidApsSlots, "Android Slot Id Data");
				#endif

				#if UNITY_IOS
					EditorGUILayout.PropertyField((_iosAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_iosApsSlots, "iOS Slot Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter APS data", MessageType.Warning);
				#endif
				GUILayout.Space(10);
			#endif
			
			#if NIMBUS_ENABLE_VUNGLE
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Vungle Configuration", headerStyle);
				#if UNITY_ANDROID
					EditorGUILayout.PropertyField((_androidVungleAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if UNITY_IOS
					EditorGUILayout.PropertyField((_iosVungleAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Vungle data", MessageType.Warning);
				#endif
				GUILayout.Space(10);
			#endif
			
			#if NIMBUS_ENABLE_META
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Meta Configuration", headerStyle);
				#if UNITY_ANDROID
					EditorGUILayout.PropertyField((_androidMetaAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if UNITY_IOS
					EditorGUILayout.PropertyField((_iosMetaAppId));
					GUILayout.Space(10);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Meta data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_ADMOB
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("AdMob Configuration", headerStyle);
				#if UNITY_ANDROID
					EditorGUILayout.PropertyField((_androidAdMobAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidAdMobAdUnitData, "Android Ad Unit Id Data");
				#endif
				#if UNITY_IOS
					EditorGUILayout.PropertyField((_iosAdMobAppId));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosAdMobAdUnitData, "iOS Ad Unit Id Data");
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter AdMob data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MINTEGRAL
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Mintegral Configuration", headerStyle);
				#if UNITY_ANDROID
					EditorGUILayout.PropertyField((_androidMintegralAppId));
					EditorGUILayout.PropertyField((_androidMintegralAppKey));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidMintegralAdUnitData, "Android Ad Unit Id Data");
				#endif
				#if UNITY_IOS
					EditorGUILayout.PropertyField((_iosMintegralAppId));
					EditorGUILayout.PropertyField((_iosMintegralAppKey));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosMintegralAdUnitData, "iOS Ad Unit Id Data");
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Mintegral data", MessageType.Warning);
				#endif
			#endif
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif