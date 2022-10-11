#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Nimbus.Internal.Utility;
using Nimbus.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Nimbus.Editor {
	public class SkaAdNetworkEditor : EditorWindow {
		public const string SkaAdSavePath = "Packages/com.adsbynimbus.unity/Runtime/Plugins/iOS/SKAdNetworks";
		public const string SkaKey = "SKAdNetworkItems";
		public const string SkaItem = "SKAdNetworkIdentifier";
		
		private const string DisplayMessage = "Paste In SkaAdNetwork XML or newline seperated ids";
		
		private string _textInputData;
		private Vector2 _scroll;
		private EditorUtil.HelpBoxDataContainer? _helpBoxDataContainer;
		
		[MenuItem("Nimbus/SKAdNetwork Support")]
		public static void ThirdPartySDKIntegrationMacros() {
			GetWindow<SkaAdNetworkEditor>("SKAdNetwork Input");
		}
		
		private void OnGUI() {
			EditorDrawUtility.DrawEditorLayoutHorizontalLine(Color.gray, 5);
			var headerStyle = EditorStyles.largeLabel;
			headerStyle.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("Enter SkaAdNetwork Below", headerStyle);

			// Ska Text Field Input
			var data = !_textInputData.IsNullOrEmpty() ? _textInputData : DisplayMessage;
			_scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(position.height * .75f));
			var tempData = EditorGUILayout.TextArea(data, GUILayout.ExpandHeight(true));
			if (tempData != DisplayMessage) {
				_textInputData = tempData;
			}
			EditorGUILayout.EndScrollView();
			GUILayout.Space(25);
			// ReSharper disable InvertIf
			if (GUILayout.Button("Create File")) {
				string[] skaIds;
				if (_textInputData.IsNullOrEmpty()) {
					_helpBoxDataContainer = new EditorUtil.HelpBoxDataContainer("No SKAdNetwork was entered", MessageType.Error);
					return;
				}

				if (_textInputData.Contains("?xml version")) {
					var (plistDictionaryList, ok) = ValidateXML();
					if (!ok) {
						_helpBoxDataContainer = new EditorUtil.HelpBoxDataContainer("The submitted SKAdNetwork plist XML data is invalid",
							MessageType.Error);
						return;
					}

					skaIds = plistDictionaryList.SelectMany(group => group.Values).
						Cast<string>().
						ToArray();

					skaIds = SanitizeNetworkIds(skaIds);
					var (lineNumber, validationOk) = ValidateNetworkIds(skaIds);
					if (!validationOk) {
						_helpBoxDataContainer = new EditorUtil.HelpBoxDataContainer($"The submitted SKAdNetworks ids are not completely valid, " +
						                           $"see line number {lineNumber}", MessageType.Error);
						return;
					}
				}
				else {
					skaIds = _textInputData.Split('\n');
					skaIds = SanitizeNetworkIds(skaIds);
					var (lineNumber, ok) = ValidateNetworkIds(skaIds);
					if (!ok) {
						_helpBoxDataContainer = new EditorUtil.HelpBoxDataContainer($"The submitted SKAdNetworks ids are not completely valid, " +
							$"see line number {lineNumber}", MessageType.Error);
						return;
					}
				}
				CreateSkAdNetworkFile(skaIds);
				_helpBoxDataContainer = new EditorUtil.HelpBoxDataContainer($"File created at path {SkaAdSavePath}", MessageType.Info);
			}

			if (_helpBoxDataContainer.HasValue) {
				EditorGUILayout.HelpBox(_helpBoxDataContainer.Value.message, _helpBoxDataContainer.Value.type);
			}
		}

		private Tuple<List<PlistDictionary>, bool> ValidateXML() {
			var plist = new PlistDictionary(_textInputData);
			if (!plist.TryGetValue(SkaKey, out var ska)) return new Tuple<List<PlistDictionary>, bool>(null, false);

			var idsArray = (List<object>)ska;
			var castedIds = idsArray.
				Cast<PlistDictionary>().
				ToList();
			
			return castedIds.SelectMany(group => group).Any(map => map.Key != SkaItem)
				? new Tuple<List<PlistDictionary>, bool>(null, false)
				: new Tuple<List<PlistDictionary>, bool>(castedIds, castedIds.Count > 0);
		}

		private static string[] SanitizeNetworkIds(IEnumerable<string> networkIds) {
			return networkIds.
				Where(s => !s.IsNullOrEmpty()).
				Select(s => s.Trim()).
				ToArray();
		}

		private static Tuple<int, bool> ValidateNetworkIds(IReadOnlyList<string> networkIds) {
			// ReSharper disable once StringLiteralTypo
			const string networkIdSuffix = "skadnetwork";
			for (var i = 0; i < networkIds.Count(); i++) {
				if (!networkIds[i].Contains(networkIdSuffix)) {
					return new Tuple<int, bool>(i + 1, false);
				}

				if (!networkIds[i].Contains(".") || networkIds[i].Count(ch => (ch == '.')) > 1) {
					return new Tuple<int, bool>(i + 1, false);
				}
			}

			return new Tuple<int, bool>(-1, true);
		}

		private static void CreateSkAdNetworkFile(IEnumerable<string> skaIds) {
			var builder = new StringBuilder();
			foreach (var id in skaIds) {
				builder.AppendLine(id);
			}

			File.WriteAllText(SkaAdSavePath, builder.ToString());
		}
	}
}
#endif