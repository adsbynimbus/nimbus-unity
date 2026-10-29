#if UNITY_EDITOR
using ScriptableObjects;
using UnityEditor;

namespace AdsByNimbus.Editor {
	public class NimbusSDKConfigCreator : EditorWindow {
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