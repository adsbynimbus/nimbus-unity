# if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Nimbus.ScriptableObjects {
	public static class EditorDrawUtility {
		public static void DrawArray(SerializedProperty rootProperty, string buttonName = null,
			string namePropertyString = null, bool showAddElement = true, bool removeIconToTheRight = true) {
			if (!rootProperty.isArray) {
				return;
			}


			for (var i = 0; i < rootProperty.arraySize; i++) {
				var item = rootProperty.GetArrayElementAtIndex(i);
				item.isExpanded = true;
				GUILayout.BeginVertical(EditorStyles.helpBox);
				DrawArrayElement(item, namePropertyString);
				GUILayout.Space(20);

				if (GUILayout.Button("Remove element", EditorStyles.miniButton)) {
					rootProperty.DeleteArrayElementAtIndex(i);
					break;
				}

				GUILayout.EndVertical();
				GUILayout.Space(20);
			}

			if (showAddElement) {
				if (GUILayout.Button("Add " + buttonName)) {
					rootProperty.arraySize++;
				}
			}
		}

		public static void DrawArrayElement(SerializedProperty property, string namePropertyString = null,
			bool skipFirstChild = false) {
			EditorGUI.indentLevel++;
			SerializedProperty nameProperty = null;
			if (namePropertyString != null) {
				nameProperty = property.FindPropertyRelative(namePropertyString);
				if (nameProperty != null) {
					var name = nameProperty.stringValue;
					EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
				}

				EditorGUI.indentLevel++;
			}


			var itr = property.Copy();
			var enterChildren = true;

			while (itr.Next(enterChildren)) {
				if (SerializedProperty.EqualContents(itr, property.GetEndProperty(true))) {
					break;
				}

				if (enterChildren && skipFirstChild) {
					enterChildren = false;
					continue;
				}

				EditorGUILayout.PropertyField(itr, enterChildren);
				enterChildren = false;
			}

			EditorGUI.indentLevel = nameProperty != null ? EditorGUI.indentLevel - 2 : EditorGUI.indentLevel - 1;
		}

		public static void DrawArrayElement(Rect rect, SerializedProperty property, string namePropertyString = null,
			bool skipFirstChild = false) {
			if (property.isArray) {
				return;
			}

			EditorGUI.indentLevel++;
			SerializedProperty nameProperty = null;

			if (namePropertyString != null) {
				nameProperty = property.FindPropertyRelative(namePropertyString);
				if (nameProperty != null) {
					var name = nameProperty.stringValue;
					EditorGUI.LabelField(rect, name, EditorStyles.boldLabel);
					rect.y += rect.height;
				}

				EditorGUI.indentLevel++;
			}

			var itr = property.Copy();
			var enterChildren = true;

			while (itr.Next(enterChildren)) {
				if (SerializedProperty.EqualContents(itr, property.GetEndProperty())) {
					break;
				}

				if (enterChildren && skipFirstChild) {
					enterChildren = false;
					continue;
				}

				EditorGUI.PropertyField(rect, itr, enterChildren);
				rect.y += rect.height;
				enterChildren = false;
			}

			EditorGUI.indentLevel = nameProperty != null ? EditorGUI.indentLevel - 2 : EditorGUI.indentLevel - 1;
		}

		public static void DrawEditorLayoutHorizontalLine(Color color, int thickness = 1, int padding = 10) {
			var rect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			rect.height = thickness;
			rect.y += padding / 2f;
			EditorGUI.DrawRect(rect, color);
		}

		public static void DrawScriptableObjectField<T>(T target) where T : ScriptableObject {
			GUI.enabled = false;
			EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(target), typeof(T), false);
			GUI.enabled = true;
		}
	}
}
#endif