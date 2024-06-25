#if UNITY_EDITOR
using Nimbus.ScriptableObjects;
using UnityEditor;

namespace Nimbus.Editor {
	public class Nimbus : EditorWindow {
		[MenuItem("Nimbus/Create Empty SDK Configuration")]
		public static void CreateSDKConfiguration() {
			var asset = CreateInstance<NimbusSDKConfiguration>();
			AssetDatabase.CreateAsset(asset, "Packages/com.adsbynimbus.nimbus/Runtime/Scripts/Nimbus.ScriptableObjects/EmptyNimbusSDKConfiguration.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}
#endif