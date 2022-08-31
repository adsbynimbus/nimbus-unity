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
		private SerializedProperty _enableAps;
		private SerializedProperty _appId;
		private ReorderableList _apsSlotIdList = null;
		private SerializedProperty _apsSlots = null;

		private void OnEnable() {
			_publisherKey = serializedObject.FindProperty("publisherKey");
			_apiKey = serializedObject.FindProperty("apiKey");
			_enableSDKInTestMode = serializedObject.FindProperty("enableSDKInTestMode");
			_enableUnityLogs = serializedObject.FindProperty("enableUnityLogs");


			_enableAps = serializedObject.FindProperty("enableAPS");
			_appId = serializedObject.FindProperty("appID");
			_apsSlots = serializedObject.FindProperty("slotData");
			_apsSlotIdList = new ReorderableList(
				serializedObject, _apsSlots,
				true,
				false,
				true,
				true
			);
			_apsSlots.isExpanded = true;
			_apsSlotIdList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_apsSlotIdList.headerHeight = 0f;
			_apsSlotIdList.drawElementCallback += OnDrawElementApsSlotData;
		}

		private void OnDisable() {
			_apsSlotIdList.drawElementCallback -= OnDrawElementApsSlotData;

			var config = target as NimbusSDKConfiguration;
			if (config == null) return;
			config.Sanitize();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		private void OnDrawElementApsSlotData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;

			var item = _apsSlots.GetArrayElementAtIndex(index);
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
			EditorGUILayout.LabelField("Third Party SDK Support", headerStyle);
			EditorGUILayout.PropertyField((_enableAps));
			if (_enableAps.boolValue) {
				EditorGUILayout.PropertyField((_appId));
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				EditorDrawUtility.DrawArray(_apsSlots, "Slot Id Data");
			}

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif