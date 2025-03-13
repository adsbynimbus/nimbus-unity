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
		
		// Unity Ads
		private SerializedProperty _androidUnityAdsGameId;
		
		private SerializedProperty _iosUnityAdsGameId;
		
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
			
			//Mintegral
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
				serializedObject, _iosAdMobAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosMintegralAdUnitData.isExpanded = true;
			_iosMintegralAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosMintegralAdUnitDataList.headerHeight = 0f;
			_iosMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralAdUnitData;
			
			// Unity Ads
			// Android Unity Ads UI
			_androidUnityAdsGameId = serializedObject.FindProperty("androidUnityAdsGameID");
			
			// IOS Unity Ads UI
			_iosUnityAdsGameId = serializedObject.FindProperty("iosUnityAdsGameID");
		}


		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementApsSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementApsSlotData;
			_androidAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAdUnitData;
			_iosAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAdUnitData;
			_androidMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAdUnitData;
			_iosMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAdUnitData;
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

		private void OnGUI() {
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			_publisherKey = EditorGUILayout.TextField("Publisher Key", _publisherKey);
			_apiKey = EditorGUILayout.TextField("ApiKey", _apiKey);
			_enableUnityLogs = EditorGUILayout.Toggle("Enable Unity Logger", _enableUnityLogs);
			_enableSDKInTestMode = EditorGUILayout.Toggle("Enable SDK In Test Mode", _enableSDKInTestMode);
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			EditorGUIUtility.labelWidth = 200.0f; 
			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS
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
			
			#if NIMBUS_ENABLE_UNITY_ADS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Unity Ads Configuration", headerStyle);
				#if UNITY_ANDROID
					EditorGUILayout.PropertyField(_androidUnityAdsGameId);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if UNITY_IOS
					EditorGUILayout.PropertyField(_iosUnityAdsGameId);
					GUILayout.Space(10);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Unity Ads data", MessageType.Warning);
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
				
				#if NIMBUS_ENABLE_ADMOB
					HandleAdMobAdUnitData();
				#endif
				
				#if NIMBUS_ENABLE_MINTEGRAL
					HandleMintegralAdUnitData();
				#endif
				
				_asset.Sanitize();
				if (_asset.apiKey.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						"Apikey cannot be empty, object NimbusAdsManager not created");
					return;
				}

				if (_asset.publisherKey.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						"Publisher cannot be empty, object NimbusAdsManager not created");
					return;
				}

				#if NIMBUS_ENABLE_APS
					if (!ValidateApsData()) {
						return;
					}
				#endif
				
				#if NIMBUS_ENABLE_VUNGLE
					if (!ValidateVungleData()) {
						return;
					}
				#endif
				
				#if NIMBUS_ENABLE_META
					if (!ValidateMetaData()) {
						return;
					}
				#endif	
				
				#if NIMBUS_ENABLE_ADMOB
					if (!ValidateAdMobData()) {
						return;
					}
				#endif
				
				#if NIMBUS_ENABLE_MINTEGRAL
					if (!ValidateMintegralData()) {
						return;
					}
				#endif
				
				#if NIMBUS_ENABLE_UNITY_ADS
					if (!ValidateUnityAdsData()) {
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
				Debug.unityLogger.LogError("Nimbus", 
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
				Debug.unityLogger.LogError("Nimbus", 
					$"APS SDK has been included, APS placement slots for {platform} need to be entered, object NimbusAdsManager not created");
				return false;
			}

			var seenAdTypes = new Dictionary<AdUnitType, bool>();
			foreach (var apsSlot in slotData) {
				if (apsSlot.SlotId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, the APS slot id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (apsSlot.AdUnitType == AdUnitType.Undefined) {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, Ad Unit type for {platform} cannot be Undefined, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(apsSlot.AdUnitType)) {
					seenAdTypes.Add(apsSlot.AdUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, APS cannot contain duplicate ad type {apsSlot.AdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
		
		private bool ValidateVungleData() {
			string appId = null;
			#if UNITY_ANDROID
				appId = _androidVungleAppId.stringValue;
			#elif UNITY_IOS
				appId = _iosVungleAppId.stringValue;
			#endif
			
			if (appId.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"Vungle SDK has been included, the Vungle App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			return true;
		}
		
		private bool ValidateMetaData() {
			string appId = null;
			#if UNITY_ANDROID
				appId = _androidMetaAppId.stringValue;
			#elif UNITY_IOS
				appId = _iosMetaAppId.stringValue;
			#endif
			
			if (appId.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"Meta SDK has been included, the Meta App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			return true;
		}

		private void HandleAdMobAdUnitData()
		{
			SerializedProperty adUnitData = null;
			#if UNITY_ANDROID
				adUnitData = _androidAdMobAdUnitData;
			# elif UNITY_IOS
				adUnitData = _iosAdMobAdUnitData;
			#endif
			var adUnitList = new List<ThirdPartyAdUnit>();
			for (var i = 0; i < adUnitData.arraySize; i++) {
				var item = adUnitData.GetArrayElementAtIndex(i);
				var adUnitId = item.FindPropertyRelative("AdUnitId");

				var adMobData  = new ThirdPartyAdUnit() {
					AdUnitId = adUnitId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("AdUnitType");
				if (adUnitType != null) {
					adMobData.AdUnitType = (AdUnitType)adUnitType.enumValueIndex;
				}

				adUnitList.Add(adMobData);
			}	

			#if UNITY_ANDROID
				_asset.androidAdMobAdUnitData = adUnitList.ToArray();
			# elif UNITY_IOS
				_asset.iosAdMobAdUnitData = adUnitList.ToArray();
			#endif
		}
		
		private bool ValidateAdMobData() {
			string appId = null;
			#if UNITY_ANDROID
				appId = _androidAdMobAppId.stringValue;
			#elif UNITY_IOS
				appId = _iosAdMobAppId.stringValue;
			#endif
			
			if (appId.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"AdMob SDK has been included, the AdMob App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			
			ThirdPartyAdUnit[] adUnitData = null;
			var platform = "Android";
			#if UNITY_ANDROID
				adUnitData = _asset.androidAdMobAdUnitData;
			# elif UNITY_IOS
				adUnitData = _asset.iosAdMobAdUnitData;
				platform = "iOS";
			#endif
			
			if (adUnitData == null || adUnitData.Length == 0) {
				Debug.unityLogger.LogError("Nimbus", 
					$"AdMob SDK has been included, AdMob Ad Unit ids for {platform} need to be entered, object NimbusAdsManager not created");
				return false;
			}

			var seenAdTypes = new Dictionary<AdUnitType, bool>();
			foreach (var adUnit in adUnitData) {
				if (adUnit.AdUnitId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"AdMob SDK has been included, the Ad Unit id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (adUnit.AdUnitType == AdUnitType.Undefined) {
					Debug.unityLogger.LogError("Nimbus", 
						$"AdMob SDK has been included, Ad Unit type for {platform} cannot be Undefined, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(adUnit.AdUnitType)) {
					seenAdTypes.Add(adUnit.AdUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"AdMob SDK has been included, AdMob cannot contain duplicate ad type {adUnit.AdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
		
		private void HandleMintegralAdUnitData()
		{
			SerializedProperty adUnitData = null;
			#if UNITY_ANDROID
				adUnitData = _androidMintegralAdUnitData;
			# elif UNITY_IOS
				adUnitData = _iosMintegralAdUnitData;
			#endif
			var adUnitList = new List<ThirdPartyAdUnit>();
			for (var i = 0; i < adUnitData.arraySize; i++) {
				var item = adUnitData.GetArrayElementAtIndex(i);
				var adUnitId = item.FindPropertyRelative("AdUnitId");

				var mintegralData  = new ThirdPartyAdUnit() {
					AdUnitId = adUnitId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("AdUnitType");
				if (adUnitType != null) {
					mintegralData.AdUnitType = (AdUnitType)adUnitType.enumValueIndex;
				}

				adUnitList.Add(mintegralData);
			}	

			#if UNITY_ANDROID
				_asset.androidMintegralAdUnitData = adUnitList.ToArray();
			# elif UNITY_IOS
				_asset.iosMintegralAdUnitData = adUnitList.ToArray();
			#endif
		}
		
		private bool ValidateMintegralData() {
			string appId = null;
			string appKey = null;
			#if UNITY_ANDROID
				appId = _androidMintegralAppId.stringValue;
				appKey = _androidMintegralAppKey.stringValue;
			#elif UNITY_IOS
				appId = _iosMintegralAppId.stringValue;
				appKey = _iosMintegralAppKey.stringValue;
			#endif
			
			if (appId.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"Mintegral SDK has been included, the Mintegral App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			if (appKey.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"Mintegral SDK has been included, the Mintegral App Key cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			
			ThirdPartyAdUnit[] adUnitData = null;
			var platform = "Android";
			#if UNITY_ANDROID
				adUnitData = _asset.androidMintegralAdUnitData;
			# elif UNITY_IOS
				adUnitData = _asset.iosMintegralAdUnitData;
				platform = "iOS";
			#endif
			
			if (adUnitData == null || adUnitData.Length == 0) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Mintegral SDK has been included, Mintegral Ad Unit ids for {platform} need to be entered, object NimbusAdsManager not created");
				return false;
			}

			var seenAdTypes = new Dictionary<AdUnitType, bool>();
			foreach (var adUnit in adUnitData) {
				if (adUnit.AdUnitId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"Mintegral SDK has been included, the Ad Unit id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (adUnit.AdUnitType == AdUnitType.Undefined) {
					Debug.unityLogger.LogError("Nimbus", 
						$"Mintegral SDK has been included, Ad Unit type for {platform} cannot be Undefined, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(adUnit.AdUnitType)) {
					seenAdTypes.Add(adUnit.AdUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"Mintegral SDK has been included, Mintegral cannot contain duplicate ad type {adUnit.AdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
		
		private bool ValidateUnityAdsData() {
			string appId = null;
			#if UNITY_ANDROID
				appId = _androidUnityAdsGameId.stringValue;
			#elif UNITY_IOS
				appId = _iosUnityAdsGameId.stringValue;
			#endif
			
			if (appId.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"Unity Ads SDK has been included, the Unity Ads Game ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			return true;
		}
	}
}
#endif