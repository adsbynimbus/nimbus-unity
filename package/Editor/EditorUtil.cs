#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Nimbus.Editor {
	public static class EditorUtil {
		public static void LogWithHelpBox(string message, MessageType type) {
			GUILayout.Space(5);
			EditorGUILayout.HelpBox(message, type);
			switch (type) {
				case MessageType.None:
				case MessageType.Info:
					Debug.unityLogger.Log("Nimbus", message);
					break;
				case MessageType.Warning:
					Debug.unityLogger.LogWarning("Nimbus", message);
					break;
				case MessageType.Error:
					Debug.unityLogger.LogError("Nimbus", message);
					break;
				default:
					Debug.unityLogger.Log("Nimbus", message);
					break;
			}
		}
		
		internal struct HelpBoxDataContainer {
			public readonly string message;
			public readonly MessageType type;
			public HelpBoxDataContainer(string message, MessageType type) {
				this.message = message;
				this.type = type;
			}
		}
	}
}
#endif