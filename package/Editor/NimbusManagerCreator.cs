#if UNITY_EDITOR
using Nimbus.Runtime.Scripts;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Nimbus.Editor {
	public class NimbusManagerCreator : EditorWindow {
		private string _apiKey;
		private string _publisherKey;
		private bool _enableUnityLogs = true;
		private bool _enableSDKInTestMode;
		
		[MenuItem("Nimbus/Create New NimbusAdsManager")]
		public static void CreateNewNimbusGameManager() {
			GetWindow<NimbusManagerCreator>("NimbusAdsManager Creator");
		}
		
		private void OnGUI() {
			_publisherKey = EditorGUILayout.TextField("Publisher Key", _publisherKey);
			_apiKey = EditorGUILayout.TextField("ApiKey", _apiKey);
			_enableUnityLogs = EditorGUILayout.Toggle("Enable Unity Logger", _enableUnityLogs);
			_enableSDKInTestMode = EditorGUILayout.Toggle("Enable SDK In Test Mode", _enableSDKInTestMode);
			

			if (GUILayout.Button("Create")) {
				var asset = CreateInstance<NimbusSDKConfiguration>();
				asset.publisherKey = _publisherKey.Trim();
				asset.apiKey = _apiKey.Trim();
				asset.enableUnityLogs = _enableUnityLogs;
				asset.enableSDKInTestMode = _enableSDKInTestMode;
				
				if (asset.apiKey.IsNullOrEmpty()) {
					Debug.LogError("Apikey cannot be empty, object not NimbusAdsManager created");
					return;
				}
				
				if (asset.publisherKey.IsNullOrEmpty()) {
					Debug.LogError("Publisher cannot be empty, object not NimbusAdsManager created");
					return;
				}
				AssetDatabase.CreateAsset(asset, "Packages/com.adsbynimbus.unity/Runtime/Scripts/Nimbus.ScriptableObjects/NimbusSDKConfiguration.asset");
				AssetDatabase.SaveAssets();
				
				var go = new GameObject {
					name = "NimbusAdsManager"
				};
				var manager = go.AddComponent<NimbusManager>();
				manager.SetNimbusSDKConfiguration(asset);
				
				Undo.RegisterCreatedObjectUndo(go, "NimbusManager created");
				Selection.activeObject = go;
				EditorUtility.FocusProjectWindow();
				Close();
			}
		}
	}
}
#endif