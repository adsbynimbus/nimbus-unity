#if UNITY_EDITOR
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
			_androidApsSlotIdList.drawElementCallback += OnDrawElementAndroidApsSlotData;
			
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
			_iosApsSlotIdList.drawElementCallback += OnDrawElementIosApsSlotData;
		}

		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementAndroidApsSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementIosApsSlotData;

			var config = target as NimbusSDKConfiguration;
			if (config == null) return;
			config.Sanitize();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		private void OnDrawElementAndroidApsSlotData(Rect rect, int index, bool isActive, bool isFocused) {
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
		
		private void OnDrawElementIosApsSlotData(Rect rect, int index, bool isActive, bool isFocused) {
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

			#if NIMBUS_ENABLE_APS
				EditorGUILayout.LabelField("Third Party SDK Support", headerStyle);
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
			#endif			
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif