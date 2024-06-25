#if UNITY_EDITOR
using Nimbus.Internal.Interceptor.ThirdPartyDemand;
using Nimbus.Runtime.Scripts;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

#region ThirdPartySDKImports

using System.Collections.Generic;
using Nimbus.Internal;

#endregion


namespace Nimbus.Editor {
	public class NimbusManagerCreator : EditorWindow {
		private string _apiKey;
		private string _publisherKey;
		private bool _enableUnityLogs = true;
		private bool _enableSDKInTestMode;
		private NimbusSDKConfiguration _asset = null;

		// APS
		private SerializedProperty _androidAppId;
		private ReorderableList _androidApsSlotIdList = null;
		private SerializedProperty _androidApsSlots = null;

		private SerializedProperty _iosAppId;
		private ReorderableList _iosApsSlotIdList = null;
		private SerializedProperty _iosApsSlots = null;


		[MenuItem("Nimbus/Create New NimbusAdsManager")]
		public static void CreateNewNimbusGameManager() {
			GetWindow<NimbusManagerCreator>("NimbusAdsManager Creator");
		}

		private void OnEnable() {
			_asset = CreateInstance<NimbusSDKConfiguration>();
			var serializedObject = new SerializedObject(_asset);

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


		private void OnGUI() {
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			_publisherKey = EditorGUILayout.TextField("Publisher Key", _publisherKey);
			_apiKey = EditorGUILayout.TextField("ApiKey", _apiKey);
			_enableUnityLogs = EditorGUILayout.Toggle("Enable Unity Logger", _enableUnityLogs);
			_enableSDKInTestMode = EditorGUILayout.Toggle("Enable SDK In Test Mode", _enableSDKInTestMode);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);

			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;

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

			// ReSharper disable InvertIf
			if (GUILayout.Button("Create")) {
				_asset.publisherKey = _publisherKey;
				_asset.apiKey = _apiKey;
				_asset.enableUnityLogs = _enableUnityLogs;
				_asset.enableSDKInTestMode = _enableSDKInTestMode;

				#if NIMBUS_ENABLE_APS
					HandleApsSlots();
				#endif
				
				_asset.Sanitize();
				if (_asset.apiKey.IsNullOrEmpty()) {
					Debug.LogError("Apikey cannot be empty, object NimbusAdsManager not created");
					return;
				}

				if (_asset.publisherKey.IsNullOrEmpty()) {
					Debug.LogError("Publisher cannot be empty, object NimbusAdsManager not created");
					return;
				}

				#if NIMBUS_ENABLE_APS
					if (!ValidateApsData()) {
						return;
					}
				#endif

				AssetDatabase.CreateAsset(_asset,
					"Packages/com.adsbynimbus.nimbus/Runtime/Scripts/Nimbus.ScriptableObjects/NimbusSDKConfiguration.asset");
				AssetDatabase.SaveAssets();

				var go = new GameObject {
					name = "NimbusAdsManager"
				};
				var manager = go.AddComponent<NimbusManager>();
				manager.SetNimbusSDKConfiguration(_asset);

				Undo.RegisterCreatedObjectUndo(go, "NimbusManager created");
				Selection.activeObject = go;
				EditorUtility.FocusProjectWindow();
				Close();
			}
		}


			private void HandleApsSlots() {
				SerializedProperty slotData = null;
				#if UNITY_ANDROID
					slotData = _androidApsSlots;
				# elif UNITY_IOS
					slotData = _iosApsSlots;
				#endif
				var apsSlotData = new List<ApsSlotData>();
				for (var i = 0; i < slotData.arraySize; i++) {
					var item = slotData.GetArrayElementAtIndex(i);
					var slotId = item.FindPropertyRelative("SlotId");

					var apsData = new ApsSlotData {
						SlotId = slotId?.stringValue
					};

					var adUnitType = item.FindPropertyRelative("AdUnitType");
					if (adUnitType != null) {
						apsData.AdUnitType = (AdUnitType)adUnitType.enumValueIndex;
					}

					apsSlotData.Add(apsData);
				}	

				#if UNITY_ANDROID
					_asset.androidApsSlotData = apsSlotData.ToArray();
				# elif UNITY_IOS
					_asset.iosApsSlotData = apsSlotData.ToArray();
				#endif
		}	

		private bool ValidateApsData() {
			string appId = null;
			#if UNITY_ANDROID
				appId = _androidAppId.stringValue;
			#elif UNITY_IOS
				appId = _iosAppId.stringValue;
			#endif
			
			if (appId.IsNullOrEmpty()) {
				Debug.LogError(
					"APS SDK has been included, the APS App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			ApsSlotData[] slotData = null;

			// ReSharper disable once ConvertToConstant.Local
			var platform = "Android";
			#if UNITY_ANDROID
				slotData = _asset.androidApsSlotData;
			# elif UNITY_IOS
				slotData = _asset.iosApsSlotData;
				platform = "iOS";
			#endif
			
			if (slotData == null || slotData.Length == 0) {
				Debug.LogError(
					$"APS SDK has been included, APS placement slots for {platform} need to be entered, object NimbusAdsManager not created");
				return false;
			}

			var seenAdTypes = new Dictionary<AdUnitType, bool>();
			foreach (var apsSlot in slotData) {
				if (apsSlot.SlotId.IsNullOrEmpty()) {
					Debug.LogError(
						$"APS SDK has been included, the APS slot id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (apsSlot.AdUnitType == AdUnitType.Undefined) {
					Debug.LogError(
						$"APS SDK has been included, Ad Unit type for {platform} cannot be Undefined, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(apsSlot.AdUnitType)) {
					seenAdTypes.Add(apsSlot.AdUnitType, true);
				}
				else {
					Debug.LogError(
						$"APS SDK has been included, APS cannot contain duplicate ad type {apsSlot.AdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
	}
}
#endif