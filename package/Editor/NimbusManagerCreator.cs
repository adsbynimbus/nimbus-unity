#if UNITY_EDITOR
using System.Collections.Generic;
using Nimbus.Internal;
using Nimbus.Internal.ThirdPartyDemandProviders;
using Nimbus.Runtime.Scripts;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nimbus.Editor {
	public class NimbusManagerCreator : EditorWindow {
		private string _apiKey;
		private string _publisherKey;
		private bool _enableUnityLogs = true;
		private bool _enableSDKInTestMode;

		private bool _enableAps;
		private string _appId;

		private ReorderableList _apsSlotIdList = null;
		private SerializedProperty _apsSlots = null;
		private NimbusSDKConfiguration _asset = null;

		[MenuItem("Nimbus/Create New NimbusAdsManager")]
		public static void CreateNewNimbusGameManager() {
			GetWindow<NimbusManagerCreator>("NimbusAdsManager Creator");
		}

		private void OnEnable() {
			_asset = CreateInstance<NimbusSDKConfiguration>();
			var serializedObject = new SerializedObject(_asset);
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


		private void OnGUI() {
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			_publisherKey = EditorGUILayout.TextField("Publisher Key", _publisherKey);
			_apiKey = EditorGUILayout.TextField("ApiKey", _apiKey);
			_enableUnityLogs = EditorGUILayout.Toggle("Enable Unity Logger", _enableUnityLogs);
			_enableSDKInTestMode = EditorGUILayout.Toggle("Enable SDK In Test Mode", _enableSDKInTestMode);

			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			_enableAps = EditorGUILayout.Toggle("Enable APS", _enableAps);
			if (_enableAps) {
				_appId = EditorGUILayout.TextField("Enter APS App ID", _appId);
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				EditorDrawUtility.DrawArray(_apsSlots, "Slot Id Data");
				GUILayout.Space(15);
			}

			// ReSharper disable InvertIf
			if (GUILayout.Button("Create")) {
				_asset.publisherKey = _publisherKey;
				_asset.apiKey = _apiKey;
				_asset.enableUnityLogs = _enableUnityLogs;
				_asset.enableSDKInTestMode = _enableSDKInTestMode;
				_asset.enableAPS = _enableAps;

				if (_asset.enableAPS) {
					HandleApsSlots();
				}

				_asset.Sanitize();

				if (_asset.apiKey.IsNullOrEmpty()) {
					Debug.LogError("Apikey cannot be empty, object NimbusAdsManager not created");
					return;
				}

				if (_asset.publisherKey.IsNullOrEmpty()) {
					Debug.LogError("Publisher cannot be empty, object NimbusAdsManager not created");
					return;
				}

				if (!ValidateApsData()) {
					return;
				}

				AssetDatabase.CreateAsset(_asset,
					"Packages/com.adsbynimbus.unity/Runtime/Scripts/Nimbus.ScriptableObjects/NimbusSDKConfiguration.asset");
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
			var apsSlotData = new List<ApsSlotData>();
			for (var i = 0; i < _apsSlots.arraySize; i++) {
				var item = _apsSlots.GetArrayElementAtIndex(i);
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

			_asset.slotData = apsSlotData.ToArray();
		}

		private bool ValidateApsData() {
			if (_asset.enableAPS) {
				if (_appId.IsNullOrEmpty()) {
					Debug.LogError(
						"APS SDK has been included, the APS App ID cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (_asset.slotData.Length == 0) {
					Debug.LogError(
						"APS SDK has been included, APS placement slots need to be entered, object NimbusAdsManager not created");
					return false;
				}

				var seenAdTypes = new Dictionary<AdUnitType, bool>();
				foreach (var apsSlot in _asset.slotData) {
					if (apsSlot.SlotId.IsNullOrEmpty()) {
						Debug.LogError(
							"APS SDK has been included, the APS slot id cannot be empty, object NimbusAdsManager not created");
						return false;
					}

					if (apsSlot.AdUnitType == AdUnitType.Undefined) {
						Debug.LogError(
							"APS SDK has been included, Ad Unit type cannot be Undefined, object NimbusAdsManager not created");
						return false;
					}

					if (!seenAdTypes.ContainsKey(apsSlot.AdUnitType)) {
						seenAdTypes.Add(apsSlot.AdUnitType, true);
					}
					else {
						Debug.LogError(
							$"APS SDK has been included, APS cannot contain duplicate ad type {apsSlot.AdUnitType}, object NimbusAdsManager not created");
						return false;
					}
				}
			}

			return true;
		}
	}
}
#endif