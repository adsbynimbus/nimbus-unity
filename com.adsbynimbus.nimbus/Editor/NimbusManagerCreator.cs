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
		private bool _enableManualInitialization = false;
		private NimbusSDKConfiguration _asset = null;

		// APS
		private SerializedProperty _androidAppId;
		private ReorderableList _androidApsSlotIdList = null;
		private SerializedProperty _androidApsSlots = null;
		private SerializedProperty _androidApsTimeoutInMilliseconds;

		private SerializedProperty _iosAppId;
		private ReorderableList _iosApsSlotIdList = null;
		private SerializedProperty _iosApsSlots = null;
		private SerializedProperty _iosApsTimeoutInMilliseconds;
		
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
		
		// Moloco
		private SerializedProperty _androidMolocoAppKey;
		private ReorderableList _androidMolocoAdUnitDataList = null;
		private SerializedProperty _androidMolocoAdUnitData = null;
		
		private SerializedProperty _iosMolocoAppKey;
		private ReorderableList _iosMolocoAdUnitDataList = null;
		private SerializedProperty _iosMolocoAdUnitData = null;
		
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
			_androidApsTimeoutInMilliseconds = serializedObject.FindProperty("androidApsTimeoutInMilliseconds");
			_androidApsTimeoutInMilliseconds.intValue = serializedObject.FindProperty("androidApsTimeoutInMilliseconds").intValue 
			                                            == 0 ? NimbusSDKConfiguration.ApsDefaultTimeout : serializedObject.FindProperty("androidApsTimeoutInMilliseconds").intValue;
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
			_androidApsSlotIdList.drawElementCallback += OnDrawElementApsAndroidSlotData;

			// IOS APS UI
			_iosAppId = serializedObject.FindProperty("iosAppID");
			_iosApsTimeoutInMilliseconds = serializedObject.FindProperty("iosApsTimeoutInMilliseconds");
			_iosApsTimeoutInMilliseconds.intValue = serializedObject.FindProperty("iosApsTimeoutInMilliseconds").intValue 
			                                        == 0 ? NimbusSDKConfiguration.ApsDefaultTimeout : serializedObject.FindProperty("iosApsTimeoutInMilliseconds").intValue;
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
			_iosApsSlotIdList.drawElementCallback += OnDrawElementApsIOSSlotData;
			
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
			_androidAdMobAdUnitDataList.drawElementCallback += OnDrawElementAdMobAndroidAdUnitData;
			
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
			_iosAdMobAdUnitDataList.drawElementCallback += OnDrawElementAdMobIOSAdUnitData;
			
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
			_androidMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralAndroidAdUnitData;
			
			// IOS Mintegral UI
			_iosMintegralAppId = serializedObject.FindProperty("iosMintegralAppID");
			_iosMintegralAppKey = serializedObject.FindProperty("iosMintegralAppKey");
			_iosMintegralAdUnitData = serializedObject.FindProperty("iosMintegralAdUnitData");
			_iosMintegralAdUnitDataList = new ReorderableList(
				serializedObject, _iosMintegralAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosMintegralAdUnitData.isExpanded = true;
			_iosMintegralAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosMintegralAdUnitDataList.headerHeight = 0f;
			_iosMintegralAdUnitDataList.drawElementCallback += OnDrawElementMintegralIOSAdUnitData;
			
			// Unity Ads
			// Android Unity Ads UI
			_androidUnityAdsGameId = serializedObject.FindProperty("androidUnityAdsGameID");
			
			// IOS Unity Ads UI
			_iosUnityAdsGameId = serializedObject.FindProperty("iosUnityAdsGameID");
			
			//Moloco
			// Android Moloco UI
			_androidMolocoAppKey = serializedObject.FindProperty("androidMolocoAppKey");
			_androidMolocoAdUnitData = serializedObject.FindProperty("androidMolocoAdUnitData");
			_androidMolocoAdUnitDataList = new ReorderableList(
				serializedObject, _androidMolocoAdUnitData,
				true,
				false,
				true,
				true
			);
			_androidMolocoAdUnitData.isExpanded = true;
			_androidMolocoAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_androidMolocoAdUnitDataList.headerHeight = 0f;
			_androidMolocoAdUnitDataList.drawElementCallback += OnDrawElementMolocoAndroidAdUnitData;
			
			// IOS Moloco UI
			_iosMolocoAppKey = serializedObject.FindProperty("iosMolocoAppKey");
			_iosMolocoAdUnitData = serializedObject.FindProperty("iosMolocoAdUnitData");
			_iosMolocoAdUnitDataList = new ReorderableList(
				serializedObject, _iosMolocoAdUnitData,
				true,
				false,
				true,
				true
			);
			_iosMolocoAdUnitData.isExpanded = true;
			_iosMolocoAdUnitDataList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
			_iosMolocoAdUnitDataList.headerHeight = 0f;
			_iosMolocoAdUnitDataList.drawElementCallback += OnDrawElementMolocoIOSAdUnitData;
		}


		private void OnDisable() {
			_androidApsSlotIdList.drawElementCallback -= OnDrawElementApsAndroidSlotData;
			_iosApsSlotIdList.drawElementCallback -= OnDrawElementApsIOSSlotData;
			_androidAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobAndroidAdUnitData;
			_iosAdMobAdUnitDataList.drawElementCallback -= OnDrawElementAdMobIOSAdUnitData;
			_androidMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralAndroidAdUnitData;
			_iosMintegralAdUnitDataList.drawElementCallback -= OnDrawElementMintegralIOSAdUnitData;
			_androidMolocoAdUnitDataList.drawElementCallback -= OnDrawElementMolocoAndroidAdUnitData;
			_iosMolocoAdUnitDataList.drawElementCallback -= OnDrawElementMolocoIOSAdUnitData;
		}

		private void OnDrawElementApsAndroidSlotData(Rect rect, int index, bool isActive, bool isFocused) {
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
		private void OnDrawElementApsIOSSlotData(Rect rect, int index, bool isActive, bool isFocused) {
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
		
		private void OnDrawElementAdMobAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidAdMobAdUnitData.GetArrayElementAtIndex(index);
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
		private void OnDrawElementAdMobIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosAdMobAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMintegralAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidMintegralAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMintegralIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosMintegralAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMolocoAndroidAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _androidMolocoAdUnitData.GetArrayElementAtIndex(index);
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
		
		private void OnDrawElementMolocoIOSAdUnitData(Rect rect, int index, bool isActive, bool isFocused) {
			var fieldRect = rect;
			fieldRect.height = EditorGUIUtility.singleLineHeight;
			var item = _iosMolocoAdUnitData.GetArrayElementAtIndex(index);
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
			_enableManualInitialization = EditorGUILayout.Toggle("Enable Manual Initialization", _enableManualInitialization);
			
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			EditorGUIUtility.labelWidth = 200.0f; 
			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;
			
			#if NIMBUS_ENABLE_APS || NIMBUS_ENABLE_VUNGLE || NIMBUS_ENABLE_META || NIMBUS_ENABLE_ADMOB || NIMBUS_ENABLE_MINTEGRAL || NIMBUS_ENABLE_UNITY_ADS || NIMBUS_ENABLE_MOBILEFUSE || NIMBUS_ENABLE_LIVERAMP || NIMBUS_ENABLE_MOLOCO
				EditorGUILayout.LabelField("Third Party SDK Support", headerStyle);
			#endif
			
			#if NIMBUS_ENABLE_LIVERAMP_ANDROID || NIMBUS_ENABLE_LIVERAMP_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("LiveRamp Configuration", headerStyle);
				#if NIMBUS_ENABLE_LIVERAMP_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.LabelField("LiveRamp is Enabled for Android", EditorStyles.label);
				#endif
							
				#if NIMBUS_ENABLE_LIVERAMP_IOS
					GUILayout.Space(10);
					EditorGUILayout.LabelField("LiveRamp is Enabled for iOS", EditorStyles.label);
				#endif
			#endif

			#if NIMBUS_ENABLE_APS_ANDROID || NIMBUS_ENABLE_APS_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("APS Configuration", headerStyle);
				#if NIMBUS_ENABLE_APS_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidAppId));
					_androidApsTimeoutInMilliseconds.intValue = EditorGUILayout.IntField("Timeout in Milliseconds", value: _androidApsTimeoutInMilliseconds.intValue);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidApsSlots, "APS Android Slot Id Data");
				#endif
				#if NIMBUS_ENABLE_APS_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosAppId));
					_iosApsTimeoutInMilliseconds.intValue = EditorGUILayout.IntField("Timeout in Milliseconds", _iosApsTimeoutInMilliseconds.intValue);
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_iosApsSlots, "APS iOS Slot Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter APS data", MessageType.Warning);
				#endif
			#endif	
			
			#if NIMBUS_ENABLE_VUNGLE_ANDROID || NIMBUS_ENABLE_VUNGLE_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Vungle Configuration", headerStyle);
				#if NIMBUS_ENABLE_VUNGLE_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidVungleAppId));
				#endif
				#if NIMBUS_ENABLE_VUNGLE_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosVungleAppId));
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Vungle data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_META_ANDROID || NIMBUS_ENABLE_META_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Meta Configuration", headerStyle);
				#if NIMBUS_ENABLE_META_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidMetaAppId));
				#endif
				#if NIMBUS_ENABLE_META_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMetaAppId));
				#endif
				GUILayout.Space(10);
				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Meta data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_ADMOB_ANDROID || NIMBUS_ENABLE_ADMOB_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("AdMob Configuration", headerStyle);
				#if NIMBUS_ENABLE_ADMOB_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidAdMobAppId));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidAdMobAdUnitData, "AdMob Android Ad Unit Id Data");
				#endif
				#if NIMBUS_ENABLE_ADMOB_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosAdMobAppId));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosAdMobAdUnitData, "AdMob iOS Ad Unit Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter AdMob data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MINTEGRAL_ANDROID || NIMBUS_ENABLE_MINTEGRAL_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Mintegral Configuration", headerStyle);
				#if NIMBUS_ENABLE_MINTEGRAL_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidMintegralAppId));
					EditorGUILayout.PropertyField((_androidMintegralAppKey));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidMintegralAdUnitData, "Mintegral Android Ad Unit Id Data");
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMintegralAppId));
					EditorGUILayout.PropertyField((_iosMintegralAppKey));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosMintegralAdUnitData, "Mintegral iOS Ad Unit Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Mintegral data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_UNITY_ADS_ANDROID || NIMBUS_ENABLE_UNITY_ADS_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Unity Ads Configuration", headerStyle);
				#if NIMBUS_ENABLE_UNITY_ADS_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_androidUnityAdsGameId);
				#endif
				#if NIMBUS_ENABLE_UNITY_ADS_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField(_iosUnityAdsGameId);
				#endif
				GUILayout.Space(10);

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Unity Ads data", MessageType.Warning);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MOBILEFUSE_ANDROID || NIMBUS_ENABLE_MOBILEFUSE_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("MobileFuse Configuration", headerStyle);
				#if NIMBUS_ENABLE_MOBILEFUSE_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.LabelField("MobileFuse is Enabled for Android", EditorStyles.label);
				#endif

				#if NIMBUS_ENABLE_MOBILEFUSE_IOS
					GUILayout.Space(10);
					EditorGUILayout.LabelField("MobileFuse is Enabled for iOS", EditorStyles.label);
				#endif
			#endif
			
			#if NIMBUS_ENABLE_MOLOCO_ANDROID || NIMBUS_ENABLE_MOLOCO_IOS
				EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 2);
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Moloco Configuration", headerStyle);
				#if NIMBUS_ENABLE_MOLOCO_ANDROID
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_androidMolocoAppKey));
					EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray);
					EditorDrawUtility.DrawArray(_androidMolocoAdUnitData, "Moloco Android Ad Unit Id Data");
				#endif
					#if NIMBUS_ENABLE_MOLOCO_IOS
					GUILayout.Space(10);
					EditorGUILayout.PropertyField((_iosMolocoAppKey));
					GUILayout.Space(10);
					EditorDrawUtility.DrawArray(_iosMolocoAdUnitData, "Moloco iOS Ad Unit Id Data");
				#endif

				#if !UNITY_ANDROID && !UNITY_IOS
					EditorGUILayout.HelpBox("In build settings select Android or IOS to enter Moloco data", MessageType.Warning);
				#endif
			#endif
			
			// ReSharper disable InvertIf
			if (GUILayout.Button("Create")) {
				_asset.publisherKey = _publisherKey;
				_asset.apiKey = _apiKey;
				_asset.enableUnityLogs = _enableUnityLogs;
				_asset.enableSDKInTestMode = _enableSDKInTestMode;
				_asset.enableManualInitialization = _enableManualInitialization;

				#if NIMBUS_ENABLE_APS_ANDROID
					HandleApsSlots(_androidApsSlots, out _asset.androidApsSlotData);
				#endif
				#if NIMBUS_ENABLE_APS_IOS
					HandleApsSlots(_iosApsSlots, out _asset.iosApsSlotData);
				#endif
				
				#if NIMBUS_ENABLE_ADMOB_ANDROID
					HandleAdMobAdUnitData(_androidAdMobAdUnitData, out _asset.androidAdMobAdUnitData);
				#endif
				#if NIMBUS_ENABLE_ADMOB_IOS
					HandleAdMobAdUnitData(_iosAdMobAdUnitData, out _asset.iosAdMobAdUnitData);
				#endif
				
				#if NIMBUS_ENABLE_MINTEGRAL_ANDROID
					HandleMintegralAdUnitData(_androidMintegralAdUnitData, out _asset.androidMintegralAdUnitData);
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL_IOS
					HandleMintegralAdUnitData(_iosMintegralAdUnitData, out _asset.iosMintegralAdUnitData);
				#endif
				
				#if NIMBUS_ENABLE_MOLOCO_ANDROID
					HandleMolocoAdUnitData(_androidMolocoAdUnitData, out _asset.androidMolocoAdUnitData);
				#endif
				#if NIMBUS_ENABLE_MOLOCO_IOS
					HandleMolocoAdUnitData(_iosMolocoAdUnitData, out _asset.iosMolocoAdUnitData);
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

				#if NIMBUS_ENABLE_APS_ANDROID
					if (!ValidateApsData("Android", _androidAppId, _asset.androidApsSlotData)) {
						return;
					}
					_asset.androidAppID = _androidAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_APS_IOS
					if (!ValidateApsData("iOS", _iosAppId, _asset.iosApsSlotData)) {
						return;
					}
					_asset.iosAppID = _iosAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_VUNGLE_ANDROID
					if (!ValidateVungleData("Android", _androidVungleAppId)) {
						return;
					}
					_asset.androidVungleAppID = _androidVungleAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_VUNGLE_IOS
					if (!ValidateVungleData("iOS", _iosVungleAppId)) {
						return;
					}
					_asset.iosVungleAppID = _iosVungleAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_META_ANDROID
					if (!ValidateMetaData("Android", _androidMetaAppId)) {
						return;
					}
					_asset.androidMetaAppID = _androidMetaAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_META_IOS
					if (!ValidateMetaData("iOS", _iosMetaAppId)) {
						return;
					}
					_asset.iosMetaAppID = _iosMetaAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_ADMOB_ANDROID
					if (!ValidateAdMobData("Android", _androidAdMobAppId, _asset.androidAdMobAdUnitData)) {
						return;
					}
					_asset.androidAdMobAppID = _androidAdMobAppId.stringValue;
				#endif
				#if NIMBUS_ENABLE_ADMOB_IOS
					if (!ValidateAdMobData("iOS", _iosAdMobAppId, _asset.iosAdMobAdUnitData)) {
						return;
					}
					_asset.iosAdMobAppID = _iosAdMobAppId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_MINTEGRAL_ANDROID
					if (!ValidateMintegralData("Android", _androidMintegralAppId, _androidMintegralAppKey, _asset.androidMintegralAdUnitData)) {
						return;
					}
					_asset.androidMintegralAppID = _androidMintegralAppId.stringValue;
					_asset.androidMintegralAppKey = _androidMintegralAppKey.stringValue;
				#endif
				#if NIMBUS_ENABLE_MINTEGRAL_IOS
					if (!ValidateMintegralData("iOS", _iosMintegralAppId, _iosMintegralAppKey, _asset.iosMintegralAdUnitData)) {
						return;
					}
					_asset.iosMintegralAppID = _iosMintegralAppId.stringValue;
					_asset.iosMintegralAppKey = _iosMintegralAppKey.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_UNITY_ADS_ANDROID
					if (!ValidateUnityAdsData("Android", _androidUnityAdsGameId)) {
						return;
					}
					_asset.androidUnityAdsGameID = _androidUnityAdsGameId.stringValue;
				#endif
				#if NIMBUS_ENABLE_UNITY_ADS_IOS
					if (!ValidateUnityAdsData("iOS", _iosUnityAdsGameId)) {
						return;
					}
					_asset.iosUnityAdsGameID = _iosUnityAdsGameId.stringValue;
				#endif
				
				#if NIMBUS_ENABLE_MOLOCO_ANDROID
					if (!ValidateMolocoData("Android", _androidMolocoAppKey, _asset.androidMolocoAdUnitData)) {
						return;
					}
					_asset.androidMintegralAppKey = _androidMolocoAppKey.stringValue;
				#endif
				#if NIMBUS_ENABLE_MOLOCO_IOS
					if (!ValidateMolocoData("iOS", _iosMolocoAppKey, _asset.iosMolocoAdUnitData)) {
						return;
					}
					_asset.iosMolocoAppKey = _iosMolocoAppKey.stringValue;
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


		private void HandleApsSlots(SerializedProperty slotData, out ApsSlotData[] platformSlots) {
			var apsSlotData = new List<ApsSlotData>();
			for (var i = 0; i < slotData.arraySize; i++) {
				var item = slotData.GetArrayElementAtIndex(i);
				var slotId = item.FindPropertyRelative("SlotId");

				var apsData = new ApsSlotData {
					SlotId = slotId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("APSAdUnitType");
				if (adUnitType != null) {
					apsData.APSAdUnitType = (APSAdUnitType)adUnitType.enumValueIndex;
				}

				apsSlotData.Add(apsData);
			}
			platformSlots = apsSlotData.ToArray();
		}	

		private bool ValidateApsData(string platform, SerializedProperty appId, ApsSlotData[] slotData) {
			if (appId.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					"APS SDK has been included, the APS App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			
			if (slotData == null || slotData.Length == 0) {
				Debug.unityLogger.LogError("Nimbus", 
					$"APS SDK has been included, APS placement slots for {platform} need to be entered, object NimbusAdsManager not created");
				return false;
			}

			var seenAdTypes = new Dictionary<APSAdUnitType, bool>();
			foreach (var apsSlot in slotData) {
				if (apsSlot.SlotId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, the APS slot id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(apsSlot.APSAdUnitType)) {
					seenAdTypes.Add(apsSlot.APSAdUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"APS SDK has been included, APS cannot contain duplicate ad type {apsSlot.APSAdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
		
		private bool ValidateVungleData(string platform, SerializedProperty appId) { 
			if (appId.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Vungle SDK has been included, the {platform} Vungle App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			return true;
		}
		
		private bool ValidateMetaData(string platform, SerializedProperty appId) {
			if (appId.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Meta SDK has been included, the {platform} Meta App ID cannot be empty, object NimbusAdsManager not created");
				return false; 
			}
			return true;
		}

		private void HandleAdMobAdUnitData(SerializedProperty adUnitData, out ThirdPartyAdUnit[] adUnits)
		{
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
			adUnits = adUnitList.ToArray();
		}
		
		private bool ValidateAdMobData(string platform, SerializedProperty appId, ThirdPartyAdUnit[] adUnitData) {
			
			if (appId.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"AdMob SDK has been included, the {platform} AdMob App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			
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
		
		private void HandleMintegralAdUnitData(SerializedProperty adUnitData, out ThirdPartyAdUnit[] adUnits)
		{
			var adUnitList = new List<ThirdPartyAdUnit>();
			for (var i = 0; i < adUnitData.arraySize; i++) {
				var item = adUnitData.GetArrayElementAtIndex(i);
				var adUnitId = item.FindPropertyRelative("AdUnitId");
				var adUnitPlacementId = item.FindPropertyRelative("AdUnitPlacementId");

				var mintegralData  = new ThirdPartyAdUnit() {
					AdUnitId = adUnitId?.stringValue,
					AdUnitPlacementId = adUnitPlacementId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("AdUnitType");
				if (adUnitType != null) {
					mintegralData.AdUnitType = (AdUnitType)adUnitType.enumValueIndex;
				}

				adUnitList.Add(mintegralData);
			}
			adUnits = adUnitList.ToArray();
		}
		
		private bool ValidateMintegralData(string platform, SerializedProperty appId, SerializedProperty appKey, ThirdPartyAdUnit[] adUnitData) {
			
			if (appId.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Mintegral SDK has been included, the {platform} Mintegral App ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			if (appKey.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Mintegral SDK has been included, the {platform} Mintegral App Key cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			
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
		
		private bool ValidateUnityAdsData(string platform, SerializedProperty gameId) {
			if (gameId.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Unity Ads SDK has been included, the {platform} Unity Ads Game ID cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			return true;
		}
		
		private void HandleMolocoAdUnitData(SerializedProperty adUnitData, out ThirdPartyAdUnit[] adUnits)
		{
			var adUnitList = new List<ThirdPartyAdUnit>();
			for (var i = 0; i < adUnitData.arraySize; i++) {
				var item = adUnitData.GetArrayElementAtIndex(i);
				var adUnitId = item.FindPropertyRelative("AdUnitId");
				var adUnitPlacementId = item.FindPropertyRelative("AdUnitPlacementId");

				var molocoData  = new ThirdPartyAdUnit() {
					AdUnitId = adUnitId?.stringValue,
					AdUnitPlacementId = adUnitPlacementId?.stringValue
				};

				var adUnitType = item.FindPropertyRelative("AdUnitType");
				if (adUnitType != null) {
					molocoData.AdUnitType = (AdUnitType)adUnitType.enumValueIndex;
				}

				adUnitList.Add(molocoData);
			}
			adUnits = adUnitList.ToArray();
		}
		
		private bool ValidateMolocoData(string platform, SerializedProperty appKey, ThirdPartyAdUnit[] adUnitData) {
			if (appKey.stringValue.IsNullOrEmpty()) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Moloco SDK has been included, the {platform} Moloco App Key cannot be empty, object NimbusAdsManager not created");
				return false;
			}
			
			if (adUnitData == null || adUnitData.Length == 0) {
				Debug.unityLogger.LogError("Nimbus", 
					$"Moloco SDK has been included, Moloco Ad Unit ids for {platform} need to be entered, object NimbusAdsManager not created");
				return false;
			}

			var seenAdTypes = new Dictionary<AdUnitType, bool>();
			foreach (var adUnit in adUnitData) {
				if (adUnit.AdUnitId.IsNullOrEmpty()) {
					Debug.unityLogger.LogError("Nimbus", 
						$"Moloco SDK has been included, the Ad Unit id for {platform} cannot be empty, object NimbusAdsManager not created");
					return false;
				}

				if (adUnit.AdUnitType == AdUnitType.Undefined) {
					Debug.unityLogger.LogError("Nimbus", 
						$"Moloco SDK has been included, Ad Unit type for {platform} cannot be Undefined, object NimbusAdsManager not created");
					return false;
				}

				if (!seenAdTypes.ContainsKey(adUnit.AdUnitType)) {
					seenAdTypes.Add(adUnit.AdUnitType, true);
				}
				else {
					Debug.unityLogger.LogError("Nimbus", 
						$"Moloco SDK has been included, Moloco cannot contain duplicate ad type {adUnit.AdUnitType} for {platform}, object NimbusAdsManager not created");
					return false;
				}
			}
			return true;
		}
	}
}
#endif